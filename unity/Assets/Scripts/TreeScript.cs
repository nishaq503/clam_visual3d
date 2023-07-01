using System.Collections.Generic;
using UnityEngine;

using TMPro;
using System.Linq;
using Unity.VisualScripting;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using static UnityEditor.Progress;
using UnityEditor;

namespace ClamFFI
{
    public class TreeScript : MonoBehaviour
    {
        public string dataName = "arrhythmia";
        public uint cardinality = 25;
        public GameObject nodePrefab;
        public TMP_Text text;


        private Dictionary<string, GameObject> m_Tree;
        //private Dictionary<string, ClamFFI.NodeData> m_SelectedNodes;
        private string m_LastSelectedNode;
        private List<NodeDataUnity> m_SelectedNodes;
        private List<NodeDataUnity> m_QueryResults;

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
            ClamFFI.Clam.ShutdownClam();
        }

        void Start()
        {
            FFIError clam_result = ClamFFI.Clam.InitClam(dataName, cardinality);
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

            int numNodes = ClamFFI.Clam.GetNumNodes();
            Debug.Log(System.String.Format("created tree with num nodes {0}.", numNodes));

            FFIError e = ClamFFI.Clam.ForEachDFT(SetNodeNames);

            if (e == FFIError.Ok)
            {
                print("ok)");

            }
            else
            {
                print("ERROR " + e);
            }
            ClamFFI.Clam.CreateReingoldLayout(Reingoldify);
            //SetLines();

            ResetColors();

            ClamFFI.Clam.TestStructArray();


            //m_NodeMenu = this.AddComponent<Dropdown>();
            //List<string> list = new List<string> { "option1", "option2" };
            //m_NodeMenu.AddOptions(list);
            //m_NodeMenu.GetComponent<Transform>().position = new Vector3(0, 0, 0);

        }

        public void RNN_Test()
        {
            print("rnn test");
            ResetColors();

            ClamFFI.Clam.TestCakesRNNQuery("test", CakesRNNQuery);
        }

        public void ResetColors()
        {
            m_SelectedNodes.Clear();
            text.text = "";
            m_Tree.ToList().ForEach(node =>
            {
                node.Value.SetActive(true);
                node.Value.GetComponent<NodeScript>().SetColor(new Color(153.0f / 255.0f, 50.0f / 255.0f, 204.0f / 255.0f));
            });
        }

        public void SelectQueryResults()
        {
            m_SelectedNodes.ForEach(node =>
            {
                m_Tree.GetValueOrDefault(node.id).GetComponent<NodeScript>().SetColor(node.color);
            });

            m_SelectedNodes.Clear();
            m_SelectedNodes = m_QueryResults;
            m_SelectedNodes = m_QueryResults.Select(a => a).ToList();
            m_SelectedNodes.ForEach(node =>
            {
                m_Tree.GetValueOrDefault(node.id).GetComponent<NodeScript>().SetColor(new Color(0.0f, 1.0f, 1.0f));

            });
        }

        public void DeselectAll()
        {
            m_SelectedNodes.ForEach(node =>
            {
                m_Tree.GetValueOrDefault(node.id).GetComponent<NodeScript>().SetColor(node.color);
            });

            m_SelectedNodes.Clear();

        }

