using Clam;
using Clam.FFI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Clam
{
    public class Tree : MonoBehaviour
    {
        public GameObject m_NodePrefab;
        public GameObject m_SpringPrefab;

        private string m_DataName;
        private uint m_Cardinality;

        public TreeStartupData m_TreeData;

        private Dictionary<string, GameObject> m_Tree;

        private float m_EdgeScalar = 25.0f;
        private float m_SearchRadius = 0.05f;
        public bool m_IsPhysicsRunning = false;

        //public void Init(GameObject nodePrefab, GameObject springPrefab, string dataName, uint cardinality)
        public FFIError Init(GameObject nodePrefab, GameObject springPrefab)
        {
            m_NodePrefab = nodePrefab;
            m_SpringPrefab = springPrefab;
            //m_NodePrefab = nodePrefab;
            //m_SpringPrefab = springPrefab;
            //m_DataName = dataName;
            //m_Cardinality = cardinality;
            //m_TreeData = new TreeStartupData();
            m_TreeData = MenuEventManager.instance.m_TreeData;
            //m_TreeData.dataName = "arrhythmia";
            //m_TreeData.cardinality = 15;
            if (m_TreeData.dataName == null || m_TreeData.dataName.Length == 0)
            {
                Debug.Log("error with tree data");
                //Application.Quit();
                return FFIError.StartupDataInvalid;

            }

            FFIError clam_result = Clam.FFI.NativeMethods.InitClam(m_TreeData.dataName, m_TreeData.cardinality);

            if (clam_result != FFIError.Ok)
            {
                Debug.Log("error with tree data");
                //Application.Quit();
                return clam_result;
            }

            m_Tree = new Dictionary<string, GameObject>();

            //int numNodes = Clam.FFI.NativeMethods.GetNumNodes();




            FFIError e = Clam.FFI.NativeMethods.SetNames(SetNodeNames);
            //FFIError e = Clam.FFI.NativeMethods.ForEachDFT(SetNodeNames);

            MenuEventManager.instance.SetTree(m_Tree);




            if (e == FFIError.Ok)
            {
                Debug.Log("ok)");
            }
            else
            {
                Debug.Log("ERROR " + e);
            }
            Clam.FFI.NativeMethods.DrawHeirarchy(PositionUpdater);

            Clam.FFI.NativeMethods.ForEachDFT(EdgeDrawer);

            if (Clam.FFI.NativeMethods.GetRootData(out var rootData) == true)
            {
                Debug.Log(System.String.Format("created tree with num nodes {0}.", rootData.Data.cardinality));
            }
            else
            {
                Debug.LogError("root not found?");
                return FFIError.HandleInitFailed;
            }

            return FFIError.Ok;
        }


        public void EdgeDrawer(ref FFI.ClusterData nodeData)
        {
            if (m_Tree.TryGetValue(nodeData.id.AsString, out var node))
            {
                if (node.activeSelf && !node.GetComponent<Node>().IsLeaf())
                {
                    if (m_Tree.TryGetValue(node.GetComponent<Node>().GetLeftChildID(), out var lc))
                    {
                        var spring = MenuEventManager.instance.MyInstantiate(m_SpringPrefab);
                        //var sprint = SpringScript.CreateInstance(node, lc, SpringScript.SpringType.heirarchal);
                        //var spring = MenuEventManager.instance.MyInstantiate(m_SpringPrefab);

                        spring.GetComponent<Edge>().InitLineRenderer(node, lc, Edge.SpringType.heirarchal);

                        //spring.GetComponent<SpringScript>().SetNodes(node, lc);
                        //spring.GetComponent<SpringScript>().SetColor(Color.white);

                    }

                    if (m_Tree.TryGetValue(node.GetComponent<Node>().GetRightChildID(), out var rc))
                    {
                        var spring = MenuEventManager.instance.MyInstantiate(m_SpringPrefab);

                        spring.GetComponent<Edge>().InitLineRenderer(node, rc, Edge.SpringType.heirarchal);

                        //spring.GetComponent<SpringScript>().SetNodes(node, rc);
                        //var sprint = SpringScript.CreateInstance(node, lc, SpringScript.SpringType.Similarity);

                        //spring.GetComponent<SpringScript>().SetColor(Color.white);
                    }
                }
            }
        }
        public Dictionary<string, GameObject> GetTree()
        {
            return m_Tree;
        }

        public void Set(Dictionary<string, GameObject> tree)
        {
            m_Tree = tree;
        }

        public bool Contains(string id)
        {
            return m_Tree.ContainsKey(id);
        }

        public GameObject Add(string id)
        {
            var wrapper = NativeMethods.CreateClusterIDsWrapper(id);
            if (wrapper != null)
            {
                GameObject node = Instantiate(m_NodePrefab);
                //nodeData.LogInfo();
                node.GetComponent<Node>().SetID(wrapper.Data.id.AsString);
                node.GetComponent<Node>().SetLeft(wrapper.Data.leftID.AsString);
                node.GetComponent<Node>().SetRight(wrapper.Data.rightID.AsString);
                print("setting name here add" + node.GetComponent<Node>().GetId());
                m_Tree.Add(id, node);
                return node;
            }
            return null;

        }

        public GameObject GetOrAdd(string id)
        {
            var wrapper = NativeMethods.CreateClusterIDsWrapper(id);
            if (wrapper != null)
            {
                GameObject node = Instantiate(m_NodePrefab);
                node.GetComponent<Node>().SetID(wrapper.Data.id.AsString);
                node.GetComponent<Node>().SetLeft(wrapper.Data.leftID.AsString);
                node.GetComponent<Node>().SetRight(wrapper.Data.rightID.AsString);
                print("setting name here getoradd " + node.GetComponent<Node>().GetId());
                m_Tree.Add(id, node);
                return node;
            }
            else
            {
                return null;
            }
        }


        public void Update()
        {
            //if (m_IsPhysicsRunning)
            //{
            //    //if (ClamFFI.PhysicsUpdateAsync() == FFIError.PhysicsFinished)
            //    //{
            //    //    m_IsPhysicsRunning = false;
            //    //    print("physics finished");
            //    //}
            //    //if (m_PhysicsIter < m_MaxPhysicsIters)
            //    //{
            //    //    ApplyForces();
            //    //    m_PhysicsIter++;
            //    //}
            //    //else
            //    //{
            //    //    ClamFFI.ShutdownPhysics();
            //    //    print("finished sim");
            //    //    m_IsPhysicsRunning = false;
            //    //}
            //}
        }
        unsafe void SetNodeNames(ref Clam.FFI.ClusterIDs nodeData)
        {
            GameObject node = Instantiate(m_NodePrefab);
            //nodeData.LogInfo();
            node.GetComponent<Node>().SetID(nodeData.id.AsString);
            node.GetComponent<Node>().SetLeft(nodeData.leftID.AsString);
            node.GetComponent<Node>().SetRight(nodeData.rightID.AsString);
            print("setting name here " + node.GetComponent<Node>().GetId());
            m_Tree.Add(nodeData.id.AsString, node);
        }



        unsafe void PositionUpdater(ref Clam.FFI.ClusterData nodeData)
        {
            GameObject node;

            bool hasValue = m_Tree.TryGetValue(nodeData.id.AsString, out node);
            if (hasValue)
            {
                node.GetComponent<Node>().SetColor(nodeData.color.AsColor);
                node.GetComponent<Node>().SetPosition(nodeData.pos.AsVector3);
            }
            else
            {
                Debug.Log("reingoldify key not found - " + nodeData.id);
            }
        }
    }
}

