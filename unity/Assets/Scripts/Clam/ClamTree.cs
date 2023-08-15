using Clam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClamTree : MonoBehaviour
{

    public GameObject m_NodePrefab;
    public GameObject m_SpringPrefab;

    private string m_DataName;
    private uint m_Cardinality;

    public ClamTreeData m_TreeData;

    private Dictionary<string, GameObject> m_Tree;

    private float m_EdgeScalar = 25.0f;
    private float m_SearchRadius = 0.05f;

    //public void Init(GameObject nodePrefab, GameObject springPrefab, string dataName, uint cardinality)
    public void Init()
    {
        //m_NodePrefab = nodePrefab;
        //m_SpringPrefab = springPrefab;
        //m_DataName = dataName;
        //m_Cardinality = cardinality;

        if (m_TreeData.dataName == null || m_TreeData.dataName.Length == 0)
        {
            Debug.Log("error with tree data");
            Application.Quit();
            
        }

        FFIError clam_result = Clam.ClamFFI.InitClam(m_TreeData.dataName, m_TreeData.cardinality);

        m_Tree = new Dictionary<string, GameObject>();

        int numNodes = Clam.ClamFFI.GetNumNodes();
        Debug.Log(System.String.Format("created tree with num nodes {0}.", numNodes));

        FFIError e = Clam.ClamFFI.ForEachDFT(SetNodeNames);

        if (e == FFIError.Ok)
        {
            Debug.Log("ok)");
        }
        else
        {
            Debug.Log("ERROR " + e);
        }
        Clam.ClamFFI.CreateReingoldLayout(Reingoldify);
    }

    public Dictionary<string, GameObject> GetTree()
    {
        return m_Tree;
    }

    unsafe void SetNodeNames(ref Clam.NodeDataFFI nodeData)
    {
        GameObject node = Instantiate(m_NodePrefab);
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
