using System.Collections.Generic;
using UnityEngine;

using TMPro;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;


namespace ClamFFI
{
    public class TreeScript : MonoBehaviour
    {
        public string dataName = "arrhythmia";
        public uint cardinality = 50;
        public GameObject nodePrefab;
        public TMP_Text text;


        private Dictionary<string, GameObject> m_Tree;
        private GameObject m_SelectedNode;
        private Color m_SelectedNodeActualColor;

        public Dictionary<string, GameObject> GetTree()
        {
            return m_Tree;
        }

        void Start()
        {
            int clam_result = ClamFFI.Clam.InitClam(dataName, cardinality);
            if (clam_result == 0)
            {
                Debug.Log(System.String.Format("Error: tree for {0} not created. Check debug log file.", dataName));
                return;
            }
            m_Tree = new Dictionary<string, GameObject>();

            int numNodes = ClamFFI.Clam.GetNumNodes();
            Debug.Log(System.String.Format("created tree with num nodes {0}.", numNodes));

            ClamFFI.Clam.ForEachDFT(SetNodeNames);
            ClamFFI.Clam.CreateReingoldLayout(Reingoldify);
            SetLines();


            //m_NodeMenu = this.AddComponent<Dropdown>();
            //List<string> list = new List<string> { "option1", "option2" };
            //m_NodeMenu.AddOptions(list);
            //m_NodeMenu.GetComponent<Transform>().position = new Vector3(0, 0, 0);

        }

        void SetLines()
        {
            foreach (var item in m_Tree.Values)
            {
                if (item.activeSelf)
                {
                    bool hasLeftChild = m_Tree.TryGetValue(item.GetComponent<NodeScript>().GetLeftChildID(), out var leftChild);
                    bool hasrightChild = m_Tree.TryGetValue(item.GetComponent<NodeScript>().GetRightChildID(), out var rightChild);

                    if (hasLeftChild && hasrightChild)
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
                    if (m_SelectedNode != null)
                    {
                        if (m_SelectedNode.GetComponent<NodeScript>().GetId() == hitInfo.collider.gameObject.GetComponent<NodeScript>().GetId())
                        {
                            m_SelectedNode.GetComponent<NodeScript>().SetColor(m_SelectedNodeActualColor);
                            m_SelectedNode = null;
                            text.text = "";
                            return;
                        }
                        else
                        {
                            m_SelectedNode.GetComponent<NodeScript>().SetColor(m_SelectedNodeActualColor);
                        }
                    }

                    m_SelectedNode = hitInfo.collider.gameObject;
                    m_SelectedNodeActualColor = m_SelectedNode.GetComponent<NodeScript>().GetColor();
                    m_SelectedNode.GetComponent<NodeScript>().SetColor(new Color(0.0f, 0.0f, 1.0f));

                    Debug.Log(m_SelectedNode.GetComponent<NodeScript>().GetId() + " was clicked");
                    Debug.Log("searching for node " + m_SelectedNode.GetComponent<NodeScript>().GetId());

                    ClamFFI.NodeWrapper nodeWrapper = new ClamFFI.NodeWrapper(m_SelectedNode.GetComponent<NodeScript>().ToNodeData());
                    bool found = ClamFFI.Clam.GetClusterData(nodeWrapper);
                    if (found)
                    {
                        nodeWrapper.Data.LogInfo();
                        text.text = nodeWrapper.Data.GetInfo();
                    }
                    else
                    {
                        Debug.LogError("node not found");
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
                        ClamFFI.Clam.ForEachDFT(DeactivateChildren, selectedNode.GetComponent<NodeScript>().GetLeftChildID());
                        ClamFFI.Clam.ForEachDFT(DeactivateChildren, selectedNode.GetComponent<NodeScript>().GetRightChildID());
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

        void DeactivateChildren(ref ClamFFI.NodeData nodeData)
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

        unsafe void SetNodeNames(ref ClamFFI.NodeData nodeData)
        {
            GameObject node = Instantiate(nodePrefab);

            node.GetComponent<NodeScript>().SetID(nodeData.id.AsString);
            node.GetComponent<NodeScript>().SetLeft(nodeData.leftID.AsString);
            node.GetComponent<NodeScript>().SetRight(nodeData.rightID.AsString);
            m_Tree.Add(nodeData.id.AsString, node);
        }

        unsafe void Reingoldify(ref ClamFFI.NodeData nodeData)
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