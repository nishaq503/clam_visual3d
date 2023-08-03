using Clam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClamTree : MonoBehaviour
{

    private GameObject m_NodePrefab;
    private GameObject m_SpringPrefab;

    private string m_DataName;
    private uint m_Cardinality;

    private Dictionary<string, GameObject> m_Tree;

    private float m_EdgeScalar = 25.0f;
    private float m_SearchRadius = 0.05f;

    public ClamTree(GameObject nodePrefab, GameObject springPrefab, string dataName, uint cardinality)
    {
        m_NodePrefab = nodePrefab;
        m_SpringPrefab = springPrefab;
        m_DataName = dataName;
        m_Cardinality = cardinality;


        FFIError clam_result = Clam.ClamFFI.InitClam(dataName, cardinality);

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
