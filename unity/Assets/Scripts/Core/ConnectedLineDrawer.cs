using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectedLineDrawer : MonoBehaviour
{
    private LineRenderer lineRenderer;

    void Start()
    {
        // Create a new mesh
        Mesh mesh = new Mesh();

        // Define the vertices for the two parallel lines
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 1, 1),
            new Vector3(0, 1, 0),
            new Vector3(1, 2, 1)
        };

        // Define the indices to form line segments
        int[] indices = new int[] { 0, 1, 2, 3 };

        // Set the vertices and indices to the mesh
        mesh.vertices = vertices;
        mesh.SetIndices(indices, MeshTopology.Lines, 0);

        // Recalculate bounds (optional, but can be useful for culling)
        mesh.RecalculateBounds();

        // Create a new game object with MeshFilter and MeshRenderer components
        GameObject lineObject = new GameObject("ParallelLinesMesh");
        MeshFilter meshFilter = lineObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = lineObject.AddComponent<MeshRenderer>();

        // Assign the mesh to the MeshFilter
        meshFilter.mesh = mesh;

        // Assign a material to the MeshRenderer
        meshRenderer.material = new Material(Shader.Find("Standard"));

        // You can set additional material properties or adjust the object's transform as needed
    }
    //private LineRenderer lineRenderer;

    //void Start()
    //{
    //    lineRenderer = gameObject.AddComponent<LineRenderer>();
    //    lineRenderer.positionCount = 4;

    //    // Set material and color
    //    lineRenderer.material = new Material(Shader.Find("Standard"));
    //    lineRenderer.material.color = Color.red;

    //    // Set line positions
    //    Vector3[] linePositions = new Vector3[]
    //    {
    //        new Vector3(0, 0, 0),
    //        new Vector3(1, 1, 1),
    //        new Vector3(2, 0, 0),
    //        new Vector3(3, 1, 1)
    //    };

    //    SetLinePositions(linePositions);
    //}

    //void SetLinePositions(Vector3[] positions)
    //{
    //    lineRenderer.SetPositions(positions);
    //}
}