        public void DrawEdges()
        {
            DeleteAllLines();
            print("test");
            var nodes = m_SelectedNodes;
            int edgeCount = 0;
            for (int i = 0; i < nodes.Count; i++)
            {
                //var entry = nodes[i];
                //var node = entry.Value;
                if (m_Tree.TryGetValue(nodes[i].id, out var node))
                {
                    //var node = m_Tree.GetValueOrDefault(nodes[i].id);
                    print(node.GetComponent<NodeScript>().GetId());
                    NodeWrapper nodeWrapper = new NodeWrapper(node.GetComponent<NodeScript>().ToNodeData());
                    for (int j = i + 1; j < nodes.Count; j++)
                    {
                        //var other = m_Tree.GetValueOrDefault(nodes[j].id);
                        if (m_Tree.TryGetValue(nodes[j].id, out var other))
                        {
                            NodeWrapper otherWrapper = new NodeWrapper(other.GetComponent<NodeScript>().ToNodeData());
                            ClamFFI.Clam.GetClusterData(nodeWrapper);
                            ClamFFI.Clam.GetClusterData(otherWrapper);

                            float distance = ClamFFI.Clam.DistanceToOther(node.GetComponent<NodeScript>().GetId(), other.GetComponent<NodeScript>().GetId());
                            //print("distance: " + distance.ToString() + ", sum rad: " + (nodeWrapper.Data.radius + otherWrapper.Data.radius).ToString());
                            // want edges btwn two clustesr whose distbtwn centers <= sum of radius
                            //if (node.GetComponent<NodeScript>().IsLeaf() && other.GetComponent<NodeScript>().IsLeaf())
                            {
                                if (distance <= nodeWrapper.Data.radius + otherWrapper.Data.radius)
                                {
                                    //if (edgeCount < 10)
                                    {

                                        print("should get edge**************************************************************************");
                                        edgeCount++;
                                        AddEdge(node, other, edgeCount);
                                    }
                                }
                            }
                        }
                        else
                        {
                            print("faled1?");
                        }




                    }
                }
                else
                {
                    print("faled?");
                }

            }
            //foreach (var entry in nodes)
            //{
            //    var node = entry.Value;
            //    print(node.GetComponent<NodeScript>().GetId());
            //    NodeWrapper nodeWrapper = new NodeWrapper(node.GetComponent<NodeScript>().ToNodeData());
            //    foreach (var otherEntry in nodes)
            //    {
            //        var other = otherEntry.Value;
            //        if (other.GetComponent<NodeScript>().GetId() == node.GetComponent<NodeScript>().GetId())
            //        {
            //            continue;
            //        }


            //        NodeWrapper otherWrapper = new NodeWrapper(other.GetComponent<NodeScript>().ToNodeData());
            //        ClamFFI.Clam.GetClusterData(nodeWrapper);
            //        ClamFFI.Clam.GetClusterData(otherWrapper);

            //        float distance = ClamFFI.Clam.DistanceToOther(node.GetComponent<NodeScript>().GetId(), other.GetComponent<NodeScript>().GetId());
            //        print("distance: " + distance.ToString() + ", sum rad: " + (nodeWrapper.Data.radius + otherWrapper.Data.radius).ToString());
            //        // want edges btwn two clustesr whose distbtwn centers <= sum of radius
            //        if (distance <= nodeWrapper.Data.radius + otherWrapper.Data.radius)
            //        {
            //            //AddEdge(node, other);
            //            print("should get edge");
            //        }




            //    }
            //}
        }

        public void RandomizeLocations()
        {
            foreach (var node in m_SelectedNodes)
            {
                var x = Random.Range(0, 100);
                var y = Random.Range(0, 100);
                var z = Random.Range(0, 100);

                if (m_Tree.TryGetValue(node.id, out var obj))
                {
                    obj.GetComponent<Transform>().position = new Vector3(x, y, z);
                    for (int i = 0; i < obj.GetComponent<Transform>().childCount; i++)
                    {
                        GameObject child = obj.transform.GetChild(i).gameObject;
                        child.GetComponent<Transform>().position = new Vector3(x, y, z);
                        print(child.name);
                        string[] names = child.name.Split();
                        print("n1,"+ names[0] +",n2," + names[1]);
                        if (m_Tree.TryGetValue(names[1], out var other))
                        {
                            print("changing line pos>");
                            List<Vector3> pos = new List<Vector3>
                            {
                                obj.GetComponent<NodeScript>().GetPosition(),
                                other.GetComponent<NodeScript>().GetPosition()
                            };
                            var l = obj.GetComponent<LineRenderer>();
                            if (l != null)
                            {

                                l.startWidth = 0.1f;
                                l.endWidth = 0.1f;
                                l.SetPositions(pos.ToArray());
                                l.useWorldSpace = true;
                            }
                            //Do something with child
                        }
                        else
                        {
                            print("failed to find????");
                        }

                    }
                }

            }
        }

