using System.Collections.Generic;
using UnityEngine;

using TMPro;
using System.Linq;
using Unity.VisualScripting;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using static UnityEditor.Progress;
using UnityEditor;

namespace Clam
{
    public class TreeScript : MonoBehaviour
    {
        public string dataName = "arrhythmia";
        public uint cardinality = 25;
        public GameObject nodePrefab;
        public TMP_Text text;


        //private Dictionary<string, ClamFFI.NodeData> m_SelectedNodes;
        private Dictionary<string, GameObject> m_Tree;
        private string m_LastSelectedNode;
        private List<NodeDataUnity> m_SelectedNodes;
        private List<NodeDataUnity> m_QueryResults;

        private TempUI m_TempUI;

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
            m_SelectedNodes = new List<NodeDataUnity>();
            m_QueryResults = new List<NodeDataUnity>();

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

            Clam.ClamFFI.TestStructArray();


            //m_NodeMenu = this.AddComponent<Dropdown>();
            //List<string> list = new List<string> { "option1", "option2" };
            //m_NodeMenu.AddOptions(list);
            //m_NodeMenu.GetComponent<Transform>().position = new Vector3(0, 0, 0);

        }

        public void RunForceDirectedSim()
        {
            if (m_SelectedNodes.Count == 0)
            {
                Debug.LogError("error cant run physics sim - no nodes selected");
                return;
            }
            else if (m_SelectedNodes.Count < 2)
            {
                Debug.LogWarning("physics sim being run with only one node");
            }
            Clam.ClamFFI.RunForceDirectedSim(m_SelectedNodes, UpdatePhysicsSim);
        }

        public void UpdatePhysicsSim(ref NodeDataFFI nodeData)
        {

        }

        public void RNN_Test()
        {
            m_TempUI.RNN_Test();
        }



        public void ResetColors()
        {

            m_TempUI.ResetColors();
        }

        public void SelectQueryResults()
        {

            m_TempUI.SelectQueryResults();
        }

        public void DeselectAll()
        {

            m_TempUI.DeselectAll();

        }

        public void DrawEdges()
        {
            DeleteAllLines();

            m_TempUI.DrawEdges();
        }

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

        void SelectNode(string id)
        {
            if (m_Tree.TryGetValue(id, out var node))
            {
                node.GetComponent<NodeScript>().SetColor(new Color(1.0f, 0.0f, 1.0f));
                m_SelectedNodes.Add(node.GetComponent<NodeScript>().ToUnityData());
            }
        }

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
        void SetParentChildLines()
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


        void Update()
        {
            HandleLMC();
            HandleRMC();
            MyQuit();
        }

        void MyQuit()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //print("quitting app");
                Application.Quit();
            }
        }

        void HandleLMC()
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