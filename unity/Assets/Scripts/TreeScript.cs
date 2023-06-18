using System.Collections.Generic;
using UnityEngine;

using TMPro;
using System;

public class TreeScript : MonoBehaviour
{
    public string dataName = "arrhythmia";
    public uint cardinality = 50;

    public GameObject m_NodePrefab;
    private Dictionary<string, GameObject> m_Tree;

    public TMP_Text text;

    private GameObject m_SelectedNode;
    private Color m_SelectedNodeActualColor;

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

    }

    void FixedUpdate()
    {
        foreach (var item in m_Tree.Values)
        {
            if (item.activeSelf)
            {
                bool hasLeft = m_Tree.TryGetValue(item.GetComponent<NodeScript>().GetLeftChildID(), out var leftChild);
                bool hasRight = m_Tree.TryGetValue(item.GetComponent<NodeScript>().GetRightChildID(), out var rightChild);
                if (hasLeft && hasRight)
                {
                    Debug.DrawLine(item.GetComponent<Transform>().position, leftChild.GetComponent<Transform>().position, Color.black, 2.5f);
                    Debug.DrawLine(item.GetComponent<Transform>().position, rightChild.GetComponent<Transform>().position, Color.white, 2.5f);
                }
            }
        }
    }

    void Update()
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

                ClamFFI.Clam.GetClusterData(nodeWrapper);
                //ClamFFI.NodeData nodeData = ClamFFI.Clam.FindClamData(objectSelected.GetComponent<NodeScript>().ToNodeData());
                nodeWrapper.Data.LogInfo();

                text.text = nodeWrapper.Data.GetInfo();
            }
        }
    }

    unsafe void SetNodeNames(ref ClamFFI.NodeData nodeData)
    {
        Debug.Log("pos x " + nodeData.pos.x);
        Debug.Log("pos y" + nodeData.pos.y);
        Debug.Log("pos z " + nodeData.pos.z);
        Debug.Log("adding nod123e " + nodeData.id.AsString);

        Debug.Log("card " + nodeData.cardinality);
        Debug.Log("left " + nodeData.leftID.AsString);
        Debug.Log("right " + nodeData.rightID.AsString);

        GameObject node = Instantiate(m_NodePrefab);

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
