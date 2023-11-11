using Clam;
using Clam.FFI;
using System.Linq;
using UnityEngine;

public class TreeLayout
{
    string m_RootID;
    int m_RootDepth;
    int m_CurrentDepth;
    int m_MaxDepth;
    int m_IntervalStep = 2;
    // depth given to reingold will be max-rootDepth

    public int CurrentDepth()
    {
        return m_CurrentDepth;
    }

    public TreeLayout(string rootId)
    {
        m_RootID = rootId;
       
        var dataWrapper = NativeMethods.CreateClusterDataWrapper(rootId);
        m_RootDepth = dataWrapper.Data.depth;

        Debug.Log("root depth: " + m_RootDepth);

        m_CurrentDepth = m_RootDepth;
        m_MaxDepth = m_CurrentDepth + m_IntervalStep;

        NativeMethods.DrawHierarchyOffsetFrom(Clam.FFI.NativeMethods.CreateClusterDataWrapper(m_RootID), UpdatePositionCallback, m_RootDepth, m_CurrentDepth, m_MaxDepth);

        NativeMethods.ForEachDFT(ClusterVisibilityCallback, m_RootID);
        NativeMethods.ForEachDFT(EdgeVisibilityCallback);
    }

    public void ShowMore()
    {
        int nextDepth = m_CurrentDepth + 1;
        if (nextDepth > NativeMethods.TreeHeight())
        {
            Debug.Log("Tree layout height is at max");
            return;
        }
        m_CurrentDepth++;

        if (m_CurrentDepth > m_MaxDepth)
        {
            Debug.Log("Increasing size");
            m_MaxDepth += m_IntervalStep;
            NativeMethods.DrawHierarchyOffsetFrom(Clam.FFI.NativeMethods.CreateClusterDataWrapper(m_RootID), UpdatePositionCallback, m_RootDepth, m_CurrentDepth, m_MaxDepth);
        }

        UpdateNodeVisibility(nextDepth);
    }
    public void ShowLess()
    {
        int nextDepth = m_CurrentDepth - 1;

        if (nextDepth <= 0)
        {
            Debug.Log("Tree height is at min");
            return;
        }
        m_CurrentDepth--;

        if (m_MaxDepth - m_CurrentDepth > m_IntervalStep * 1.5)
        {
            Debug.Log("Decreasing size");

            m_MaxDepth = m_CurrentDepth + m_IntervalStep;
            NativeMethods.DrawHierarchyOffsetFrom(Clam.FFI.NativeMethods.CreateClusterDataWrapper(m_RootID), UpdatePositionCallback, m_RootDepth, m_CurrentDepth, m_MaxDepth);
        }

        UpdateNodeVisibility(nextDepth);
    }

    void GrowTree()
    {
        m_MaxDepth += m_IntervalStep;
        //Resize()
    }

    void ShrinkTree()
    {
        m_MaxDepth -= m_IntervalStep;
        // resize
    }

    void UpdateNodeVisibility(int newDepth)
    {
        //NativeMethods.DrawHierarchyOffsetFrom(Clam.FFI.NativeMethods.CreateClusterDataWrapper(m_RootID), UpdatePositionCallback, m_RootDepth, m_MaxDepth);

        NativeMethods.ForEachDFT(ClusterVisibilityCallback, m_RootID);
        NativeMethods.ForEachDFT(EdgeVisibilityCallback);
        //if (newDepth == m_RootDepth)
        //{
        //    // destroy this object
        //}
        //if (newDepth > m_CurrentDepth)
        //{
        //    //if (newDepth > m_MaxDepth)
        //    //{
        //    //    m_CurrentDepth = newDepth;
        //    //    GrowTree();
        //    //}
        //    // traverse children and make active up to newDepth
        //    NativeMethods.DrawHierarchyOffsetFrom(Clam.FFI.NativeMethods.CreateClusterDataWrapper(m_RootID), VisibilityCallback, m_RootDepth, m_CurrentDepth);
        //}
        //else if (newDepth < m_CurrentDepth)
        //{
        //    //if (newDepth < m_MaxDepth - m_IntervalStep)
        //    //{
        //    //    m_CurrentDepth = newDepth;

        //    //    ShrinkTree();
        //    //}
        //    NativeMethods.DrawHierarchyOffsetFrom(Clam.FFI.NativeMethods.CreateClusterDataWrapper(m_RootID), VisibilityCallback, m_RootDepth, m_CurrentDepth);

        //    // traverse children above current depth and make inactive
        //}
        //else
        //{
        //    Debug.Log("?? tree size change");
        //    return;
        //}
    }

    void UpdatePositionCallback(ref ClusterData data)
    {
        var id = data.id.AsString;
        GameObject clusterObject = Cakes.Tree.GetOrAdd(id);
        Debug.Log("visibility callback");

        clusterObject.GetComponent<Clam.Node>().SetPosition(data.pos.AsVector3);
        //Debug.Log("node depth: " + data.depth + ", cur depth: " + m_CurrentDepth);
        //if (data.depth <= m_CurrentDepth)
        //{
        //    Debug.Log("set active");
        //    clusterObject.SetActive(true);
        //}
        //else
        //{
        //    Debug.Log("set inactive");

        //    clusterObject.SetActive(false);
        //}
    }

    void ClusterVisibilityCallback(ref ClusterData data)
    {
        var id = data.id.AsString;
        GameObject clusterObject = Cakes.Tree.GetOrAdd(id);
        if (data.depth <= m_CurrentDepth)
        {
            clusterObject.SetActive(true);
        }
        else
        {
            clusterObject.SetActive(false);
        }
    }

    public void EdgeVisibilityCallback(ref Clam.FFI.ClusterData nodeData)
    {
        if (Cakes.Tree.GetTree().TryGetValue(nodeData.id.AsString, out var node))
        {
            if (node.activeSelf && !node.GetComponent<Node>().IsLeaf())
            {
                if (Cakes.Tree.GetTree().TryGetValue(node.GetComponent<Node>().GetLeftChildID(), out var lc))
                {
                    if (lc.activeSelf)
                    {
                        var edges = GameObject.FindObjectsOfType<Edge>(true).Where(e => e.name == node.GetComponent<Node>().GetId() + lc.GetComponent<Node>().GetId()).ToArray();
                        if (edges.Count() != 0)
                        {
                            var edge = edges[0];
                            if (!edge.gameObject.activeSelf)
                            {

                                Debug.Log("setting left edge acgive");
                                edge.gameObject.SetActive(true);
                            }
                        }
                    }
                }

                if (Cakes.Tree.GetTree().TryGetValue(node.GetComponent<Node>().GetRightChildID(), out var rc))
                {
                    if (rc.activeSelf)
                    {
                        var edge = System.Array.Find(GameObject.FindObjectsOfType<Edge>(true), e => e.name == (node.GetComponent<Node>().GetId() + rc.GetComponent<Node>().GetId()));
                        if (edge != null)
                        {
                            if (!edge.gameObject.activeSelf)
                            {
                                edge.gameObject.SetActive(true);
                            }
                        }
                    }
                }
            }
        }
    }




    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
