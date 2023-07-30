using Clam;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MainView : MonoBehaviour
{
    [SerializeField]
    VisualTreeAsset m_ListEntryTemplate;

    private UIDocument m_UIDocument;

    VisualTreeAsset m_VisualAsset;

    //TextField m_SelectedClusterInfo;
    Label m_ClusterInfo;
    Label m_ClusterInfoLabel;

    private List<GameObject> m_SelectedClusters;

    void OnEnable()
    {

        m_SelectedClusters = new List<GameObject>();
        m_UIDocument = GetComponent<UIDocument>();

        m_ClusterInfo = m_UIDocument.rootVisualElement.Q<Label>("ClusterInfo");
        m_ClusterInfoLabel = m_UIDocument.rootVisualElement.Q<Label>("ClusterInfoLabel");

        InitClusterInfoLabel();
       
    }

    public void InitClusterInfoLabel()
    {
        StringBuilder stringBuilder = new StringBuilder();
        //stringBuilder.AppendLine("Selected Cluster");

        stringBuilder.AppendLine("id: ");
        stringBuilder.AppendLine("depth: ");
        stringBuilder.AppendLine("card: ");
        stringBuilder.AppendLine("radius: ");
        stringBuilder.AppendLine("lfd: ");
        stringBuilder.AppendLine("argC: ");
        stringBuilder.AppendLine("argR: ");

        //m_SelectedClusterInfo.value = stringBuilder.ToString();
        m_ClusterInfoLabel.text = stringBuilder.ToString();

    }

    public void SetSelectedClusterInfo(string value)
    {     
        m_ClusterInfo.text = value;
    }

    public void OnLMC(InputValue value)
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

        m_ClusterInfo.text = wrapper.Data.GetInfoForUI();
        if (cluster.GetComponent<NodeScript>().distanceToQuery >= 0.0f)
        {
            m_ClusterInfo.text += "dist to query: " + cluster.GetComponent<NodeScript>().distanceToQuery.ToString();
        }
        Debug.Log("num selected: " + m_SelectedClusters.Count);

    }

    public void DeSelectNode(GameObject node)
    {
        node.GetComponent<NodeScript>().Deselect();
        m_SelectedClusters.Remove(node);
        m_ClusterInfo.text = "";
    }



}