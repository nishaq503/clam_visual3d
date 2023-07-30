using System.Collections.Generic;
using UnityEngine;

using TMPro;
using System.Linq;

namespace Clam
{

    public class TempUI
    {
        private GameObject nodePrefab;
        private TMP_Text text;


        //private Dictionary<string, ClamFFI.NodeData> m_SelectedNodes;
        private Dictionary<string, GameObject> m_Tree;
        private string m_LastSelectedNode;
        private List<GameObject> m_SelectedNodes;
        private List<GameObject> m_QueryResults;


        private bool m_IsQuerySelected = false;
        private Color m_DefaultColor;
        private Color m_SelectedColor;
        private Color m_QueryHitFullColor;
        private Color m_QueryHitPartialColor;
        //private bool m_IsQuerySelected = false;

        public TempUI(Dictionary<string, GameObject> tree, List<GameObject> selectedNodes, List<GameObject> queryResults, GameObject nodePrefab, TMP_Text text)
        {
            m_Tree = tree;
            m_SelectedNodes = selectedNodes;
            m_QueryResults = queryResults;
            this.nodePrefab = nodePrefab;
            this.text = text;

            m_DefaultColor = new Color(153.0f / 255.0f, 50.0f / 255.0f, 204.0f / 255.0f);
            m_SelectedColor = new Color(0.0f, 0.0f, 1.0f);
            m_QueryHitFullColor = new Color(0.0f, 01.0f, 0.0f);
            m_QueryHitPartialColor = new Color(0.0f, 1.0f, 1.0f);
        }


        public void RNN_Test(float searchRadius)
        {
            Debug.Log("rnn test");
            //ResetColors();

            global::Clam.ClamFFI.TestCakesRNNQuery(searchRadius, CakesRNNQuery);
        }

        public unsafe void CakesRNNQuery(ref global::Clam.NodeDataFFI nodeData)
        {
            GameObject node;

            bool hasValue = m_Tree.TryGetValue(nodeData.id.AsString, out node);
            if (hasValue)
            {
                node.GetComponent<NodeScript>().SetColor(nodeData.color.AsColor);
                node.GetComponent<NodeScript>().SetActualColor(nodeData.color.AsColor);
                node.GetComponent<NodeScript>().distanceToQuery = nodeData.distToQuery;
                m_SelectedNodes.Add(node);
                m_QueryResults.Add(node);
                text.text = nodeData.GetInfo();
                text.text += "distance to query: " + nodeData.distToQuery.ToString();

                Debug.Log("distance to query: " + nodeData.distToQuery.ToString());
            }
            else
            {
                Debug.Log("node key not found - " + nodeData.id);
            }
        }

        public void ResetColors()
        {
            m_Tree.ToList().ForEach(node =>
            {
                //node.Value.SetActive(true);
                if (node.Value.activeSelf)
                {
                    node.Value.GetComponent<NodeScript>().SetColor(new Color(153.0f / 255.0f, 50.0f / 255.0f, 204.0f / 255.0f));
                }
            });
        }

        public void ClamRadius()
        {
            foreach (var node in m_SelectedNodes)
            {
                NodeWrapper nodeWrapper = new NodeWrapper(node.GetComponent<NodeScript>().ToNodeData());
                Clam.ClamFFI.GetClusterData(nodeWrapper);

                node.GetComponent<Transform>().localScale = new Vector3(nodeWrapper.Data.radius * 100, nodeWrapper.Data.radius * 100, nodeWrapper.Data.radius * 100);
            }
        }


        public void Reset()
        {
            m_SelectedNodes.Clear();
            text.text = "";
            m_Tree.ToList().ForEach(node =>
            {
                node.Value.SetActive(true);
                node.Value.GetComponent<NodeScript>().SetColor(new Color(153.0f / 255.0f, 50.0f / 255.0f, 204.0f / 255.0f));
            });
            
        }

        public void HighLightQueryResults()
        {
            m_IsQuerySelected = !m_IsQuerySelected;
            if (m_IsQuerySelected)
            {
                foreach (var node in m_QueryResults)
                {
                    node.GetComponent<NodeScript>().SetColor(node.GetComponent<NodeScript>().GetActualColor());
                }
            }
            else
            {
                foreach (var node in m_QueryResults)
                {
                    node.GetComponent<NodeScript>().SetColor(m_DefaultColor);
                }
            }
            
        }

