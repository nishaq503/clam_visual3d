extern crate nalgebra as na;

use std::collections::HashMap;
use std::path::Path;
use std::sync::Arc;
// use std::thread;
use std::thread::JoinHandle;

use abd_clam::PartitionCriteria;
// use abd_clam::cluster::PartitionCriteria;
// use abd_clam::core::dataset::Dataset;
use abd_clam::Cakes;
use abd_clam::Tree;
use abd_clam::VecDataset;
// use abd_clam::dataset::VecVec;
// use abd_clam::search::cakes::CAKES;
// use glam::Vec3;

use crate::ffi_impl::cluster_ids_wrapper::ClusterIDsWrapper;
use crate::graph::force_directed_graph::{self, ForceDirectedGraph};
use crate::graph::spring;
use crate::tree_layout::reingold_tilford;
use crate::utils::distances::DistanceMetric;
use crate::utils::error::FFIError;
use crate::utils::types::{Cakesf32, Clusterf32, DataSet};
use crate::utils::{self, anomaly_readers, helpers};

use crate::{debug, CBFnNodeVisitor, CBFnNodeVisitorMut};

use crate::ffi_impl::cluster_data::ClusterData;
use crate::ffi_impl::cluster_data_wrapper::ClusterDataWrapper;
// use super::reingold_impl::{self};
use crate::graph::physics_node::PhysicsNode;
use spring::Spring;
use crate::ffi_impl::startup_data::StartupData;
// use crate::physics::ForceDirectedGraph;

// use ForceDirectedGraph;

// either leaf node or
// depth at least 4
// lfd - btwn 0.5 - 2.5
// color clusters by radius or lfd
// draw clusters with lfd value

// noedges btwn parents and children
// want edges btwn two clustesr whose distbtwn centers <= sum of radius
// add radius and lfd to display info

//tokio
// pub struct Handle2 {
//     root: Clusterf32,
//     dataset: & DataSet,
//     labels: & [u8],
// }

// struct TestDrop {
//     pub test: i32,
// }

// impl Drop for TestDrop {
//     fn drop(&mut self) {
//         debug!("drop test");
//     }
// }

pub struct Handle {
    cakes: Option<Cakes<Vec<f32>, f32, DataSet>>,
    // cakes1: Option<Cakes<Vec<f32>, f32, VecDataset<f32,f32>>>,
    labels: Option<Vec<u8>>,
    graph: Option<HashMap<String, PhysicsNode>>,
    edges: Option<Vec<Spring>>,
    current_query: Option<Vec<f32>>,
    // longest_edge: Option<f32>,
    force_directed_graph: Option<(JoinHandle<()>, Arc<ForceDirectedGraph>)>,
    // _test_drop: Option<TestDrop>,
    num_edges_in_graph: Option<i32>, // temporary figure out better way later
}

// impl Drop for Handle {
//     fn drop(&mut self) {

//         debug!("DroppingHandle");
//     }
// }
impl Handle {
    pub fn shutdown(&mut self) {
        self.cakes = None;
        // self.dataset = None;
        self.labels = None;
    }

    pub fn get_tree(&self) -> Option<&Tree<Vec<f32>, f32, VecDataset<Vec<f32>, f32>>> {
        if let Some(cakes) = &self.cakes {
            return cakes.trees().first().map(|x| *x);
        } else {
            return None;
        }
    }

    pub fn data(&self) -> Option<&DataSet> {
        return if let Some(c) = &self.cakes {
            Some(self.get_tree().unwrap().data())
        } else {
            None
        };
    }
    pub fn root(&self) -> Option<&Clusterf32> {
        return if let Some(c) = &self.cakes {
            Some(self.get_tree().unwrap().root())
        } else {
            None
        };
    }

    pub fn labels(&self) -> Option<&Vec<u8>> {
        return if let Some(labels) = &self.labels {
            Some(&labels)
        } else {
            None
        };
    }

    pub fn set_cakes(&mut self, cakes: Cakes<Vec<f32>, f32, DataSet>) {
        self.cakes = Some(cakes);
    }

    pub fn cakes(&self) -> &Option<Cakes<Vec<f32>, f32, DataSet>> {
        &self.cakes
    }

