
#pragma warning disable CS8500
#pragma warning disable CS8981
using Clam;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Clam
{
    namespace FFI
    {
        public unsafe delegate void NodeVisitor(ref Clam.FFI.ClusterData baton);

        public static partial class NativeMethods
        {


            public const string __DllName = "clam_ffi_20231003155445";
            private static IntPtr m_Handle;

            [DllImport(__DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "free_string")]
            private unsafe static extern void free_string(IntPtr context, IntPtr data);


            [DllImport(__DllName, EntryPoint = "for_each_dft", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern FFIError for_each_dft(IntPtr ptr, NodeVisitor callback, string startNode);

            public static FFIError ForEachDFT(NodeVisitor callback, string startNode = "root")
            {
                return for_each_dft(m_Handle, callback, startNode);
            }

            [DllImport(__DllName, EntryPoint = "get_cluster_data", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static unsafe extern FFIError get_cluster_data(IntPtr handle, ref global::Clam.FFI.ClusterData inNode, out global::Clam.FFI.ClusterData outNode);

            public static unsafe FFIError GetClusterData(global::Clam.FFI.ClusterWrapper nodeWrapper)
            {
                ClusterData nodeData = nodeWrapper.Data;

                FFIError found = get_cluster_data(m_Handle, ref nodeData, out var outNode);
                if (found == FFIError.Ok)
                {
                    nodeWrapper.Data = outNode;
                }
                return found;
            }

            //[DllImport(__DllName, EntryPoint = "get_root_data", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            //private static unsafe extern FFIError get_root_data(IntPtr handle, ref global::Clam.NodeDataFFI inNode, out global::Clam.NodeDataFFI outNode);

            //public static unsafe FFIError GetClamRootData(global::Clam.NodeWrapper nodeWrapper)
            //{
            //    NodeDataFFI nodeData = nodeWrapper.Data;

            //    FFIError found = get_root_data(_handle, ref nodeData, out var outNode);
            //    if (found == FFIError.Ok)
            //    {
            //        nodeWrapper.Data = outNode;
            //    }
            //    return found;
            //}

            [DllImport(__DllName, EntryPoint = "distance_to_other", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static unsafe extern float distance_to_other(IntPtr handle, string node1, string node2);

            public static unsafe float DistanceToOther(string node1, string node2)
            {
                //NodeDataFFI nodeData = nodeWrapper.Data;

                //FFIError found = get_cluster_data(_handle, ref nodeData, out var outNode);
                //if (found == FFIError.Ok)
                //{
                //    nodeWrapper.Data = outNode;
                //}
                return distance_to_other(m_Handle, node1, node2); ;
            }

            [DllImport(__DllName, EntryPoint = "draw_heirarchy", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern FFIError draw_heirarchy(IntPtr ptr, NodeVisitor callback);

            public static FFIError DrawHeirarchy(NodeVisitor callback)
            {
                return draw_heirarchy(m_Handle, callback);
            }

            [DllImport(__DllName, EntryPoint = "draw_heirarchy_offset_from", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern FFIError draw_heirarchy_offset_from(IntPtr ptr, ref ClusterData offsetPos, NodeVisitor callback);
            public static FFIError DrawHeirarchyOffsetFrom(ClusterWrapper wrapper, NodeVisitor callback)
            {

                //FFIError found = get_cluster_data(_handle, ref nodeData, out var outNode);
                //if (found == FFIError.Ok)
                //{
                //    nodeWrapper.Data = outNode;
                //}
                ClusterData nodeData = wrapper.Data;

                return draw_heirarchy_offset_from(m_Handle, ref nodeData, callback);
            }

            [DllImport(__DllName, EntryPoint = "get_num_nodes", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int get_num_nodes(IntPtr handle);

            public static int GetNumNodes()
            {
                return get_num_nodes(m_Handle);
            }

            [DllImport(__DllName, EntryPoint = "cardinality", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int cardinality(IntPtr handle);

            public static int Cardinality()
            {
                return cardinality(m_Handle);
            }

            [DllImport(__DllName, EntryPoint = "tree_height", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int tree_height(IntPtr handle);

            public static int TreeHeight()
            {
                return tree_height(m_Handle);
            }

            [DllImport(__DllName, EntryPoint = "radius", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern double radius(IntPtr handle);

            public static double Radius()
            {
                return radius(m_Handle);
            }

            [DllImport(__DllName, EntryPoint = "lfd", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern double lfd(IntPtr handle);

            public static double LFD()
            {
                return lfd(m_Handle);
            }

            [DllImport(__DllName, EntryPoint = "arg_center", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int arg_center(IntPtr handle);

            public static int ArgCenter()
            {
                return arg_center(m_Handle);
            }

            [DllImport(__DllName, EntryPoint = "arg_radius", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int arg_radius(IntPtr handle);

            public static int ArgRadius()
            {
                return arg_radius(m_Handle);
            }



            [DllImport(__DllName, EntryPoint = "shutdown_physics", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern FFIError shutdown_physics(IntPtr handle);

            public static FFIError ShutdownPhysics()
            {
                return shutdown_physics(m_Handle);
            }

            [DllImport(__DllName, EntryPoint = "test_cakes_rnn_query", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int test_cakes_rnn_query(IntPtr handle, float searchRadius, NodeVisitor callback);

            public static int TestCakesRNNQuery(float searchRadius, NodeVisitor callback)
            {
                return test_cakes_rnn_query(m_Handle, searchRadius, callback);
            }

            [DllImport(__DllName, EntryPoint = "init_clam", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern FFIError init_clam(out IntPtr ptr, byte[] data_name, int name_len, uint cardinality);

            public static FFIError InitClam(string dataName, uint cardinality)
            {
                byte[] byteName = Encoding.UTF8.GetBytes(dataName);
                int len = byteName.Length;

                return init_clam(out m_Handle, byteName, len, cardinality);
            }



            [DllImport(__DllName, EntryPoint = "shutdown_clam", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern FFIError shutdown_clam(out IntPtr ptr);

            public static FFIError ShutdownClam()
            {
                if (m_Handle != IntPtr.Zero)
                {
                    return shutdown_clam(out m_Handle);
                }

                Debug.Log("error clam not initialized");
                //Debug.Log("Application ending after " + Time.time + " seconds");

                return FFIError.NullPointerPassed;
            }

            public unsafe static void FreeString(IntPtr data)
            {
                free_string(m_Handle, data);
            }

            [DllImport(__DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "free_string_ffi")]
            public unsafe static extern void free_string_ffi(ref global::Clam.FFI.StringFFI inNode, out global::Clam.FFI.StringFFI outNode);

            public unsafe static void FreeStringFFI(ref global::Clam.FFI.StringFFI inNode)
            {
                free_string_ffi(ref inNode, out var outNode);
            }

            [System.Security.SecurityCritical]
            [DllImport(__DllName, EntryPoint = "test_struct_array", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static unsafe extern void test_struct_array(IntPtr handle, [In, Out] ClusterData[] arr, int len);
            struct float1
            {
                public float f;
            }
            public static unsafe void TestStructArray()
            {
                int numItems = 5;
                ClusterData[] items = new ClusterData[numItems];
                for (int i = 0; i < numItems; i++)
                {
                    items[i] = new ClusterData(i);
                }

                test_struct_array(m_Handle, items, items.Length);
            }

            //[System.Security.SecurityCritical]
            //[DllImport(__DllName, EntryPoint = "color_by_dist_to_query", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            //private static unsafe extern void color_by_dist_to_query(IntPtr handle, [In, Out] NodeDataFFI[] arr, int len, NodeVisitor cb_fn);
            //public static unsafe void ColorByDistToQuery(List<NodeDataUnity> nodes, NodeVisitor cbFn)
            //{
            //    NodeDataFFI[] items = new NodeDataFFI[nodes.Count];
            //    for (int i = 0; i < nodes.Count; i++)
            //    {
            //        //var id = nodes[i].id;
            //        items[i] = new NodeDataFFI(nodes[i]);
            //    }

            //    color_by_dist_to_query(_handle, items, items.Length, cbFn);



            //}

            //[System.Security.SecurityCritical]
            //[DllImport(__DllName, EntryPoint = "detect_edges", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            //private static unsafe extern void detect_edges(IntPtr handle, [In, Out] NodeDataFFI[] arr, int len, NodeVisitor cb_fn);
            //public static unsafe void DetectEdges(List<NodeDataUnity> nodes, NodeVisitor cbFn)
            //{
            //    NodeDataFFI[] items = new NodeDataFFI[nodes.Count];
            //    for (int i = 0; i < nodes.Count; i++)
            //    {
            //        //var id = nodes[i].id;
            //        items[i] = new NodeDataFFI(nodes[i]);
            //    }

            //    detect_edges(_handle, items, items.Length, cbFn);
            //}

            //[System.Security.SecurityCritical]
            //[DllImport(__DllName, EntryPoint = "init_force_directed_sim", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            //private static unsafe extern void init_force_directed_sim(IntPtr handle, [In, Out] NodeDataFFI[] arr, int len, NodeVisitor cb_fn);
            //public static unsafe void InitForceDirectedSim(List<NodeDataUnity> nodes, NodeVisitor cbFn)
            //{
            //    NodeDataFFI[] items = new NodeDataFFI[nodes.Count];
            //    for (int i = 0; i < nodes.Count; i++)
            //    {
            //        //var id = nodes[i].id;
            //        items[i] = new NodeDataFFI(nodes[i]);
            //    }

            //    init_force_directed_sim(_handle, items, items.Length, cbFn);

            //}

            [System.Security.SecurityCritical]
            [DllImport(__DllName, EntryPoint = "launch_physics_thread", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static unsafe extern void launch_physics_thread(IntPtr handle, [In, Out] ClusterData[] arr, int len, float scalar, int maxIters, NodeVisitor edge_cb, NodeVisitor update_cb);
            public static unsafe void LaunchPhysicsThread(ClusterData[] nodes, float scalar, int maxIters, NodeVisitor edgeCB, NodeVisitor updateCB)
            {
                //NodeDataFFI[] items = new NodeDataFFI[nodes.Count];
                //for (int i = 0; i < nodes.Count; i++)
                //{
                //    //var id = nodes[i].id;
                //    items[i] = new NodeDataFFI(nodes[i]);
                //}

                launch_physics_thread(m_Handle, nodes, nodes.Length, scalar, maxIters, edgeCB, updateCB);
            }


            [System.Security.SecurityCritical]
            [DllImport(__DllName, EntryPoint = "run_force_directed_graph_sim", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static unsafe extern void run_force_directed_graph_sim(IntPtr handle, [In, Out] ClusterData[] arr, int len, float scalar, int maxIters, NodeVisitor edge_cb);
            public static unsafe void RunForceDirectedSim(ClusterData[] nodes, float scalar, int maxIters, NodeVisitor edgeCB)
            {
                //NodeDataFFI[] items = new NodeDataFFI[nodes.Count];
                //for (int i = 0; i < nodes.Count; i++)
                //{
                //    //var id = nodes[i].id;
                //    items[i] = new NodeDataFFI(nodes[i]);
                //}

                run_force_directed_graph_sim(m_Handle, nodes, nodes.Length, scalar, maxIters, edgeCB);
            }

            [System.Security.SecurityCritical]
            [DllImport(__DllName, EntryPoint = "physics_update_async", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static unsafe extern FFIError physics_update_async(IntPtr handle, NodeVisitor cb_fn);
            public static unsafe FFIError PhysicsUpdateAsync(NodeVisitor cb_fn)
            {
                return physics_update_async(m_Handle, cb_fn);
            }

            [System.Security.SecurityCritical]
            [DllImport(__DllName, EntryPoint = "apply_forces", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static unsafe extern void apply_forces(IntPtr handle, float edgeScalar, NodeVisitor cb_fn);
            public static unsafe void ApplyForces(float edgeScalar, NodeVisitor cbFn)
            {
                apply_forces(m_Handle, edgeScalar, cbFn);
            }




        }
    }

}
