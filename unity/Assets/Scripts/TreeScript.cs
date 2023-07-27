using System.Collections.Generic;
using UnityEngine;

using TMPro;
using System.Linq;
using Unity.VisualScripting;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using static UnityEditor.Progress;
using UnityEditor;
using System.Security.Cryptography;

namespace Clam
{
    public class TreeScript : MonoBehaviour
    {
        public string dataName = "arrhythmia";
        public uint cardinality = 25;
        public GameObject nodePrefab;
        public GameObject springPrefab;
        public TMP_Text text;

        public TMP_Text ClusterText;
        public GameObject ClusterUI;


        //private Dictionary<string, ClamFFI.NodeData> m_SelectedNodes;
        private Dictionary<string, GameObject> m_Tree;
        private string m_LastSelectedNode;
        private List<GameObject> m_SelectedNodes;
        private List<GameObject> m_QueryResults;
        private List<GameObject> m_GraphNodes;

        private TempUI m_TempUI;
        private bool m_IsPhysicsRunning = false;
        private int m_MaxPhysicsIters = 1000;
        private int m_PhysicsIter = 0;

        private float m_EdgeScalar = 25.0f;
        private float m_SearchRadius = 0.05f;

        //private List<Color> m_SelectedNodeActualColors;

        public Dictionary<string, GameObject> GetTree()
        {
            return m_Tree;
        }

        void OnApplicationQuit()
        {
            Debug.Log("Application ending after " + Time.time + " seconds");
            //m_Tree = new Dictionary<string, GameObject>();
            //m_SelectedNode = null;
            Clam.ClamFFI.ShutdownClam();
        }

        void Start()
        {
            FFIError clam_result = Clam.ClamFFI.InitClam(dataName, cardinality);
            if (clam_result != FFIError.Ok)
            {
                Debug.Log(System.String.Format("Error: tree for {0} not created. Check debug log file.", dataName));
                if (EditorApplication.isPlaying)
                {
                    EditorApplication.isPlaying = false;
                }
                else
                {
                    Application.Quit();

                }
                return;
            }
            //print(ClamFFI.Clam.GetNumNodes());
            m_Tree = new Dictionary<string, GameObject>();
            m_SelectedNodes = new List<GameObject>();
            m_QueryResults = new List<GameObject>();

            int numNodes = Clam.ClamFFI.GetNumNodes();
            Debug.Log(System.String.Format("created tree with num nodes {0}.", numNodes));

            FFIError e = Clam.ClamFFI.ForEachDFT(SetNodeNames);

            if (e == FFIError.Ok)
            {
                print("ok)");
            }
            else
            {
                print("ERROR " + e);
            }
            Clam.ClamFFI.CreateReingoldLayout(Reingoldify);
            //SetLines();

            m_TempUI = new TempUI(m_Tree, m_SelectedNodes, m_QueryResults, nodePrefab, text);
            ResetColors();

            //Clam.ClamFFI.TestStructArray();

            //m_NodeMenu = this.AddComponent<Dropdown>();
            //List<string> list = new List<string> { "option1", "option2" };
            //m_NodeMenu.AddOptions(list);
            //m_NodeMenu.GetComponent<Transform>().position = new Vector3(0, 0, 0);

        }

        public void ColorByDistToQuery()
        {
            m_TempUI.ColorAllByDistToQuery();
        }

        public void RunForceDirectedSim()
        {
            m_IsPhysicsRunning = true;
            if (m_SelectedNodes.Count == 0)
            {
                Debug.LogError("error cant run physics sim - no nodes selected");
                return;
            }
            else if (m_SelectedNodes.Count < 2)
            {
                Debug.LogWarning("physics sim being run with only one node");
            }

            print("initializing physics sim");

            List<NodeDataUnity> nodes = new List<NodeDataUnity>();
            foreach (GameObject node in m_SelectedNodes)
            {
                nodes.Add(node.GetComponent<NodeScript>().ToUnityData());
            }
            Clam.ClamFFI.InitForceDirectedSim(nodes, EdgeDrawer);
        }

        public void UpdatePhysicsSim(ref NodeDataFFI nodeData)
        {

            if (m_Tree.TryGetValue(nodeData.id.AsString, out var node))
            {
                node.GetComponent<NodeScript>().SetPosition(nodeData.pos.AsVector3);
            }
            else
            {
                Debug.Log("physics upodate key not found - " + nodeData.id);
            }
        }