    pub fn new(
        startup_data :&StartupData,
        // data_name: &str,
        // cardinality: usize,
        // distance_metric: DistanceMetric,
    ) -> Result<Self, FFIError> {
        let criteria = PartitionCriteria::new(true).with_min_cardinality(startup_data.cardinality as usize);
        match Self::create_dataset(startup_data){
            Ok((dataset, labels)) => {
                return Ok(Handle {
                    cakes: Some(Cakes::new(dataset, Some(1), &criteria)), //.build(&criteria)),
                    labels: Some(labels),
                    graph: None,
                    edges: None,
                    current_query: None,
                    // longest_edge: None,
                    force_directed_graph: None,
                    // _test_drop: Some(TestDrop { test: 5 }),
                    num_edges_in_graph: None,
                });
            }
            Err(_) => Err(FFIError::HandleInitFailed),
        }
    }

    pub fn load(startup_data: &StartupData) -> Result<Self, FFIError> {
        let c = Cakes::<Vec<f32>, f32, VecDataset<_, _>>::load(
            Path::new(&startup_data.data_name),
            utils::distances::from_enum(startup_data.distance_metric),
            startup_data.is_expensive,
        );
        match c {
            Ok(cakes) => {
                return Ok(Handle {
                    cakes: Some(cakes),
                    labels: None,
                    graph: None,
                    edges: None,
                    current_query: None,
                    // longest_edge: None,
                    force_directed_graph: None,
                    // _test_drop: Some(TestDrop { test: 5 }),
                    num_edges_in_graph: None,
                });
            }
            Err(_) => Err(FFIError::HandleInitFailed),
        }
    }

    fn create_dataset(
        startup_data: &StartupData
        // data_name: &str,
        // distance_metric: DistanceMetric,
        // distance_metric: fn(&Vec<f32>, &Vec<f32>) -> f32,
    ) -> Result<(DataSet, Vec<u8>), String> {
        match anomaly_readers::read_anomaly_data(&startup_data.data_name, false) {
            Ok((first_data, labels)) => {
                let dataset = VecDataset::new(
                    startup_data.data_name.clone(),
                    first_data,
                    utils::distances::from_enum(startup_data.distance_metric),
                    startup_data.is_expensive,
                );

                Ok((dataset, labels))
            }
            Err(e) => Err(e),
        }
    }

    pub unsafe fn force_physics_shutdown(&mut self) -> FFIError {
        // let mut finished = false;

        //** */ this function blocks the main thread which prevents ohysics from finishing -
        // need to notify working thread to stop and then wait for it

        if let Some(force_directed_graph) = &self.force_directed_graph {
            force_directed_graph::force_shutdown(&force_directed_graph.1);
            // let is_finished = force_directed_graph.0.is_finished();

            let _ = self.force_directed_graph.take().unwrap().0.join();

            self.force_directed_graph = None;
            debug!("shutting down physics");
            return FFIError::PhysicsFinished;
        }
        return FFIError::PhysicsAlreadyShutdown;
    }

    pub unsafe fn init_unity_edges(&mut self, edge_detect_cb: CBFnNodeVisitorMut) -> FFIError {
        // let mut finished = false;

        //** */ this function blocks the main thread which prevents ohysics from finishing -
        // need to notify working thread to stop and then wait for it

        if let Some(force_directed_graph) = &self.force_directed_graph {
            force_directed_graph::init_unity_edges(
                // self,
                &force_directed_graph.1,
                edge_detect_cb,
            );

            // let is_finished = force_directed_graph.0.is_finished();

            // let _ = self.force_directed_graph.take().unwrap().0.join();

            // self.force_directed_graph = None;
            // debug!("shutting down physics");
            // return FFIError::PhysicsFinished;
        }
        return FFIError::PhysicsAlreadyShutdown;
    }

    pub unsafe fn physics_update_async(&mut self, updater: CBFnNodeVisitor) -> FFIError {
        // let mut finished = false;
        if let Some(force_directed_graph) = &self.force_directed_graph {
            let is_finished = force_directed_graph.0.is_finished();

            return if is_finished {
                let _ = self.force_directed_graph.take().unwrap().0.join();
                self.force_directed_graph = None;
                debug!("shutting down physics");
                FFIError::PhysicsFinished
            } else {
                // debug!("try to update unity");

                force_directed_graph::try_update_unity(&force_directed_graph.1, updater)
            };
            // let update_result =
            //     physics::force_directed_graph::try_update_unity(&force_directed_graph.1);
        }

        return FFIError::PhysicsAlreadyShutdown;
    }

    pub fn set_graph(&mut self, graph: (JoinHandle<()>, Arc<ForceDirectedGraph>)) {
        self.force_directed_graph = Some(graph);
        if let Some(g) = &self.force_directed_graph {
            self.num_edges_in_graph = Some(force_directed_graph::get_num_edges(&g.1));
        }
    }

