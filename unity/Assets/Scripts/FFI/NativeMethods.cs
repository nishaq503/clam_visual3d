
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
	public const string __DllName = "clam_ffi_20231007164536";
            private static IntPtr m_Handle;

            private static bool m_Initialized = false;


            // init/shutdown functions for clam
            public static FFIError InitClam(string dataName, uint cardinality)
            {
                byte[] byteName = Encoding.UTF8.GetBytes(dataName);
                int len = byteName.Length;
                var e = init_clam(out m_Handle, byteName, len, cardinality);
                if (e == FFIError.Ok)
                {
                    m_Initialized = true;
                }
                return e;
            }

            public static FFIError ShutdownClam()
            {
                if (m_Handle != IntPtr.Zero && m_Initialized)
                {
                    var e = shutdown_clam(out m_Handle);
                    m_Initialized = false;
                    m_Handle = IntPtr.Zero;
                    return e;
                }
                
                return FFIError.NullPointerPassed;
            }

            // -------------------------------------  Tree helpers ------------------------------------- 

            public static FFIError ForEachDFT(NodeVisitor callback, string startNode = "root")
            {
                return for_each_dft(m_Handle, callback, startNode);
            }

            //public static int GetNumNodes()
            //{
            //    return get_num_nodes(m_Handle);
            //}

            public static int TreeHeight()
            {
                return tree_height(m_Handle);
            }

            // ------------------------------------- Cluster Helpers ------------------------------------- 

            public static unsafe FFIError GetClusterData(global::Clam.FFI.ClusterDataWrapper nodeWrapper)
            {
                ClusterData nodeData = nodeWrapper.Data;

                FFIError found = get_cluster_data(m_Handle, ref nodeData, out var outNode);
                if (found == FFIError.Ok)
                {
                    // does reassigning data lead to memory leak?
                    // pretty sure it is resassigning to itself - in rust, outNode is set equal
                    // to nodeData meaning outNode copies it?
                    nodeWrapper.Data = outNode;
                }
                return found;
            }

            public static bool GetRootData(out ClusterDataWrapper clusterDataWrapper)
            {
                if(MenuEventManager.instance.GetTree().TryGetValue("1", out var root))
                {
                    clusterDataWrapper = new ClusterDataWrapper(root);

                    ClusterData clusterData = clusterDataWrapper.Data;

                    FFIError found = get_cluster_data(m_Handle, ref clusterData, out var outNode);
                    if (found == FFIError.Ok)
                    {
                        clusterDataWrapper.Data = outNode;
                        return true;
                    }
                    else
                    {
                        clusterDataWrapper = null;
                        return false;
                    }
                }
                clusterDataWrapper = null;
                return false;

                
            }

            public static unsafe float DistanceToOther(string node1, string node2)
            {
                return distance_to_other(m_Handle, node1, node2); ;
            }

           

            // Reingold Tilford Tree Layout
            public static FFIError DrawHeirarchy(NodeVisitor callback)
            {
                return draw_heirarchy(m_Handle, callback);
            }

            public static FFIError DrawHeirarchyOffsetFrom(ClusterDataWrapper wrapper, NodeVisitor callback)
            {
                ClusterData nodeData = wrapper.Data;
                return draw_heirarchy_offset_from(m_Handle, ref nodeData, callback);
            }

            // Graph Physics
            public static unsafe void RunForceDirectedSim(ClusterData[] nodes, float scalar, int maxIters, NodeVisitor edgeCB)
            {
                run_force_directed_graph_sim(m_Handle, nodes, nodes.Length, scalar, maxIters, edgeCB);
            }

            public static unsafe FFIError PhysicsUpdateAsync(NodeVisitor cb_fn)
            {
                return physics_update_async(m_Handle, cb_fn);
            }

            public static FFIError ShutdownPhysics()
            {
                return shutdown_physics(m_Handle);
            }

            // RNN 
            public static int TestCakesRNNQuery(float searchRadius, NodeVisitor callback)
            {
                return test_cakes_rnn_query(m_Handle, searchRadius, callback);
            }
        }
    }

}