        public void EdgeDrawer(ref NodeDataFFI nodeData)
        {
            if (this.m_Tree.TryGetValue(nodeData.id.AsString, out var node))
            {
                if (this.m_Tree.TryGetValue(nodeData.leftID.AsString, out var other))
                {
                    //m_TempUI.AddEdge(node, other, 0);

                    var spring = Instantiate(springPrefab);

                    spring.GetComponent<SpringScript>().SetNodes(node, other);

                }
            }
        }

        public void DrawEdges()
        {
            List<NodeDataUnity> nodes = new List<NodeDataUnity>();
            foreach (GameObject node in m_SelectedNodes)
            {
                nodes.Add(node.GetComponent<NodeScript>().ToUnityData());
            }
            ClamFFI.DetectEdges(nodes, EdgeDrawer);
        }

        public void RNN_Test()
        {
            m_TempUI.RNN_Test(m_SearchRadius);
        }

        public void ResetAll()
        {
            m_TempUI.Reset();
            Clam.ClamFFI.CreateReingoldLayout(Reingoldify);
            GameObject[] gos = GameObject.FindGameObjectsWithTag("Spring");
            Debug.Log("found " + gos.Length.ToString() + " springs");
            foreach (GameObject go in gos)
            {
                Destroy(go);
            }
        }






        public void ResetColors()
        {

            m_TempUI.ResetColors();
        }

        public void SelectQueryResults()
        {

            m_TempUI.SelectQueryResults();
        }

        public void ClamRadius()
        {

            m_TempUI.ClamRadius();
        }

        public void EstimateQueryLocation()
        {
            var obj = Instantiate(nodePrefab);
            List<Vector3> given = new List<Vector3>();
            List<float> dist = new List<float>();

            foreach (var node in m_QueryResults)
            {
                given.Add(node.transform.position);
                dist.Add(node.GetComponent<NodeScript>().distanceToQuery);

            }

            obj.transform.position = SolveQueryPos(given, dist);
            obj.GetComponent<Renderer>().material.color = Color.red;
        }

        //    Point solve(const vector<Point>& given, const vector<double>& dist) {
        //  Point res;
        //        double alpha = ALPHA;
        //  for (int iter = 0; iter<ITER; iter++) {
        //      Point delta;
        //      for (int i = 0; i<given.size(); i++) {
        //        double d = res.dist(given[i]);
        //        Point diff = (given[i] - res) * (alpha * (d - dist[i]) / max(dist[i], d));
        //        delta = delta + diff;
        //      }
        //    delta = delta* (1.0 / given.size()); 
        //      alpha *= RATIO;   
        //      res = res + delta;
        //  }
        //return res;
        //}

        Vector3 SolveQueryPos(List<Vector3> given, List<float> dist)
        {
            //const int iter = 2000;
            float alpha = 2.0f;
            const float ratio = 0.99f;

            Vector3 res = new Vector3(0, 0, 0);
            for (int iter = 0; iter < 1000; iter++)
            {
                Vector3 delta = new Vector3(0, 0, 0);
                for (int i = 0; i < given.Count; i++)
                {
                    float d = Vector3.Distance(res, given[i]);
                    Vector3 diff = (given[i] - res);
                    float val = (alpha * (d - dist[i]) / Mathf.Max(dist[i], d));
                    Vector3 diff2 = new Vector3(diff.x * val, diff.y * val, diff.z * val);
                    delta += diff2;
                }

                delta = delta * (1.0f / given.Count);
                alpha *= ratio;
                res = res + delta;
            }

            return res;
        }

        public void HighlightQueryResults()
        {

            //foreach(var node in m_QueryResults)
            //{
            //    //if(m_Tree.TryGetValue(node.id, out var obj))
            //    {
            //        node.GetComponent<NodeScript>().SetColor(node.color);
            //    }
            //}
        }

        public void SelectAllActive()
        {
            m_SelectedNodes.Clear();
            foreach (var node in m_Tree)
            {
                if (node.Value.activeSelf)
                {
                    m_SelectedNodes.Add(node.Value);
                }
            }

            Debug.Log("num selected: " + m_SelectedNodes.Count);

        }

        public void DeselectAll()
        {

            m_TempUI.DeselectAll();

        }

