using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


namespace Clam
{
    namespace FFI
    {
        public static partial class NativeMethods
        {
            // ------------------------------------- Startup/Shutdown -------------------------------------

            [DllImport(__DllName, EntryPoint = "init_clam", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern FFIError init_clam(out IntPtr ptr, byte[] data_name, int name_len, uint cardinality);

            [DllImport(__DllName, EntryPoint = "shutdown_clam", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern FFIError shutdown_clam(out IntPtr ptr);

            // -------------------------------------  Tree helpers ------------------------------------- 

            [DllImport(__DllName, EntryPoint = "for_each_dft", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern FFIError for_each_dft(IntPtr ptr, NodeVisitor callback, string startNode);

            [DllImport(__DllName, EntryPoint = "set_names", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern FFIError set_names(IntPtr ptr, NameSetter callback, string startNode);

            [DllImport(__DllName, EntryPoint = "tree_height", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int tree_height(IntPtr handle);

            [DllImport(__DllName, EntryPoint = "color_clusters_by_label", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern FFIError color_clusters_by_label(IntPtr ptr, NodeVisitor callback);

            // ------------------------------------- Cluster Helpers ------------------------------------- 

            //[DllImport(__DllName, EntryPoint = "get_cluster_data", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            //private static unsafe extern FFIError get_cluster_data(IntPtr handle, ref global::Clam.FFI.ClusterData inNode, out global::Clam.FFI.ClusterData outNode);

            [DllImport(__DllName, EntryPoint = "get_root_data", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int get_root_data(IntPtr handle);
            [DllImport(__DllName, EntryPoint = "create_cluster_data", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern FFIError create_cluster_data(IntPtr ptr, string id, out ClusterData data);

            [DllImport(__DllName, EntryPoint = "create_cluster_ids", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern FFIError create_cluster_ids(IntPtr ptr, string id, out ClusterIDs data);

            [DllImport(__DllName, EntryPoint = "delete_cluster_data", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern FFIError delete_cluster_data(ref ClusterData inData, out ClusterData outData);
            [DllImport(__DllName, EntryPoint = "delete_cluster_ids", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern FFIError delete_cluster_ids(ref ClusterIDs inData, out ClusterIDs outData);

            [DllImport(__DllName, EntryPoint = "set_message", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern FFIError set_message(string msg, out ClusterData outData);

            [DllImport(__DllName, EntryPoint = "distance_to_other", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static unsafe extern float distance_to_other(IntPtr handle, string node1, string node2);

            // ------------------------------------- Reingold Tilford Tree Layout -------------------------------------

            [DllImport(__DllName, EntryPoint = "draw_heirarchy", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern FFIError draw_heirarchy(IntPtr ptr, NodeVisitor callback);

            [DllImport(__DllName, EntryPoint = "draw_heirarchy_offset_from", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern FFIError draw_heirarchy_offset_from(IntPtr ptr, ref ClusterData offsetPos, NodeVisitor callback);

            // ------------------------------------- Graph Physics -------------------------------------
            [System.Security.SecurityCritical]
            [DllImport(__DllName, EntryPoint = "init_force_directed_graph", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static unsafe extern void init_force_directed_graph(IntPtr handle, [In, Out] ClusterData[] arr, int len, float scalar, int maxIters);

            [System.Security.SecurityCritical]
            [DllImport(__DllName, EntryPoint = "init_graph_vertices", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static unsafe extern void init_graph_vertices(IntPtr handle, NodeVisitorMut edge_cb);

            [System.Security.SecurityCritical]
            [DllImport(__DllName, EntryPoint = "physics_update_async", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static unsafe extern FFIError physics_update_async(IntPtr handle, NodeVisitor cb_fn);

            [DllImport(__DllName, EntryPoint = "shutdown_physics", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern FFIError shutdown_physics(IntPtr handle);

            [DllImport(__DllName, EntryPoint = "force_physics_shutdown", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern FFIError force_physics_shutdown(IntPtr handle);

            [DllImport(__DllName, EntryPoint = "get_num_edges_in_graph", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int get_num_edges_in_graph(IntPtr ptr);

            // ------------------------------------- RNN Search -------------------------------------

            [DllImport(__DllName, EntryPoint = "test_cakes_rnn_query", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int test_cakes_rnn_query(IntPtr handle, float searchRadius, NodeVisitor callback);

        }

    }
}