        public void AddEdge(GameObject node, GameObject other, int edgeCount)
        {
            GameObject childOb = new GameObject(node.GetComponent<NodeScript>().GetId() + " " + other.GetComponent<NodeScript>().GetId());

            //LineRenderer l1 = childOb.GetComponent<LineRenderer>();
            //if(l1 != null)
            //{
            //    print("updating line pos**************");
            //    List<Vector3> pos = new List<Vector3>
            //    {
            //        node.GetComponent<NodeScript>().GetPosition(),
            //        other.GetComponent<NodeScript>().GetPosition()
            //    };

            //    l1.startWidth = 0.1f;
            //    l1.endWidth = 0.1f;
            //    l1.SetPositions(pos.ToArray());
            //    l1.useWorldSpace = true;
            //}
            //else
            {
                LineRenderer l = childOb.AddComponent<LineRenderer>();
                childOb.transform.SetParent(node.transform);
                //if (childOb.name.Contains(node.GetComponent<NodeScript>().GetId()) && childOb.name.Contains(other.GetComponent<NodeScript>().GetId()))
                //{
                //    print("error edge already exists");
                //    return;
                //}

                {
                    List<Vector3> pos = new List<Vector3>
                {
                    node.GetComponent<NodeScript>().GetPosition(),
                    other.GetComponent<NodeScript>().GetPosition()
                };

                    l.startWidth = 0.1f;
                    l.endWidth = 0.1f;
                    l.SetPositions(pos.ToArray());
                    l.useWorldSpace = true;
                    //l.startColor = Color.black;
                    //l.endColor = Color.black;
                    l.material.color = Color.black;
                }
            }
            

        }

        public void SelectAllLeafNodes()
        {
            DeselectAll();

            m_Tree.ToList().ForEach(entry =>
            {
                if (entry.Value.GetComponent<NodeScript>().GetLeftChildID() == "None")
                {
                    SelectNode(entry.Value.GetComponent<NodeScript>().GetId());
                }
            });
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
            m_Tree.ToList().ForEach(gameObject =>
            {
                var node = gameObject.Value;
                var found = m_SelectedNodes.Find(n => n.id == node.GetComponent<NodeScript>().GetId());
                if (found == null)
                {
                    node.SetActive(false);
                }
            });
        }

        public void DistanceToOther()
        {
            if (m_SelectedNodes.Count != 2)
            {
                print("only works when two nodes are selected");
                print("len " + m_SelectedNodes.Count);
                return;
            }

            float distance = ClamFFI.Clam.DistanceToOther(m_SelectedNodes[0].id, m_SelectedNodes[1].id);
            print("distance to other " + distance);
            text.text += "distance: " + distance.ToString();
        }

        void DeleteAllLines()
        {
            //m_Tree.ToList().ForEach(entry =>
            //{
            //    var line = entry.Value.GetComponent<LineRenderer>();
            //    //FixedJoint fixedJoint = GetComponent<FixedJoint>();
            //    if (line != null)
            //    {
            //        Destroy(line);
            //    }
            //});
        }

        void SetLines()
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
                            l.useWorldSpace = true;
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
                            l.useWorldSpace = true;
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