        //public void DrawEdges()
        //{
        //    DeleteAllLines();

        //    m_TempUI.DrawEdges();
        //}

        public void RandomizeLocations()
        {
            m_TempUI.RandomizeLocations();
        }

        public void AddEdge(GameObject node, GameObject other, int edgeCount)
        {
            m_TempUI.AddEdge(node, other, edgeCount);
        }

        public void SelectAllLeafNodes()
        {
            m_TempUI.SelectAllLeafNodes();
        }

        public void SampleLeafNodes()
        {
            int numLeaves = 10;
            m_TempUI.SampleLeafNodes(numLeaves);
        }

        //public void SelectNode(string id)
        //{
        //    //if (m_Tree.TryGetValue(id, out var node))
        //    //{
        //    //    node.GetComponent<NodeScript>().SetColor(new Color(1.0f, 0.0f, 1.0f));
        //    //    m_SelectedNodes.Add(node);
        //    //}
        //    m_TempUI.SelectNode(id);
        //}

        public void HideUnselectedNodes()
        {
            m_TempUI.HideUnselectedNodes();
        }

        public void DistanceToOther()
        {
            m_TempUI.DistanceToOther();
        }


        void DeleteAllLines()
        {
            m_Tree.ToList().ForEach(entry =>
            {
                var line = entry.Value.GetComponent<LineRenderer>();
                //FixedJoint fixedJoint = GetComponent<FixedJoint>();
                if (line != null)
                {
                    Destroy(line);
                }
            });
        }
        public void SetParentChildLines()
        {
            foreach (var item in m_Tree.Values)
            {
                if (item.activeSelf)
                {

                    bool hasLeftChild = m_Tree.TryGetValue(item.GetComponent<NodeScript>().GetLeftChildID(), out var leftChild);
                    bool hasrightChild = m_Tree.TryGetValue(item.GetComponent<NodeScript>().GetRightChildID(), out var rightChild);

                    if (hasLeftChild && hasrightChild && leftChild.activeSelf && rightChild.activeSelf)
                    {
                        {
                            List<Vector3> pos = new List<Vector3>
                        {
                            item.GetComponent<NodeScript>().GetPosition(),
                            rightChild.GetComponent<NodeScript>().GetPosition()
                        };

                            LineRenderer l = rightChild.AddComponent<LineRenderer>();

                            l.startWidth = 0.1f;
                            l.endWidth = 0.1f;
                            l.SetPositions(pos.ToArray());
                            l.useWorldSpace = false;
                            //l.startColor = Color.black;
                            //l.endColor = Color.black;
                            l.material.color = Color.black;
                        }

                        {
                            List<Vector3> pos = new List<Vector3>
                            {
                                item.GetComponent<NodeScript>().GetPosition(),
                                leftChild.GetComponent<NodeScript>().GetPosition()
                            };
                            LineRenderer l = leftChild.AddComponent<LineRenderer>();

                            l.startWidth = 0.1f;
                            l.endWidth = 0.1f;
                            l.SetPositions(pos.ToArray());
                            l.useWorldSpace = false;
                            //l.startColor = Color.green;
                            //l.endColor = Color.black;
                            l.material.color = Color.black;
                        }
                    }
                    //LineRenderer l = gameObject.AddComponent<LineRenderer>();

                    //List<Vector3> pos = new List<Vector3>();
                    //Debug.Log()
                    //pos.Add(new Vector3(0, 0));
                    //pos.Add(new Vector3(10, 10));
                    //l.startWidth = 1f;
                    //l.endWidth = 1f;
                    //l.SetPositions(pos.ToArray());
                    //l.useWorldSpace = true;

                    //pos.Add(new Vector3(0, 0));
                    //pos.Add(new Vector3(10, 10));
                }
            }
        }