        public void SelectQueryResults()
        {
            m_IsQuerySelected = !m_IsQuerySelected;

            if (m_IsQuerySelected)
            {
                foreach (var node in m_QueryResults)
                {
                    SelectNode(node);
                    node.GetComponent<NodeScript>().SetColor(node.GetComponent<NodeScript>().GetActualColor());
                }
                Debug.Log("num selected: " + m_SelectedNodes.Count);

                //m_SelectedNodes.ForEach(node =>
                //{
                //    m_Tree.GetValueOrDefault(node.id).GetComponent<NodeScript>().SetColor(node.color);
                //});

                //m_SelectedNodes.Clear();
                //m_SelectedNodes = m_QueryResults;
                //m_SelectedNodes = m_QueryResults.Select(a => a).ToList();
                //m_SelectedNodes.ForEach(node =>
                //{
                //    m_Tree.GetValueOrDefault(node.id).GetComponent<NodeScript>().SetColor(new Color(0.0f, 1.0f, 1.0f));

                //});


            }
            else
            {
                foreach (var node in m_QueryResults)
                {
                    DeSelectNode(node);
                }
            }


        }


        public void ColorAllByDistToQuery()
        {
            Debug.Log("num selected: " + m_SelectedNodes.Count);

            List<NodeDataUnity> nodes = new List<NodeDataUnity>();
            foreach (var obj in m_SelectedNodes)
            {
                //obj.GetComponent<NodeScript>().SetColor(new Color(1.0f, 1.0f, 1.0f));
                nodes.Add(obj.GetComponent<NodeScript>().ToUnityData());
            }
            ClamFFI.ColorByDistToQuery(nodes, ColorByDistToQuery);

        }

        public void ColorByDistToQuery(ref NodeDataFFI nodeData)
        {
            if (m_Tree.TryGetValue(nodeData.id.AsString, out var nodeObj))
            {
                nodeObj.GetComponent<NodeScript>().distanceToQuery = nodeData.distToQuery;
                //nodeObj.GetComponent<NodeScript>().SetColor(new Color(1.0f , 1.0f, 1.0f));
                nodeObj.GetComponent<NodeScript>().SetColor(new Color(1.0f - nodeData.distToQuery, 1.0f - nodeData.distToQuery, 1.0f - nodeData.distToQuery));
                Debug.Log("setting color by query dist : " + nodeData.distToQuery);
            }
            else
            {
                Debug.LogError("node not found in color by query");
            }
        }

        public void SelectNode(GameObject node)
        {
            node.GetComponent<NodeScript>().Select();
            //Debug.Log("node selected");
            m_SelectedNodes.Add(node);
            ////text.text = node.GetComponent<NodeScript>().distanceToQuery.ToString();

            //NodeWrapper wrapper = new NodeWrapper(node.GetComponent<NodeScript>().ToNodeData());
            //ClamFFI.GetClusterData(wrapper);
            //text.text = wrapper.Data.GetInfo();
            //if (node.GetComponent<NodeScript>().distanceToQuery >= 0.0f)
            //{
            //    text.text += "dist to query: " + node.GetComponent<NodeScript>().distanceToQuery.ToString();
            //}
            //Debug.Log("num selected: " + m_SelectedNodes.Count);




        }

        public void DeSelectNode(GameObject node)
        {
            node.GetComponent<NodeScript>().Deselect();
            m_SelectedNodes.Remove(node);
            text.text = "";
        }

        public void DemoPhysics(float searchRadius)
        {
            //SampleLeafNodes(40);
            SelectAllLeafNodes();
            SelectQueryResults();
            RNN_Test(searchRadius);
            HideUnselectedNodes();
            RandomizeLocations();
            //ClamFFI.InitForceDirectedSim()

        }



        public void DeselectAll()
        {
            m_SelectedNodes.ForEach(node =>
            {
                node.GetComponent<NodeScript>().Deselect();
            });

            m_SelectedNodes.Clear();
            text.text = "";

        }