    pub fn get_num_edges_in_graph(&self) -> i32 {
        self.num_edges_in_graph.unwrap_or(-1)
    }

    pub unsafe fn color_by_dist_to_query(
        &self,
        id_arr: &[String],
        node_visitor: CBFnNodeVisitor,
    ) -> FFIError {
        for id in id_arr {
            match self.get_cluster(id.clone()) {
                Ok(cluster) => {
                    if let Some(query) = &self.current_query {
                        let mut baton_data = ClusterDataWrapper::from_cluster(cluster);

                        baton_data.data_mut().dist_to_query =
                            cluster.distance_to_instance(self.data().unwrap(), query);

                        node_visitor(Some(baton_data.data()));
                    } else {
                        return FFIError::QueryIsNull;
                    }
                }
                Err(e) => {
                    return e;
                }
            }
        }
        return FFIError::Ok;
    }

    pub unsafe fn for_each_dft(
        &self,
        node_visitor: CBFnNodeVisitor,
        start_node: String,
    ) -> FFIError {
        return if let Some(_) = &self.cakes {
            if start_node == "root" {
                if let Some(node) = self.root() {
                    Self::for_each_dft_helper(&node, node_visitor);
                    FFIError::Ok
                } else {
                    FFIError::HandleInitFailed
                }
            } else {
                match Self::get_cluster(&self, start_node) {
                    Ok(root) => {
                        Self::for_each_dft_helper(root, node_visitor);
                        FFIError::Ok
                    }
                    Err(e) => {
                        debug!("{:?}", e);
                        FFIError::InvalidStringPassed
                    }
                }
            }
        } else {
            FFIError::NullPointerPassed
        };
    }

    pub unsafe fn set_names(
        &self,
        node_visitor: crate::CBFnNameSetter,
        start_node: String,
    ) -> FFIError {
        return if let Some(_) = &self.cakes {
            if start_node == "root" {
                if let Some(node) = self.root() {
                    Self::set_names_helper(&node, node_visitor);
                    FFIError::Ok
                } else {
                    FFIError::HandleInitFailed
                }
            } else {
                match Self::get_cluster(&self, start_node) {
                    Ok(root) => {
                        Self::set_names_helper(root, node_visitor);
                        FFIError::Ok
                    }
                    Err(e) => {
                        debug!("{:?}", e);
                        FFIError::InvalidStringPassed
                    }
                }
            }
        } else {
            FFIError::NullPointerPassed
        };
    }

    fn set_names_helper(root: &Clusterf32, node_visitor: crate::CBFnNameSetter) {
        if root.is_leaf() {
            let baton = ClusterIDsWrapper::from_cluster(&root);

            node_visitor(Some(baton.data()));
            // baton.free_ids();

            return;
        }
        if let Some([left, right]) = root.children() {
            debug!("node name: {:?}", root.name());
            let baton = ClusterIDsWrapper::from_cluster(&root);

            node_visitor(Some(baton.data()));
            // baton.free_ids();

            Self::set_names_helper(left, node_visitor);
            Self::set_names_helper(right, node_visitor);
        }
    }

    fn for_each_dft_helper(root: &Clusterf32, node_visitor: CBFnNodeVisitor) {
        if root.is_leaf() {
            let baton = ClusterDataWrapper::from_cluster(&root);

            node_visitor(Some(baton.data()));
            return;
        }
        if let Some([left, right]) = root.children() {
            let baton = ClusterDataWrapper::from_cluster(&root);

            node_visitor(Some(&baton.data()));

            Self::for_each_dft_helper(left, node_visitor);
            Self::for_each_dft_helper(right, node_visitor);
        }
    }

    pub fn shutdown_physics(&mut self) -> FFIError {
        let should_shutdown = { self.graph.is_some() && self.edges.is_some() };

        return if should_shutdown {
            self.graph = None;
            self.edges = None;
            FFIError::Ok
        } else {
            FFIError::PhysicsAlreadyShutdown
        };
    }

    pub fn set_current_query(&mut self, data: &Vec<f32>) {
        self.current_query = Some(data.clone());
    }

    pub fn get_current_query(&self) -> &Option<Vec<f32>> {
        &self.current_query
    }

    // pub fn rnn_search(
    //     &self,
    //     query: &Vec<f32>,
    //     radius: f32,
    // ) -> Result<(Vec<(&Clusterf32, f32)>, Vec<(&Clusterf32, f32)>), FFIError> {
    //     if let Some(cakes) = &self.cakes {
    //         // temporary fix later
    //         // self.current_query = Some(query.clone());
    //         return Ok(cakes.rnn_search_candidates(query, radius));
    //     }
    //     return Err(FFIError::NullPointerPassed);
    // }

