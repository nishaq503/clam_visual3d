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

    public class TempUI
    {
        private GameObject nodePrefab;
        private TMP_Text text;


        //private Dictionary<string, ClamFFI.NodeData> m_SelectedNodes;
        private Dictionary<string, GameObject> m_Tree;
        private string m_LastSelectedNode;
        private List<NodeDataUnity> m_SelectedNodes;
        private List<NodeDataUnity> m_QueryResults;

        public TempUI(Dictionary<string, GameObject> tree, List<NodeDataUnity> selectedNodes, List<NodeDataUnity> queryResults, GameObject nodePrefab, TMP_Text text)
        {
            m_Tree = tree;
            m_SelectedNodes = selectedNodes;
            m_QueryResults = queryResults;
            this.nodePrefab = nodePrefab;
            this.text = text;
        }


        public void RNN_Test()
        {
            Debug.Log("rnn test");
            ResetColors();

            global::Clam.ClamFFI.TestCakesRNNQuery("test", CakesRNNQuery);
        }

        public unsafe void CakesRNNQuery(ref global::Clam.NodeDataFFI nodeData)
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

                Debug.Log("distance to query: " + nodeData.placeHolder.ToString());
            }
            else
            {
                Debug.Log("node key not found - " + nodeData.id);
            }
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

        public void RandomizeLocations()
        {
            Debug.Log("rand locs");
            foreach (var node in m_SelectedNodes)
            {
                var x = Random.Range(0, 100);
                var y = Random.Range(0, 100);
                var z = Random.Range(0, 100);

                if (m_Tree.TryGetValue(node.id, out var obj))
                {
                    obj.GetComponent<Transform>().position = new Vector3(x, y, z);
                }
            }

            foreach (var node in m_SelectedNodes)
            {
                if (m_Tree.TryGetValue(node.id, out var obj))
                {
                    for (int i = 0; i < obj.GetComponent<Transform>().childCount; i++)
                    {
                        GameObject child = obj.transform.GetChild(i).gameObject;
                        string[] names = child.name.Split();
                        if (m_Tree.TryGetValue(names[1], out var other))
                        {
                            List<Vector3> pos = new List<Vector3>
                            {
                                obj.GetComponent<NodeScript>().GetPosition(),
                                other.GetComponent<NodeScript>().GetPosition()
                            };
                            var l = child.GetComponent<LineRenderer>();
                            if (l != null)
                            {
                                Debug.Log("n1," + names[0] + ",n2," + names[1]);
                                Debug.Log("changing line pos>");
                                l.SetPositions(pos.ToArray());
                            }
                            else
                            {
                                Debug.Log("line rendeer null");
                            }
                            //Do something with child
                        }
                        else
                        {
                            Debug.Log("failed to find????");
                        }
                    }
                }
            }
        }

        public void AddEdge(GameObject node, GameObject other, int edgeCount)
        {
            GameObject childOb = new GameObject(node.GetComponent<NodeScript>().GetId() + " " + other.GetComponent<NodeScript>().GetId());


            LineRenderer l = childOb.AddComponent<LineRenderer>();
            childOb.transform.SetParent(node.transform);
            //if (childOb.name.Contains(node.GetComponent<NodeScript>().GetId()) && childOb.name.Contains(other.GetComponent<NodeScript>().GetId()))
            //{
            //    Debug.Log("error edge already exists");
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

        public void AddBakedEdge(GameObject node, GameObject other, int edgeCount)
        {
            GameObject childOb = new GameObject(node.GetComponent<NodeScript>().GetId() + " " + other.GetComponent<NodeScript>().GetId());
            LineRenderer lineRenderer = childOb.AddComponent<LineRenderer>();



            List<Vector3> pos = new List<Vector3>
                {
                    node.GetComponent<NodeScript>().GetPosition(),
                    other.GetComponent<NodeScript>().GetPosition()
                };

            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.SetPositions(pos.ToArray());
            lineRenderer.useWorldSpace = true;
            //l.startColor = Color.black;
            //l.endColor = Color.black;
            lineRenderer.material.color = Color.black;


            //var lineRenderer = lineObj.GetComponent<LineRenderer>();
            var meshFilter = childOb.AddComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            lineRenderer.BakeMesh(mesh);
            meshFilter.sharedMesh = mesh;

            var meshRenderer = childOb.AddComponent<MeshRenderer>();
            meshRenderer.material = lineRenderer.material;

            GameObject.Destroy(lineRenderer);

            //LineRenderer l = childOb.AddComponent<LineRenderer>();
            //childOb.transform.SetParent(node.transform);


            //    {
            //        List<Vector3> pos = new List<Vector3>
            //    {
            //        node.GetComponent<NodeScript>().GetPosition(),
            //        other.GetComponent<NodeScript>().GetPosition()
            //    };

            //        l.startWidth = 0.1f;
            //        l.endWidth = 0.1f;
            //        l.SetPositions(pos.ToArray());
            //        l.useWorldSpace = false;
            //        //l.startColor = Color.black;
            //        //l.endColor = Color.black;
            //        l.material.color = Color.black;

            //        var meshFilter = .AddComponent<MeshFilter>();
            //        Mesh mesh = new Mesh();
            //        lineRenderer.BakeMesh(mesh);
            //        meshFilter.sharedMesh = mesh;
            //}


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
                Debug.Log("only works when two nodes are selected");
                Debug.Log("len " + m_SelectedNodes.Count);
                return;
            }

            float distance = global::Clam.ClamFFI.DistanceToOther(m_SelectedNodes[0].id, m_SelectedNodes[1].id);
            Debug.Log("distance to other " + distance);
            text.text += "distance: " + distance.ToString();
        }

        public void HandleLMC()
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

                    Debug.Log("selexting");

                    global::Clam.NodeWrapper wrapper = new global::Clam.NodeWrapper(selectedNode.ToNodeData());
                    FFIError found = global::Clam.ClamFFI.GetClusterData(wrapper);
                    if (found == FFIError.Ok)
                    {
                        m_SelectedNodes.Add(new NodeDataUnity(wrapper.Data));
                        selectedNode.SetColor(Color.blue);
                        text.text = wrapper.Data.GetInfo();
                    }

                }
            }
        }

        public void HandleRMC()
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

        void SetIsActiveToChildren(GameObject node, bool isActive)
        {
            Debug.Log("asdibasbaf");
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

        void DeactivateChildren(ref global::Clam.NodeDataFFI nodeData)
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



        public void DrawEdges()
        {
            //DeleteAllLines();
            Debug.Log("test");
            var nodes = m_SelectedNodes;
            int edgeCount = 0;
            for (int i = 0; i < nodes.Count; i++)
            {
                //var entry = nodes[i];
                //var node = entry.Value;
                if (m_Tree.TryGetValue(nodes[i].id, out var node))
                {
                    //var node = m_Tree.GetValueOrDefault(nodes[i].id);
                    //Debug.Log(node.GetComponent<NodeScript>().GetId());
                    NodeWrapper nodeWrapper = new NodeWrapper(node.GetComponent<NodeScript>().ToNodeData());
                    for (int j = i + 1; j < nodes.Count; j++)
                    {
                        //var other = m_Tree.GetValueOrDefault(nodes[j].id);
                        if (m_Tree.TryGetValue(nodes[j].id, out var other))
                        {
                            NodeWrapper otherWrapper = new NodeWrapper(other.GetComponent<NodeScript>().ToNodeData());
                            global::Clam.ClamFFI.GetClusterData(nodeWrapper);
                            global::Clam.ClamFFI.GetClusterData(otherWrapper);

                            float distance = global::Clam.ClamFFI.DistanceToOther(node.GetComponent<NodeScript>().GetId(), other.GetComponent<NodeScript>().GetId());
                            //Debug.Log("distance: " + distance.ToString() + ", sum rad: " + (nodeWrapper.Data.radius + otherWrapper.Data.radius).ToString());
                            // want edges btwn two clustesr whose distbtwn centers <= sum of radius
                            //if (node.GetComponent<NodeScript>().IsLeaf() && other.GetComponent<NodeScript>().IsLeaf())
                            {
                                if (distance <= nodeWrapper.Data.radius + otherWrapper.Data.radius)
                                {
                                    //if (edgeCount < 10)
                                    {
                                        Debug.Log("should get edge**************************************************************************");
                                        edgeCount++;
                                        AddEdge(node, other, edgeCount);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("faled1?");
                        }
                    }
                }
                else
                {
                    Debug.Log("faled?");
                }

            }
            //foreach (var entry in nodes)
            //{
            //    var node = entry.Value;
            //    Debug.Log(node.GetComponent<NodeScript>().GetId());
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
            //        Debug.Log("distance: " + distance.ToString() + ", sum rad: " + (nodeWrapper.Data.radius + otherWrapper.Data.radius).ToString());
            //        // want edges btwn two clustesr whose distbtwn centers <= sum of radius
            //        if (distance <= nodeWrapper.Data.radius + otherWrapper.Data.radius)
            //        {
            //            //AddEdge(node, other);
            //            Debug.Log("should get edge");
            //        }




            //    }
            //}
        }
    }



}
