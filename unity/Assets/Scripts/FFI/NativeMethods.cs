
#pragma warning disable CS8500
#pragma warning disable CS8981
using Clam;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

namespace Clam
{
    namespace FFI
    {
        public unsafe delegate void NodeVisitor(ref Clam.FFI.ClusterData baton);
        public unsafe delegate void NameSetter(ref Clam.FFI.ClusterIDs baton);
        public unsafe delegate void NodeVisitorMut(ref Clam.FFI.ClusterData inData);

        public static partial class NativeMethods
        {
	public const string __DllName = "clam_ffi_20231017114716";
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
                Debug.Log("Failed to shutdown clam");
                return FFIError.NullPointerPassed;
            }

            // -------------------------------------  Tree helpers ------------------------------------- 

            public static FFIError ForEachDFT(NodeVisitor callback, string startNode = "root")
            {
                return for_each_dft(m_Handle, callback, startNode);
            }

            public static FFIError SetNames(NameSetter callback, string startNode = "root")
            {
                return set_names(m_Handle, callback, startNode);
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

            public static Clam.FFI.ClusterDataWrapper CreateClusterDataWrapper(string id)
            {
                var result = create_cluster_data(m_Handle, id, out var data);
                if (result != FFIError.Ok)
                {
                    Debug.Log(result);
                    return null;
                }
                var node = Cakes.Tree.GetTree().GetValueOrDefault(data.id.AsString).GetComponent<Node>();
                data.SetPos(node.GetPosition());
                data.SetColor(node.GetColor());

                return new ClusterDataWrapper(data);
                //ClusterData* data = create_cluster_data("1");
            }

            public static Clam.FFI.ClusterIDsWrapper CreateClusterIDsWrapper(string id)
            {
                var result = create_cluster_ids(m_Handle, id, out var data);
                if (result != FFIError.Ok)
                {
                    Debug.Log(result);
                    return null;
                }
                //var node = Cakes.Tree.GetTree().GetValueOrDefault(data.id.AsString).GetComponent<Node>();
                //data.SetPos(node.GetPosition());
                //data.SetColor(node.GetColor());

                return new ClusterIDsWrapper(data);
                //ClusterData* data = create_cluster_data("1");
            }

            public static FFIError CreateClusterDataMustFree(string id, out Clam.FFI.ClusterData clusterData)
            {
                var result = create_cluster_data(m_Handle, id, out var data);
                if (result != FFIError.Ok)
                {
                    Debug.Log(result);
                    clusterData = new ClusterData();
                    return result;
                }
                var node = Cakes.Tree.GetTree().GetValueOrDefault(data.id.AsString).GetComponent<Node>();
                data.SetPos(node.GetPosition());
                data.SetColor(node.GetColor());
                clusterData = data;
                return FFIError.Ok;
                //return new ClusterDataWrapper(data);
                //ClusterData* data = create_cluster_data("1");
            }

            public static FFIError DeleteClusterData(ref ClusterData data)
            {
                //Debug.Log("freeing with delete cluster data");
                return delete_cluster_data(ref data, out var outData);
                //return data;
                //ClusterData* data = create_cluster_data("1");
            }

            public static FFIError DeleteClusterIDs(ref ClusterIDs data)
            {
                //Debug.Log("freeing with delete cluster data");
                return delete_cluster_ids(ref data, out var outData);
                //return data;
                //ClusterData* data = create_cluster_data("1");
            }

            public static FFIError SetMessage(string msg, out ClusterData data)
            {
                //Debug.Log("freeing with delete cluster data");
                set_message(msg, out data);
                //data = outData;
                return FFIError.Ok;
                //return data;
                //ClusterData* data = create_cluster_data("1");
            }

            public static bool GetRootData(out ClusterDataWrapper clusterDataWrapper)
            {
                string rootName = "1";
                if (Cakes.Tree.GetTree().TryGetValue(rootName, out var root))
                {
                    clusterDataWrapper = CreateClusterDataWrapper(root.GetComponent<Node>().GetId());

                    if (clusterDataWrapper != null)
                    {
                        return true;
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

            public static void InitForceDirectedGraph(ClusterData[] nodes, float scalar, int maxIters)
            {
                init_force_directed_graph(m_Handle, nodes, nodes.Length, scalar, maxIters);
            }
            public static void InitGraphVertices(NodeVisitorMut edgeCB)
            {
                init_graph_vertices(m_Handle, edgeCB);
            }
           
            public static FFIError PhysicsUpdateAsync(NodeVisitor cb_fn)
            {
                return physics_update_async(m_Handle, cb_fn);
            }

            public static FFIError ShutdownPhysics()
            {
                return shutdown_physics(m_Handle);
            }
            public static FFIError ForceShutdownPhysics()
            {
                return force_physics_shutdown(m_Handle);
            }

            // -1 if no graph
            public static int GetNumEdgesInGraph()
            {
                return get_num_edges_in_graph(m_Handle);
            }

            // RNN 
            public static int TestCakesRNNQuery(float searchRadius, NodeVisitor callback)
            {
                return test_cakes_rnn_query(m_Handle, searchRadius, callback);
            }
        }
    }

}