        void FixedUpdate()
        {
            //foreach (var item in m_Tree.Values)
            //{
            //    if (item.activeSelf)
            //    {
            //        bool hasLeft = m_Tree.TryGetValue(item.GetComponent<NodeScript>().GetLeftChildID(), out var leftChild);
            //        bool hasRight = m_Tree.TryGetValue(item.GetComponent<NodeScript>().GetRightChildID(), out var rightChild);
            //        if (hasLeft && hasRight)
            //        {
            //            Debug.DrawLine(item.GetComponent<Transform>().position, leftChild.GetComponent<Transform>().position, Color.black, 2.5f);
            //            Debug.DrawLine(item.GetComponent<Transform>().position, rightChild.GetComponent<Transform>().position, Color.white, 2.5f);
            //        }
            //    }
            //}
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
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePosition = Input.mousePosition;
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                RaycastHit hitInfo;

                if (Physics.Raycast(ray.origin, ray.direction * 10, out hitInfo, Mathf.Infinity))
                {
                    var selectedNode = hitInfo.collider.gameObject.GetComponent<NodeScript>();
                    if (m_SelectedNodes.Count > 0)
                    {
                        var existingSelection = m_SelectedNodes.Find(node => node.id == selectedNode.GetId());
                        if (existingSelection != null)
                        {
                            selectedNode.SetColor(existingSelection.color);
                            m_SelectedNodes.Remove(existingSelection);
                            if (m_SelectedNodes.Count > 0)
                            {
                                text.text = m_SelectedNodes.Last().GetInfo();
                            }
                            else
                            {
                                text.text = "";
                            }
                            return;
                        }
                    }

                    print("selexting");

                    ClamFFI.NodeWrapper wrapper = new ClamFFI.NodeWrapper(selectedNode.ToNodeData());
                    FFIError found = ClamFFI.Clam.GetClusterData(wrapper);
                    if (found == FFIError.Ok)
                    {
                        m_SelectedNodes.Add(new NodeDataUnity(wrapper.Data));
                        selectedNode.SetColor(Color.blue);
                        text.text = wrapper.Data.GetInfo();
                    }

                }
            }
        }

        void HandleRMC()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Vector3 mousePosition = Input.mousePosition;
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                RaycastHit hitInfo;

                if (Physics.Raycast(ray.origin, ray.direction * 10, out hitInfo, Mathf.Infinity))
                {


                    var selectedNode = hitInfo.collider.gameObject;

                    //ClamFFI.NodeWrapper nodeWrapper = new ClamFFI.NodeWrapper(selectedNode.GetComponent<NodeScript>().ToNodeData());
                    if (selectedNode != null)
                    {
                        //ClamFFI.Clam.ForEachDFT(DeactivateChildren, selectedNode.GetComponent<NodeScript>().GetLeftChildID());
                        //ClamFFI.Clam.ForEachDFT(DeactivateChildren, selectedNode.GetComponent<NodeScript>().GetRightChildID());
                        var hasLC = m_Tree.TryGetValue(selectedNode.GetComponent<NodeScript>().GetLeftChildID(), out var lc);
                        if (hasLC)
                        {
                            SetIsActiveToChildren(selectedNode, !lc.activeSelf);
                        }
                    }
                    //bool found = ClamFFI.Clam.GetClusterData(nodeWrapper);
                    //if (found)
                    //{
                    //    nodeWrapper.Data.LogInfo();
                    //    text.text = nodeWrapper.Data.GetInfo();
                    //}
                    //else
                    //{
                    //    Debug.LogError("node not found");
                    //}
                }
            }
        }

        void DeactivateChildren(ref ClamFFI.NodeDataFFI nodeData)
        {
            GameObject node;

            bool hasValue = m_Tree.TryGetValue(nodeData.id.AsString, out node);
            if (hasValue)
            {
                node.SetActive(!node.activeSelf);
            }
            else
            {
                Debug.Log("reingoldify key not found - " + nodeData.id);
            }
        }

        unsafe void SetNodeNames(ref ClamFFI.NodeDataFFI nodeData)
        {
            GameObject node = Instantiate(nodePrefab);
            print("setting name " + node.GetComponent<NodeScript>().GetId());
            nodeData.LogInfo();
            node.GetComponent<NodeScript>().SetID(nodeData.id.AsString);
            node.GetComponent<NodeScript>().SetLeft(nodeData.leftID.AsString);
            node.GetComponent<NodeScript>().SetRight(nodeData.rightID.AsString);
            m_Tree.Add(nodeData.id.AsString, node);
        }

        public unsafe void CakesRNNQuery(ref ClamFFI.NodeDataFFI nodeData)
        {
            GameObject node;

            bool hasValue = m_Tree.TryGetValue(nodeData.id.AsString, out node);
            if (hasValue)
            {
                node.GetComponent<NodeScript>().SetColor(nodeData.color.AsColor);
                m_SelectedNodes.Add(node.GetComponent<NodeScript>().ToUnityData());
                m_QueryResults.Add(node.GetComponent<NodeScript>().ToUnityData());
                text.text = nodeData.GetInfo();
                text.text += "distance to query: " + nodeData.placeHolder.ToString();

                print("distance to query: " + nodeData.placeHolder.ToString());
            }
            else
            {
                Debug.Log("node key not found - " + nodeData.id);
            }
        }

        unsafe void Reingoldify(ref ClamFFI.NodeDataFFI nodeData)
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

        void SetIsActiveToChildren(GameObject node, bool isActive)
        {
            print("asdibasbaf");
            //bool hasValue = m_Tree.TryGetValue(id, out var node);
            //if (hasValue)
            {
                //node.SetActive(isActive);
                var lid = node.GetComponent<NodeScript>().GetLeftChildID();
                var rid = node.GetComponent<NodeScript>().GetRightChildID();
                var hasLC = m_Tree.TryGetValue(lid, out var leftChild);
                var hasRC = m_Tree.TryGetValue(rid, out var rightChild);

                if (hasLC && hasRC)
                {
                    leftChild.SetActive(isActive);
                    rightChild.SetActive(isActive);

                    SetIsActiveToChildren(leftChild, isActive);
                    SetIsActiveToChildren(rightChild, isActive);

                }
            }
        }
    }



}