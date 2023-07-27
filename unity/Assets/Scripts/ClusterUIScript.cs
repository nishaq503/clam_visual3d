using Clam;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ClusterUIScript : MonoBehaviour
{

    public TMP_Text ClusterText;
    public Canvas ClusterCanvas;
    public Image ClusterBackground;

    private List<GameObject> m_SelectedClusters;

    // Start is called before the first frame update
    void Start()
    {
        m_SelectedClusters = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnLMC()
    {
        Debug.Log("cluster ui script lmc");
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray.origin, ray.direction * 10, out hitInfo, Mathf.Infinity))
        {
            var selectedNode = hitInfo.collider.gameObject;
            if (selectedNode.GetComponent<NodeScript>().IsSelected())
            {
                DeSelectNode(selectedNode);
                return;
            }
            //if (m_SelectedClusters.Count > 0)
            //{
            //    var existingSelection = m_SelectedClusters.Find(node => node.GetComponent<NodeScript>().GetId() == selectedNode.GetComponent<NodeScript>().GetId());
            //    if (existingSelection != null)
            //    {
            //        //selectedNode.SetColor(m_DefaultColor);
            //        //m_SelectedNodes.Remove(existingSelection);
            //        //DeSelectNode(existingSelection);
            //        //selectedNode.GetComponent<NodeScript>().Deselect();

            //        //text.text = "";
            //        //if (m_SelectedNodes.Count > 0)
            //        //{
            //        //    text.text = m_SelectedNodes.Last().GetInfo();
            //        //}
            //        //else
            //        //{
            //        //    text.text = "";
            //        //}
            //        return;
            //    }
            //}

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

    public void SelectNode(GameObject cluster)
    {
        cluster.GetComponent<NodeScript>().Select();
        m_SelectedClusters.Add(cluster);
        //text.text = node.GetComponent<NodeScript>().distanceToQuery.ToString();

        NodeWrapper wrapper = new NodeWrapper(cluster.GetComponent<NodeScript>().ToNodeData());
        ClamFFI.GetClusterData(wrapper);
        ClusterText.text = wrapper.Data.GetInfo();
        if (cluster.GetComponent<NodeScript>().distanceToQuery >= 0.0f)
        {
            ClusterText.text += "dist to query: " + cluster.GetComponent<NodeScript>().distanceToQuery.ToString();
        }
        Debug.Log("num selected: " + m_SelectedClusters.Count);

    }

    public void DeSelectNode(GameObject node)
    {
        node.GetComponent<NodeScript>().Deselect();
        m_SelectedClusters.Remove(node);
        ClusterText.text = "";
    }
}
