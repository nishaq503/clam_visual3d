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
    public bool m_IsPhysicsRunning = false;

    //public void Init(GameObject nodePrefab, GameObject springPrefab, string dataName, uint cardinality)
    public FFIError Init()
    {
        //m_NodePrefab = nodePrefab;
        //m_SpringPrefab = springPrefab;
        //m_DataName = dataName;
        //m_Cardinality = cardinality;

        if (m_TreeData.dataName == null || m_TreeData.dataName.Length == 0)
        {
            Debug.Log("error with tree data");
            //Application.Quit();
            return FFIError.HandleInitFailed;

        }

        FFIError clam_result = Clam.ClamFFI.InitClam(m_TreeData.dataName, m_TreeData.cardinality);

        if (clam_result != FFIError.Ok)
        {
            Debug.Log("error with tree data");
            //Application.Quit();
            return clam_result;
        }

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
        Clam.ClamFFI.DrawHeirarchy(PositionUpdater);

        Clam.ClamFFI.ForEachDFT(EdgeDrawer);

        return FFIError.Ok;
    }


    public void EdgeDrawer(ref NodeDataFFI nodeData)
    {
        if (m_Tree.TryGetValue(nodeData.id.AsString, out var node))
        {
            if (node.activeSelf && !node.GetComponent<NodeScript>().IsLeaf())
            {
                if (m_Tree.TryGetValue(node.GetComponent<NodeScript>().GetLeftChildID(), out var lc))
                {
                    var spring = MenuEventManager.instance.MyInstantiate(m_SpringPrefab);
                    //var sprint = SpringScript.CreateInstance(node, lc, SpringScript.SpringType.heirarchal);
                    //var spring = MenuEventManager.instance.MyInstantiate(m_SpringPrefab);

                    spring.GetComponent<SpringScript>().InitLineRenderer(node, lc, SpringScript.SpringType.heirarchal);

                    //spring.GetComponent<SpringScript>().SetNodes(node, lc);
                    //spring.GetComponent<SpringScript>().SetColor(Color.white);

                }

                if (m_Tree.TryGetValue(node.GetComponent<NodeScript>().GetRightChildID(), out var rc))
                {
                    var spring = MenuEventManager.instance.MyInstantiate(m_SpringPrefab);

                    spring.GetComponent<SpringScript>().InitLineRenderer(node, rc, SpringScript.SpringType.heirarchal);

                    //spring.GetComponent<SpringScript>().SetNodes(node, rc);
                    //var sprint = SpringScript.CreateInstance(node, lc, SpringScript.SpringType.Similarity);

                    //spring.GetComponent<SpringScript>().SetColor(Color.white);
                }
            }
        }
    }
    public Dictionary<string, GameObject> GetTree()
    {
        return m_Tree;
    }


    public void Update()
    {
        //if (m_IsPhysicsRunning)
        //{
        //    //if (ClamFFI.PhysicsUpdateAsync() == FFIError.PhysicsFinished)
        //    //{
        //    //    m_IsPhysicsRunning = false;
        //    //    print("physics finished");
        //    //}
        //    //if (m_PhysicsIter < m_MaxPhysicsIters)
        //    //{
        //    //    ApplyForces();
        //    //    m_PhysicsIter++;
        //    //}
        //    //else
        //    //{
        //    //    ClamFFI.ShutdownPhysics();
        //    //    print("finished sim");
        //    //    m_IsPhysicsRunning = false;
        //    //}
        //}
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



    unsafe void PositionUpdater(ref Clam.NodeDataFFI nodeData)
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