        public void RandomizeLocations()
        {
            Debug.Log("rand locs");
            foreach (var node in m_SelectedNodes)
            {
                var x = Random.Range(0, 100);
                var y = Random.Range(0, 100);
                var z = Random.Range(0, 100);

                //if (m_Tree.TryGetValue(node.id, out var obj))

                node.GetComponent<Transform>().position = new Vector3(x, y, z);

            }

            foreach (var node in m_SelectedNodes)
            {
                //if (m_Tree.TryGetValue(node.id, out var obj))
                {
                    for (int i = 0; i < node.GetComponent<Transform>().childCount; i++)
                    {
                        GameObject child = node.transform.GetChild(i).gameObject;
                        string[] names = child.name.Split();
                        if (m_Tree.TryGetValue(names[1], out var other))
                        {
                            List<Vector3> pos = new List<Vector3>
                            {
                                node.GetComponent<NodeScript>().GetPosition(),
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
                    SelectNode(entry.Value);
                }
            });
        }

        public void SampleLeafNodes(int numLeaves)
        {

            DeselectAll();
            var rand = new System.Random();
            //m_Tree.OrderBy(x => rand.Next()).ToList().ForEach(entry =>
            //{

            //    if (entry.Value.GetComponent<NodeScript>().GetLeftChildID() == "None" && selected++ <= numLeaves)
            //    {
            //        SelectNode(entry.Value);
            //    }
            //});
            m_Tree.Values.ToList().Where(node => node.GetComponent<NodeScript>().IsLeaf() && m_QueryResults.Contains(node) == false).OrderBy(x => rand.Next()).Take(numLeaves).ToList().ForEach(obj => SelectNode(obj));




        }

        public void HideUnselectedNodes()
        {
            m_Tree.ToList().ForEach(gameObject =>
            {
                var node = gameObject.Value;
                var found = m_SelectedNodes.Find(n => n.GetComponent<NodeScript>().GetId() == node.GetComponent<NodeScript>().GetId());
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

            float distance = global::Clam.ClamFFI.DistanceToOther(m_SelectedNodes[0].GetComponent<NodeScript>().GetId(), m_SelectedNodes[1].GetComponent<NodeScript>().GetId());
            Debug.Log("distance to other " + distance);
            text.text += "distance: " + distance.ToString();
        }

        void Deselect(GameObject node)
        {
            node.GetComponent<NodeScript>().SetColor(m_DefaultColor);
            m_SelectedNodes.Remove(node);
            if (!m_SelectedNodes.Contains(node))
            {
                text.text = "";
            }
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
                    var selectedNode = hitInfo.collider.gameObject;
                    if (m_SelectedNodes.Count > 0)
                    {
                        var existingSelection = m_SelectedNodes.Find(node => node.GetComponent<NodeScript>().GetId() == selectedNode.GetComponent<NodeScript>().GetId());
                        if (existingSelection != null)
                        {
                            //selectedNode.SetColor(m_DefaultColor);
                            //m_SelectedNodes.Remove(existingSelection);
                            Deselect(existingSelection);
                            //selectedNode.GetComponent<NodeScript>().Deselect();

                            //text.text = "";
                            //if (m_SelectedNodes.Count > 0)
                            //{
                            //    text.text = m_SelectedNodes.Last().GetInfo();
                            //}
                            //else
                            //{
                            //    text.text = "";
                            //}
                            return;
                        }
                    }

                    Debug.Log("selexting");

                    global::Clam.NodeWrapper wrapper = new global::Clam.NodeWrapper(selectedNode.GetComponent<NodeScript>().ToNodeData());
                    FFIError found = global::Clam.ClamFFI.GetClusterData(wrapper);
                    if (found == FFIError.Ok)
                    {
                        //m_SelectedNodes.Add(selectedNode);
                        //selectedNode.GetComponent<NodeScript>().SetColor(m_SelectedColor);
                        //text.text = wrapper.Data.GetInfo();
                        SelectNode(selectedNode);
                        //selectedNode.GetComponent<NodeScript>().Deselect();

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



        
    }



}
