using Clam;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class GraphBuilder : MonoBehaviour
{
    private Vector3[] m_Vertices;
    private int[] m_Indices;
    private int m_VertexCounter;
    private int m_IndexCounter;
    private bool m_IsPhysicsRunning;

    int m_ShouldUpdate = 0;
    //private bool m_InitializedIndices;
    //private bool m_UpdatedAllVertices;

    // Start is called before the first frame update
    void Start()
    {
        m_VertexCounter = 0;
        m_IndexCounter = 0;
        //m_IsPhysicsRunning = false;
        //lineMesh = new Mesh();
        //GetComponent<MeshFilter>().mesh = lineMesh;

        Debug.Log("physics is running start val " + m_IsPhysicsRunning);

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("graph builder update");
        Debug.Log("physics is running updatet val " + m_IsPhysicsRunning);

        //m_VertexCounter = 0;
        if (m_IsPhysicsRunning)
        {
            Debug.Log("graph builder updatephysics is runningf");

            if (Clam.FFI.NativeMethods.PhysicsUpdateAsync(PositionUpdater) == FFIError.PhysicsFinished)
            {
                m_IsPhysicsRunning = false;
                print("physics finished");
            }
        }

    }

    public void Init(Clam.FFI.ClusterData[] nodes, float edgeScalar, int numIters)
    {
        GetComponent<MeshFilter>().mesh = new Mesh();

        Clam.FFI.NativeMethods.InitForceDirectedSim(nodes, edgeScalar, numIters, EdgeDrawer);

        m_VertexCounter = 0;
        m_IndexCounter = 0;

        //GetComponent<MeshFilter>().mesh = new Mesh();

        int numNodes = Cakes.Tree.GetTree().Count;
        int numEdges = Clam.FFI.NativeMethods.GetNumEdgesInGraph();
        m_Vertices = new Vector3[numNodes];
        m_Indices = new int[numEdges * 2];
        InitNodeIndices();
        Debug.Log("num edges in graph : " + numEdges + ", num nodes " + numNodes);
        Clam.FFI.NativeMethods.RunForceDirectedSim(nodes, edgeScalar, numIters, EdgeDrawer);



        for (int K = 0; K < nodes.Length; K++)
        {
            //ref var node = ref node1;
            //Debug.Log("freeing all nodes from physics sim");
            Clam.FFI.NativeMethods.DeleteClusterData(ref nodes[K]);
        }
        //Clam.FFI.NativeMethods.RunForceDirectedSim(nodes, edgeScalar, numIters, EdgeDrawer);

        m_IsPhysicsRunning = true;

        Debug.Log("physics is runninginit val " + m_IsPhysicsRunning);

    }

    void InitNodeIndices()
    {
        int i = 0;
        foreach (var (id, node) in Cakes.Tree.GetTree())
        {
            node.GetComponent<Node>().IndexBufferID = i;
            m_Vertices[i] = node.GetComponent<Node>().GetPosition();
            i++;
        }

        var mesh = GetComponent<MeshFilter>().mesh;
        mesh.vertices = m_Vertices;
        mesh.RecalculateBounds();
    }

    public void PositionUpdater(ref Clam.FFI.ClusterData nodeData)
    {
        
        Debug.Log("graph builder update");
        string id = nodeData.id.AsString;
        //Debug.Log("id of updated node is " + id);
        if (Cakes.Tree.GetTree().TryGetValue(id, out var node))
        {
            node.GetComponent<Node>().SetPosition(nodeData.pos.AsVector3);

            int numNodes = Cakes.Tree.GetTree().Count;
            

            m_Vertices[node.GetComponent<Node>().IndexBufferID] = node.GetComponent<Node>().GetPosition();
            m_VertexCounter++;

            if (m_VertexCounter == m_Vertices.Length)
            {
                //m_UpdatedAllVertices = true;
                var mesh = GetComponent<MeshFilter>().mesh;
                mesh.vertices = m_Vertices;
                mesh.RecalculateBounds();
                m_VertexCounter = 0;

                Debug.Log("set all line vertices in update");
            }
        }
        else
        {
            Debug.Log("physics upodate key not found - " + id);
        }
    }

    public void EdgeDrawer(ref Clam.FFI.ClusterData nodeData)
    {
        
        Debug.Log("graph builder edge drawer");

        if (Cakes.Tree.GetTree().TryGetValue(nodeData.id.AsString, out var node))
        {
            if (Cakes.Tree.GetTree().TryGetValue(nodeData.message.AsString, out var other))
            {
                var id1 = node.GetComponent<Node>().IndexBufferID;
                var id2 = other.GetComponent<Node>().IndexBufferID;
                m_Indices[m_IndexCounter++] = id1;
                m_Indices[m_IndexCounter++] = id2;
                //m_Vertices[id1] = node.ge;
                //m_Vertices[id2] = nodeData.pos.AsVector3;
                if (m_IndexCounter == m_Indices.Length)
                {
                    //m_InitializedIndices = true;
                    GetComponent<MeshFilter>().mesh.SetIndices(m_Indices, MeshTopology.Lines, 0);
                    //m_Indices = null;
                    Debug.Log("set indices in edge drawer");
                }
            }
        }
        Debug.Log("graph builder edge drawer should never hit this");

    }
}