        public void DemoPhysics()
        {
            m_IsPhysicsRunning = true;
            //m_TempUI.SampleLeafNodes(45);
            m_TempUI.SelectAllLeafNodes();
            m_GraphNodes = m_SelectedNodes;
            RNN_Test();
            HideUnselectedNodes();
            RandomizeLocations();
            if (m_SelectedNodes.Count == 0)
            {
                Debug.LogError("error cant run physics sim - no nodes selected");
                return;
            }
            else if (m_SelectedNodes.Count < 2)
            {
                Debug.LogWarning("physics sim being run with only one node");
            }

            print("initializing physics sim");
            foreach (GameObject node in m_SelectedNodes)
            {
                //if (m_Tree.TryGetValue(node.id, out var obj))
                //{
                //    node.pos = obj.GetComponent<NodeScript>().GetPosition();
                //}
            }
            List<NodeDataUnity> nodes = new List<NodeDataUnity>();
            foreach (GameObject node in m_SelectedNodes)
            {
                nodes.Add(node.GetComponent<NodeScript>().ToUnityData());
            }
            //Clam.ClamFFI.InitForceDirectedSim(nodes, EdgeDrawer);
            Clam.ClamFFI.LaunchPhysicsThread(nodes, m_EdgeScalar, m_MaxPhysicsIters, EdgeDrawer, UpdatePhysicsSim);

        }

        void Update()
        {
            if (m_IsPhysicsRunning)
            {
                if (ClamFFI.PhysicsUpdateAsync() == FFIError.PhysicsFinished)
                {
                    m_IsPhysicsRunning = false;
                    print("physics finished");
                }
                //if (m_PhysicsIter < m_MaxPhysicsIters)
                //{
                //    ApplyForces();
                //    m_PhysicsIter++;
                //}
                //else
                //{
                //    ClamFFI.ShutdownPhysics();
                //    print("finished sim");
                //    m_IsPhysicsRunning = false;
                //}
            }
            //HandleLMC();
            HandleRMC();
            MyQuit();
        }

        public void ApplyForces()
        {
            print("applying forces");
            Clam.ClamFFI.ApplyForces(m_EdgeScalar, UpdatePhysicsSim);
            m_PhysicsIter++;
            //foreach (var obj in m_SelectedNodes)
            //{
            //    //if (m_Tree.TryGetValue(node.id, out var obj))
            //    {
            //        for (int i = 0; i < obj.GetComponent<Transform>().childCount; i++)
            //        {
            //            GameObject child = obj.transform.GetChild(i).gameObject;
            //            string[] names = child.name.Split();
            //            if (m_Tree.TryGetValue(names[1], out var other))
            //            {
            //                List<Vector3> pos = new List<Vector3>
            //                {
            //                    obj.GetComponent<NodeScript>().GetPosition(),
            //                    other.GetComponent<NodeScript>().GetPosition()
            //                };
            //                var l = child.GetComponent<LineRenderer>();
            //                if (l != null)
            //                {
            //                    Debug.Log("n1," + names[0] + ",n2," + names[1]);
            //                    Debug.Log("changing line pos>");
            //                    l.SetPositions(pos.ToArray());
            //                }
            //                else
            //                {
            //                    Debug.Log("line rendeer null");
            //                }
            //                //Do something with child
            //            }
            //            else
            //            {
            //                Debug.Log("failed to find????");
            //            }
            //        }
            //    }
            //}
        }

        void MyQuit()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //print("quitting app");
                Application.Quit();
            }
        }

        void OnLMC()
        {
            print("test onlmc1234");
            m_TempUI.HandleLMC();
        }

        public void HandleLMC()
        {
            m_TempUI.HandleLMC();
        }

        void HandleRMC()
        {
            m_TempUI.HandleRMC();

        }



        unsafe void SetNodeNames(ref Clam.NodeDataFFI nodeData)
        {
            GameObject node = Instantiate(nodePrefab);
            print("setting name " + node.GetComponent<NodeScript>().GetId());
            nodeData.LogInfo();
            node.GetComponent<NodeScript>().SetID(nodeData.id.AsString);
            node.GetComponent<NodeScript>().SetLeft(nodeData.leftID.AsString);
            node.GetComponent<NodeScript>().SetRight(nodeData.rightID.AsString);
            m_Tree.Add(nodeData.id.AsString, node);
        }



        unsafe void Reingoldify(ref Clam.NodeDataFFI nodeData)
        {
            GameObject node;

            bool hasValue = m_Tree.TryGetValue(nodeData.id.AsString, out node);
            if (hasValue)
            {
                node.GetComponent<NodeScript>().SetColor(nodeData.color.AsColor);
                node.GetComponent<NodeScript>().SetPosition(nodeData.pos.AsVector3);
            }
            else
            {
                Debug.Log("reingoldify key not found - " + nodeData.id);
            }
        }


    }



}