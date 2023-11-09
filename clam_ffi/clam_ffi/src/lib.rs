//need to use partial struct to pass by reference and access sub structs
// pass by pointer with classes but cant seem to access sub structs

use std::{ffi::c_char, ptr::null_mut};
mod ffi_impl;
mod graph;
mod handle;
mod tests;
mod tree_layout;
mod utils;
// use abd_clam::utils;
// use abd_clam::utils::helpers;
use ffi_impl::{
    cluster_data::{self, ClusterData},
    cluster_ids::ClusterIDs,
    lib_impl::{
        color_by_dist_to_query_impl, distance_to_other_impl, for_each_dft_impl, set_names_impl,
        tree_height_impl,
    },
    string_ffi::StringFFI,
};
use graph::entry::{
    get_num_edges_in_graph_impl, init_force_directed_graph_impl, init_graph_vertices_impl,
    physics_update_async_impl, shutdown_physics_impl,
};
use tree_layout::entry_point::{draw_heirarchy_impl, draw_heirarchy_offset_from_impl};
use utils::{
    debug,
    distances::DistanceMetric,
    error::FFIError,
    // helpers,
    types::{InHandlePtr, OutHandlePtr},
};

use crate::handle::entry_point::{init_clam_impl, shutdown_clam_impl};

type CBFnNodeVisitor = extern "C" fn(Option<&ClusterData>) -> ();
type CBFnNameSetter = extern "C" fn(Option<&ClusterIDs>) -> ();
type CBFnNodeVisitorMut = extern "C" fn(Option<&mut ClusterData>) -> ();

#[no_mangle]
pub unsafe extern "C" fn create_cluster_data(
    ptr: InHandlePtr,
    id: *const c_char,
    outgoing: Option<&mut ClusterData>,
) -> FFIError {
    if let Some(handle) = ptr {
        let outgoing = outgoing.unwrap();
        let id = utils::helpers::c_char_to_string(id);
        // let data = Box::new(ClusterData::default());

        // match out_node.id.as_string() {
        match handle.find_node(id) {
            Ok(cluster) => {
                let cluster_data = ClusterData::from_clam(cluster);

                *outgoing = cluster_data;
                return FFIError::Ok;
            }
            Err(e) => {
                return FFIError::InvalidStringPassed;
            }
        }
    }
    return FFIError::NullPointerPassed;
}

#[no_mangle]
pub extern "C" fn delete_cluster_data(
    in_cluster_data: Option<&ClusterData>,
    out_cluster_data: Option<&mut ClusterData>,
) -> FFIError {
    // if data.is_none() {
    // }

    if let Some(in_data) = in_cluster_data {
        if let Some(out_data) = out_cluster_data {
            *out_data = *in_data;
            out_data.free_ids();
            return FFIError::Ok;
        } else {
            return FFIError::NullPointerPassed;
        }
    } else {
        return FFIError::NullPointerPassed;
    }
}

#[no_mangle]
pub unsafe extern "C" fn create_cluster_ids(
    ptr: InHandlePtr,
    id: *const c_char,
    outgoing: Option<&mut ClusterIDs>,
) -> FFIError {
    if let Some(handle) = ptr {
        let outgoing = outgoing.unwrap();
        let id = utils::helpers::c_char_to_string(id);
        // let data = Box::new(ClusterData::default());

        // match out_node.id.as_string() {
        match handle.find_node(id) {
            Ok(cluster) => {
                let cluster_data = ClusterIDs::from_clam(cluster);

                *outgoing = cluster_data;
                return FFIError::Ok;
            }
            Err(e) => {
                return FFIError::InvalidStringPassed;
            }
        }
    }
    return FFIError::NullPointerPassed;
}

#[no_mangle]
pub extern "C" fn delete_cluster_ids(
    in_cluster_data: Option<&ClusterIDs>,
    out_cluster_data: Option<&mut ClusterIDs>,
) -> FFIError {
    // if data.is_none() {
    // }

    if let Some(in_data) = in_cluster_data {
        if let Some(out_data) = out_cluster_data {
            *out_data = *in_data;
            out_data.free_ids();
            return FFIError::Ok;
        } else {
            return FFIError::NullPointerPassed;
        }
    } else {
        return FFIError::NullPointerPassed;
    }
}

#[no_mangle]
pub unsafe extern "C" fn set_message(
    msg: *const c_char,
    // in_cluster_data: Option<&ClusterData>,
    out_cluster_data: Option<&mut ClusterData>,
) -> FFIError {
    // if data.is_none() {
    // }

    // if let Some(in_data) = in_cluster_data {
    if let Some(out_data) = out_cluster_data {
        // *out_data = *in_data;
        // out_data.free_ids();
        let msg_str = StringFFI::c_char_to_string(msg);

        out_data.set_message(msg_str);
        return FFIError::Ok;
    } else {
        return FFIError::NullPointerPassed;
    }
    // } else {
    //     return FFIError::NullPointerPassed;
    // }

    // let ctx = data.unwrap();

    // {
    //     unsafe { drop(Box::from_raw(*ctx)) };
    // }

    // *ctx = null_mut();
}

#[repr(C)]
pub struct Context {
    pub foo: bool,
    pub bar: i32,
    pub baz: u64,
}

// ------------------------------------- Startup/Shutdown -------------------------------------