    pub fn get_num_nodes(&self) -> i32 {
        if let Some(cakes) = &self.cakes {
            // self.get_tree().unwrap().root.num_descendants() as i32
            self.get_tree().unwrap().cardinality() as i32
        } else {
            0
        }
    }

    pub fn tree_height(&self) -> i32 {
        if let Some(cakes) = &self.cakes {
            self.get_tree().unwrap().depth() as i32
        } else {
            0
        }
    }

    // pub fn cardinality(&self) -> i32 {
    //     if let Some(cakes) = &self.cakes {
    //         self.get_tree().unwrap().root.cardinality() as i32
    //     } else {
    //         0
    //     }
    // }

    // pub fn radius(&self) -> f64 {
    //     if let Some(cakes) = &self.cakes {
    //         self.get_tree().unwrap().root().radius() as f64
    //     } else {
    //         0.
    //     }
    // }

    // pub fn lfd(&self) -> f64 {
    //     if let Some(cakes) = &self.cakes {
    //         self.get_tree().unwrap().root().lfd()
    //     } else {
    //         0.
    //     }
    // }

    // pub fn arg_center(&self) -> i32 {
    //     if let Some(cakes) = &self.cakes {
    //         self.get_tree().unwrap().root().arg_center() as i32
    //     } else {
    //         0
    //     }
    // }
    // pub fn arg_radius(&self) -> i32 {
    //     if let Some(cakes) = &self.cakes {
    //         self.get_tree().unwrap().root().arg_radius() as i32
    //     } else {
    //         0
    //     }
    // }

    // why isnt string taken by reference?
    pub unsafe fn get_cluster(&self, cluster_id: String) -> Result<&Clusterf32, FFIError> {
        if let Some(_) = &self.cakes {
            let mut parts = cluster_id.split('-');

            if let (Some(offset_str), Some(cardinality_str)) = (parts.next(), parts.next()) {
                if let (Ok(offset), Ok(cardinality)) = (
                    offset_str.parse::<usize>(),
                    cardinality_str.parse::<usize>(),
                ) {
                    println!("Offset: {}", offset);
                    println!("Cardinality: {}", cardinality);
                    if let Some(tree) = self.get_tree() {
                        if let Some(cluster) = tree.get_cluster(offset, cardinality) {
                            return Ok(cluster);
                        } else {
                            return Err(FFIError::InvalidStringPassed);
                        }
                    }
                }
            }
        }
        debug!("root not built");
        return Err(FFIError::HandleInitFailed);
    }

    // pub fn find_node_helper(root: &Clusterf32, mut path: String) -> Result<&Clusterf32, FFIError> {
    //     if path.len() == 0 {
    //         return Ok(&root);
    //     }
    //     let choice: char = path.pop().unwrap();
    //     return if let Some([left, right]) = root.children() {
    //         if choice == '0' {
    //             Self::find_node_helper(left, path)
    //         } else if choice == '1' {
    //             Self::find_node_helper(right, path)
    //         } else {
    //             Err(FFIError::InvalidStringPassed)
    //         }
    //     } else {
    //         Err(FFIError::InvalidStringPassed)
    //     };
    // }

    pub fn create_reingold_layout(&self, node_visitor: CBFnNodeVisitor) -> FFIError {
        return if let Some(cakes) = &self.cakes {
            reingold_tilford::run(
                self.root()
                    .unwrap_or_else(|| unreachable!("cakes exists - root should exist")),
                &self.labels,
                self.get_tree().unwrap().depth() as i32,
                node_visitor,
            )
        } else {
            FFIError::HandleInitFailed
        };
    }

    pub unsafe fn create_reingold_layout_offset_from(
        &self,
        root: &ClusterData,
        _current_depth: i32,
        max_depth: i32,
        node_visitor: CBFnNodeVisitor,
    ) -> FFIError {
        return if let Some(_) = &self.cakes {
            if let Ok(clam_root) = self.get_cluster(root.get_id()) {
                reingold_tilford::run_offset(
                    &root.pos,
                    clam_root,
                    &self.labels,
                    // current_depth,
                    max_depth,
                    node_visitor,
                )
            } else {
                FFIError::NullPointerPassed
            }
        } else {
            FFIError::HandleInitFailed
        };
    }
}
