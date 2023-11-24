extern crate nalgebra as na;

use std::cell::RefCell;
use std::collections::{HashMap, HashSet};
use std::path::Path;
use std::rc::Rc;
use std::sync::Arc;
// use std::thread;
use std::thread::JoinHandle;

use abd_clam::builder::{detect_edges, my_select_clusters, select_clusters};
use abd_clam::{ClusterSet, Edge, EdgeSet, Graph, MyEdge, MyEdgeSet, PartitionCriteria};
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
use crate::ffi_impl::tree_startup_data_ffi::TreeStartupDataFFI;
use crate::graph::physics_node::PhysicsNode;
use spring::Spring;

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
    cakes: Option<Rc<RefCell<Cakes<Vec<f32>, f32, DataSet>>>>,
    // cakes1: Option<Cakes<Vec<f32>, f32, VecDataset<f32,f32>>>,
    labels: Option<Vec<u8>>,
    graph: Option<HashMap<String, PhysicsNode>>,
    clam_graph: Option<Rc<RefCell<Graph<Vec<f32>, f32, DataSet>>>>,
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

    // pub fn get_tree(&'a self) -> Option<Rc<RefCell<&Tree<Vec<f32>, f32, VecDataset<Vec<f32>, f32, f32>>>>> {
    //     if let Some(cakes) = self.cakes() {
    //         let cakes = cakes.clone().borrow();
    //         let trees = cakes.trees();
    //         return Some(Rc::new(RefCell::new(trees.first().unwrap().clone())));
    //         // return Some(Rc::new(RefCell::new(cakes.borrow().trees().first().map(|x| *x).unwrap())));
    //     } else {
    //         return None;
    //     }
    // }

    // pub fn data(&self) -> Option<Rc<RefCell<&DataSet>>> {
    //     return if let Some(c) = &self.cakes {
    //         let c = c.borrow();
    //         Some(Rc::new(RefCell::new(c.trees().first().unwrap().data())))
    //     } else {
    //         None
    //     };
    // }
    // pub fn root(&'a self) -> Option<&'a Clusterf32> {
    //     return if let Some(c) = self.cakes() {
    //         let c = c.borrow();
    //         let tree = c.trees().first().unwrap();
    //         Some(tree.root())
    //     } else {
    //         None
    //     };
    // }

    pub fn labels(&self) -> Option<&Vec<u8>> {
        return if let Some(labels) = &self.labels {
            Some(&labels)
        } else {
            None
        };
    }

    pub fn set_cakes(&mut self, cakes: Cakes<Vec<f32>, f32, DataSet>) {
        self.cakes = Some(Rc::new(RefCell::new(cakes)));
    }

    pub fn cakes(&self) -> Option<Rc<RefCell<Cakes<Vec<f32>, f32, DataSet>>>> {
        return self.cakes.clone();
    }

    pub fn new(
        data_name: &str,
        cardinality: usize,
        distance_metric: DistanceMetric,
        is_expensive: bool,
    ) -> Result<Self, FFIError> {
        let criteria = PartitionCriteria::new(true).with_min_cardinality(cardinality);
        match Self::create_dataset(data_name, distance_metric, is_expensive) {
            Ok((dataset, labels)) => {
                // let cakes = Cakes::new(dataset, Some(1), &criteria);
                // let selected_clusters = select_clusters(cakes.trees().first().unwrap().root());
                // let edges = detect_edges(&selected_clusters, cakes.trees().first().unwrap().data());
                // let mut edges_ref = EdgeSet::new();
                // for edge in &edges {
                //     edges_ref.insert(edge);
                // }
                // let graph = Graph::new(selected_clusters.clone(), edges_ref.clone()).unwrap();
                return Ok(Handle {
                    cakes: Some(Rc::new(RefCell::new(Cakes::new(
                        dataset,
                        Some(1),
                        &criteria,
                    )))), //.build(&criteria)),
                    labels: Some(labels),
                    graph: None,
                    clam_graph: None,
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

    pub fn init_clam_graph(&mut self) {
        if let Some(cakes) = &self.cakes {
            let c2 = cakes.clone();
            let cakes = cakes.borrow();
            if let Some(tree) = cakes.trees().first() {
                let selected_clusters = my_select_clusters(tree.root());
                let selected_clusters2 = select_clusters(tree.root());
                let edges = detect_edges(&selected_clusters2, tree.data());
                let edges = edges
                    .iter()
                    .map(|e| MyEdge::from_edge(e))
                    .collect::<MyEdgeSet<f32>>();
                let graph = Graph::new(c2.clone(), selected_clusters.clone(), edges.clone());
                self.clam_graph = Some(Rc::new(RefCell::new(graph.unwrap())));
            }
        }
    }
    pub fn load(data_name: &str) -> Result<Self, FFIError> {
        let c = Cakes::<Vec<f32>, f32, VecDataset<_, _, _>>::load(
            Path::new(data_name),
            utils::distances::euclidean,
            false,
        );
        match c {
            Ok(cakes) => {
                return Ok(Handle {
                    cakes: Some(Rc::new(RefCell::new(cakes))),
                    labels: None,
                    graph: None,
                    clam_graph: None,
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

    pub fn load_struct(data: &TreeStartupDataFFI) -> Result<Self, FFIError> {
        let data_name = match data.data_name.as_string() {
            Ok(data_name) => data_name,
            Err(e) => {
                debug!("{:?}", e);
                return Err(FFIError::InvalidStringPassed);
            }
        };

        let metric = match utils::distances::from_enum(data.distance_metric) {
            Ok(metric) => metric,
            Err(e) => {
                debug!("{:?}", e);
                return Err(e);
            }
        };
        let c = Cakes::<Vec<f32>, f32, VecDataset<_, _, _>>::load(
            Path::new(&data_name),
            metric,
            data.is_expensive,
        );
        match c {
            Ok(cakes) => {
                return Ok(Handle {
                    cakes: Some(Rc::new(RefCell::new(cakes))),
                    labels: None,
                    graph: None,
                    clam_graph: None,
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
        data_name: &str,
        distance_metric: DistanceMetric,
        is_expensive: bool,
        // distance_metric: fn(&Vec<f32>, &Vec<f32>) -> f32,
    ) -> Result<(DataSet, Vec<u8>), FFIError> {
        let metric = match utils::distances::from_enum(distance_metric) {
            Ok(metric) => metric,
            Err(e) => {
                debug!("{:?}", e);
                return Err(e);
            }
        };
        match anomaly_readers::read_anomaly_data(data_name, false) {
            Ok((first_data, labels)) => {
                let dataset = VecDataset::new(
                    data_name.to_string(),
                    first_data,
                    metric,
                    is_expensive,
                    None,
                );

                Ok((dataset, labels))
            }
            Err(e) => {
                debug!("{:?}", e);
                Err(e)
            }
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

    // pub unsafe fn color_by_dist_to_query(
    //     &self,
    //     id_arr: &[String],
    //     node_visitor: CBFnNodeVisitor,
    // ) -> FFIError {
    //     for id in id_arr {
    //         match self.get_cluster(id.clone()) {
    //             Ok(cluster) => {
    //                 if let Some(query) = &self.current_query {
    //                     let mut baton_data = ClusterDataWrapper::from_cluster(cluster);
    //
    //                     baton_data.data_mut().dist_to_query =
    //                         cluster.distance_to_instance(self.data().unwrap(), query);
    //
    //                     node_visitor(Some(baton_data.data()));
    //                 } else {
    //                     return FFIError::QueryIsNull;
    //                 }
    //             }
    //             Err(e) => {
    //                 return e;
    //             }
    //         }
    //     }
    //     return FFIError::Ok;
    // }

    pub unsafe fn for_each_dft(
        &self,
        node_visitor: CBFnNodeVisitor,
        start_node: String,
        max_depth: i32,
    ) -> FFIError {
        return if let Some(c) = self.cakes() {
            // let cakes = c.borrow();
            // let tree = c.clone().borrow().trees().first().unwrap();

            if start_node == "root" {
                // Self::for_each_dft_helper(tree.root(), node_visitor, max_depth);
                if let Some(tree) = c.borrow().trees().first() {
                    for cluster in tree.root().subtree() {
                        let baton = ClusterDataWrapper::from_cluster(cluster);
                        node_visitor(Some(baton.data()));
                    }

                    return FFIError::Ok;
                } else {
                    return FFIError::HandleInitFailed;
                }

                // if let Some(node) = self.root() {
                //     Self::for_each_dft_helper(&node, node_visitor, max_depth);
                //     FFIError::Ok
                // } else {
                //     FFIError::HandleInitFailed
                // }
            } else {
                let id = Self::parse_cluster_id(start_node);
                if let Ok((offset, cardinality)) = id {
                    if let Some(tree) = c.borrow().trees().first() {
                        if let Some(cluster) = tree.get_cluster(offset, cardinality) {
                            Self::for_each_dft_helper(cluster, node_visitor, max_depth);
                            return FFIError::Ok;
                        } else {
                            return FFIError::InvalidStringPassed;
                        }
                    } else {
                        return FFIError::HandleInitFailed;
                    }
                } else {
                    return FFIError::InvalidStringPassed;
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
        if let Some(cakes) = self.cakes() {
            let cakes = cakes.borrow();
            if let Some(tree) = cakes.trees().first() {
                let subtree = tree.root().subtree();
                for cluster in subtree {
                    let baton = ClusterIDsWrapper::from_cluster(cluster);
                    node_visitor(Some(baton.data()));
                }
            }
        }

        return FFIError::Ok;
    }

    fn set_names_helper(root: &Clusterf32, node_visitor: crate::CBFnNameSetter) {
        if root.is_leaf() {
            let baton = ClusterIDsWrapper::from_cluster(&root);

            node_visitor(Some(baton.data()));
            return;
        }
        if let Some([left, right]) = root.children() {
            let baton = ClusterIDsWrapper::from_cluster(&root);

            node_visitor(Some(baton.data()));
            // baton.free_ids();

            Self::set_names_helper(left, node_visitor);
            Self::set_names_helper(right, node_visitor);
        }
    }
    fn for_each_dft_helper(root: &Clusterf32, node_visitor: CBFnNodeVisitor, max_depth: i32) {
        if root.is_leaf() || root.depth() as i32 >= max_depth {
            let baton = ClusterDataWrapper::from_cluster(&root);

            node_visitor(Some(baton.data()));
            return;
        }
        if let Some([left, right]) = root.children() {
            let baton = ClusterDataWrapper::from_cluster(&root);

            node_visitor(Some(&baton.data()));

            Self::for_each_dft_helper(left, node_visitor, max_depth);
            Self::for_each_dft_helper(right, node_visitor, max_depth);
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
            let cakes = cakes.borrow();
            if let Some(tree) = cakes.trees().first() {
                tree.cardinality() as i32
            } else {
                0
            }
            // self.get_tree().unwrap().root.num_descendants() as i32
        } else {
            0
        }
    }

    pub fn tree_height(&self) -> i32 {
        if let Some(cakes) = &self.cakes {
            cakes.borrow().trees().first().unwrap().depth() as i32
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
    pub fn parse_cluster_id(cluster_id: String) -> Result<(usize, usize), FFIError> {
        let mut parts = cluster_id.split('-');

        if let (Some(offset_str), Some(cardinality_str)) = (parts.next(), parts.next()) {
            if let (Ok(offset), Ok(cardinality)) = (
                offset_str.parse::<usize>(),
                cardinality_str.parse::<usize>(),
            ) {
                return Ok((offset, cardinality));
            }
        }
        return Err(FFIError::InvalidStringPassed);
    }
    // why isnt string taken by reference?
    // pub unsafe fn get_cluster(&'a self, cluster_id: String) -> Result<&'a Clusterf32, FFIError> {
    //     if let Some(c) = self.cakes() {
    //         let mut parts = cluster_id.split('-');
    //
    //         if let (Some(offset_str), Some(cardinality_str)) = (parts.next(), parts.next()) {
    //             if let (Ok(offset), Ok(cardinality)) = (
    //                 offset_str.parse::<usize>(),
    //                 cardinality_str.parse::<usize>(),
    //             ) {
    //                 println!("Offset: {}", offset);
    //                 println!("Cardinality: {}", cardinality);
    //                 if let Some(tree) = c.borrow().trees().first() {
    //                     if let Some(cluster) = tree.get_cluster(offset, cardinality) {
    //                         return Ok(cluster);
    //                     } else {
    //                         return Err(FFIError::InvalidStringPassed);
    //                     }
    //                 }
    //             }
    //         }
    //     }
    //     debug!("root not built");
    //     return Err(FFIError::HandleInitFailed);
    // }

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
            let cakes = cakes.borrow();
            if let Some(tree) = cakes.trees().first() {
                reingold_tilford::run(tree.root(), &self.labels, tree.depth() as i32, node_visitor)
            } else {
                FFIError::NullPointerPassed
            }
            // let root = cakes.trees().first().unwrap().root();
            // reingold_tilford::run(tree.root(), &self.labels, tree.depth() as i32, node_visitor)
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
        return if let Some(c) = &self.cakes {
            let cakes = c.borrow();
            if let Some(tree) = cakes.trees().first() {
                let id = tree.root().name();
                let id = Self::parse_cluster_id(id.to_string());
                if let Ok((offset, cardinality)) = id {
                    if let Some(cluster) = tree.get_cluster(offset, cardinality) {
                        reingold_tilford::run_offset(
                            &root.pos,
                            cluster,
                            // &self.labels,
                            // current_depth,
                            max_depth,
                            node_visitor,
                        )
                    } else {
                        FFIError::NullPointerPassed
                    }
                } else {
                    FFIError::InvalidStringPassed
                }
            } else {
                FFIError::NullPointerPassed
            }
        } else {
            FFIError::HandleInitFailed
        };
    }
}