#[no_mangle]
pub unsafe extern "C" fn init_clam(
    ptr: OutHandlePtr,
    data_name: *const u8,
    name_len: i32,
    cardinality: u32,
    distance_metric: DistanceMetric,
) -> FFIError {
    return init_clam_impl(ptr, data_name, name_len, cardinality, distance_metric);
}

#[no_mangle]
pub unsafe extern "C" fn shutdown_clam(context_ptr: OutHandlePtr) -> FFIError {
    return shutdown_clam_impl(context_ptr);
}

// -------------------------------------  Tree helpers -------------------------------------

#[no_mangle]
pub unsafe extern "C" fn for_each_dft(
    ptr: InHandlePtr,
    node_visitor: CBFnNodeVisitor,
    start_node: *const c_char,
) -> FFIError {
    return for_each_dft_impl(ptr, node_visitor, start_node);
}

#[no_mangle]
pub unsafe extern "C" fn set_names(
    ptr: InHandlePtr,
    node_visitor: CBFnNameSetter,
    start_node: *const c_char,
) -> FFIError {
    return set_names_impl(ptr, node_visitor, start_node);
}

#[no_mangle]
pub unsafe extern "C" fn tree_height(ptr: InHandlePtr) -> i32 {
    return tree_height_impl(ptr);
}

#[no_mangle]
// add recursive bool option and node name
pub unsafe extern "C" fn color_clusters_by_label(
    ptr: InHandlePtr,
    node_visitor: CBFnNodeVisitor,
) -> FFIError {
    return ffi_impl::lib_impl::color_clusters_by_label_impl(ptr, node_visitor);
}
// ------------------------------------- Cluster Helpers -------------------------------------

// #[no_mangle]
// pub unsafe extern "C" fn get_cluster_data(
//     context: InHandlePtr,
//     incoming: Option<&ClusterData>,
//     outgoing: Option<&mut ClusterData>,
// ) -> FFIError {
//     return get_cluster_data_impl(context, incoming, outgoing);
// }

#[no_mangle]
pub unsafe extern "C" fn distance_to_other(
    ptr: InHandlePtr,
    node_name1: *const c_char,
    node_name2: *const c_char,
) -> f32 {
    return distance_to_other_impl(ptr, node_name1, node_name2);
}

// ------------------------------------- Reingold Tilford Tree Layout -------------------------------------

#[no_mangle]
pub extern "C" fn draw_heirarchy(ptr: InHandlePtr, node_visitor: CBFnNodeVisitor) -> FFIError {
    return draw_heirarchy_impl(ptr, node_visitor);
}

#[no_mangle]
pub unsafe extern "C" fn draw_heirarchy_offset_from(
    ptr: InHandlePtr,
    root: Option<&ClusterData>,
    current_depth: i32,
    max_depth: i32,
    node_visitor: CBFnNodeVisitor,
) -> FFIError {
    return draw_heirarchy_offset_from_impl(ptr, root, current_depth, max_depth, node_visitor);
}

// ------------------------------------- Graph Physics -------------------------------------

#[no_mangle]
pub unsafe extern "C" fn init_force_directed_graph(
    context: InHandlePtr,
    arr_ptr: *mut ClusterData,
    len: i32,
    scalar: f32,
    max_iters: i32,
    // edge_detect_cb: CBFnNodeVisitorMut,
    // physics_update_cb: CBFnNodeVisitor,
) -> FFIError {
    return init_force_directed_graph_impl(
        context, arr_ptr, len, scalar, max_iters,
        // edge_detect_cb,
    );
}

#[no_mangle]
pub unsafe extern "C" fn init_graph_vertices(
    context: InHandlePtr,
    edge_detect_cb: CBFnNodeVisitorMut,
) -> FFIError {
    return init_graph_vertices_impl(context, edge_detect_cb);
}

#[no_mangle]
pub unsafe extern "C" fn physics_update_async(
    context: InHandlePtr,
    updater: CBFnNodeVisitor,
) -> FFIError {
    return physics_update_async_impl(context, updater);
}

#[no_mangle]
pub extern "C" fn shutdown_physics(ptr: InHandlePtr) -> FFIError {
    return shutdown_physics_impl(ptr);
}

#[no_mangle]
pub extern "C" fn get_num_edges_in_graph(ptr: InHandlePtr) -> i32 {
    return get_num_edges_in_graph_impl(ptr);
}

#[no_mangle]
pub unsafe extern "C" fn force_physics_shutdown(ptr: InHandlePtr) -> i32 {
    // Handle::from_ptr(ptr).get_num_nodes() + 1
    // force_physics_shutdown_impl(ptr);
    if let Some(handle) = ptr {
        // debug!("cardinality: {}", handle.tree_height() + 1);
        handle.force_physics_shutdown();
        return 0;
        // return handle.tree_height() + 1;
    }
    debug!("handle not created force physics shutdown");

    return 0;
}
// ------------------------------------- RNN Search -------------------------------------

// #[no_mangle]
// pub unsafe extern "C" fn test_cakes_rnn_query(
//     ptr: InHandlePtr,
//     search_radius: f32,
//     node_visitor: CBFnNodeVisitor,
// ) -> FFIError {
//     return test_cakes_rnn_query_impl(ptr, search_radius, node_visitor);
// }

#[no_mangle]
pub unsafe extern "C" fn color_by_dist_to_query(
    context: InHandlePtr,
    arr_ptr: *mut ClusterData,
    len: i32,
    node_visitor: CBFnNodeVisitor,
) -> FFIError {
    return color_by_dist_to_query_impl(context, arr_ptr, len, node_visitor);
}
