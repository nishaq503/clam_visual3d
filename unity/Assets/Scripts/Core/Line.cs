using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    //public Mesh m_Mesh;
    Mesh mesh;
    MeshFilter meshFilter;
    //Vector3[] newVertices;
    //int[] newTriangles;

    //void Start()
    //{
    //    Vector3[] verts = new Vector3[] { Vector3.up, Vector3.right, Vector3.down, Vector3.left };
    //    //int[] indicesForLineStrip = new int[] { 0, 1, 2, 3, 0 };
    //    int[] indicesForLines = new int[] { 0, 1, 1, 2, 2, 3, 3, 0 };
    //    Mesh mesh = new Mesh();
    //    mesh.vertices = verts;
    //    //mesh.SetIndices(indicesForLineStrip, MeshTopology.LineStrip, 0);
    //    mesh.SetIndices(indicesForLines, MeshTopology.Lines, 0);
    //    mesh.RecalculateNormals();
    //    mesh.RecalculateBounds();
    //    //Mesh mesh = new Mesh();
    //    //GetComponent<MeshFilter>().mesh = mesh;

    //    mesh = new Mesh();
    //    meshFilter = gameObject.GetComponent<MeshFilter>();
    //    meshFilter.mesh = mesh;


    //    //We assign our vertices and triangles to the mesh.
    //    mesh.vertices = newVertices;
    //    mesh.triangles = newTriangles;


    //}
    //private void Start()
    //{
    //    triangle();
    //}

    Vector3[] newVertices;
    Vector2[] newUV;
    int[] newTriangles;
    void triangle()
    {
        newVertices = new Vector3[4];

        newVertices[0] = new Vector3(0, 0, 0);
        newVertices[1] = new Vector3(1, 0, 0);
        newVertices[2] = new Vector3(0, 1, 0);
        newVertices[3] = new Vector3(0, 0, 1);


        newTriangles = new int[12];

        newTriangles[0] = 0;
        newTriangles[1] = 2;
        newTriangles[2] = 1;

        newTriangles[3] = 0;
        newTriangles[4] = 1;
        newTriangles[5] = 3;

        newTriangles[6] = 0;
        newTriangles[7] = 3;
        newTriangles[8] = 2;

        newTriangles[9] = 1;
        newTriangles[10] = 2;
        


        //We instantiate our mesh object and attach it to our mesh filter
        mesh = new Mesh();
        meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;


        //We assign our vertices and triangles to the mesh.
        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;
    }
    

    void Update()
    {
        //Mesh mesh = GetComponent<MeshFilter>().mesh;
        //Vector3[] vertices = mesh.vertices;
        //Vector3[] normals = mesh.normals;

        //for (var i = 0; i < vertices.Length; i++)
        //{
        //    vertices[i] += normals[i] * Mathf.Sin(Time.time);
        //}

        //mesh.vertices = vertices;
    }

    //Start is called before the first frame update
    void Start()
    {
        //var mesh = this.gameObject.AddComponent<Mesh>();
        //m_Mesh = Instantiate<Mesh>();
        //m_Mesh.SetIndices()
        //var mesh = new Mesh();
        //Vector3[] verts = new Vector3[] { new Vector3(0,0,0), new Vector3(0,-10,-5) };
        //int[] indicesForLineStrip = new int[] { 0, 1,1,2,2,1, ,3};
        //int[] indicesForLineStrip = new int[] { 0, 1, 2, 3, 1 };
        //Mesh mesh = new Mesh();
        //mesh.SetIndices(indicesForLines, MeshTopology.Lines, 0);
        int[] indicesForLineStrip = new int[] { 0, 1, 1, 2, 2, 3, 3, 1 };
        mesh = new Mesh();
        Vector3[] verts = new Vector3[] { Vector3.up, Vector3.right, Vector3.down, Vector3.left};
        int[] indicesForLines = new int[] { 0, 1, 1, 2, 2, 3, 3, 0 };
        mesh.vertices = verts;
        mesh.SetIndices(indicesForLines, MeshTopology.Lines, 0);
        //mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;


        //We assign our vertices and triangles to the mesh.
        //mesh.vertices = newVertices;
        //mesh.triangles = newTriangles;

    }

    // Update is called once per frame
    //void Update()
    //{

    //}
}
