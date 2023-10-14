using UnityEngine;

public class LineRendererTest : MonoBehaviour
{
    public int lineCount = 1000; // Adjust the number of lines as needed
    public float lineLength = 1.0f; // Adjust the length of each line segment

    private LineRenderer[] lineRenderers;
    private Material lineMaterial;

    void Start()
    {
        InitializeLineRenderers();
    }

    void InitializeLineRenderers()
    {
        lineRenderers = new LineRenderer[lineCount];
        lineMaterial = new Material(Shader.Find("Standard")); // You can replace "Standard" with your desired shader
        lineMaterial.color = Color.white;
        for (int i = 0; i < lineCount; i++)
        {
            // Create or assign a material for the LineRenderer
            
            GameObject lineObject = new GameObject("LineSegment_" + i);
            lineRenderers[i] = lineObject.AddComponent<LineRenderer>();
            Vector3 startPoint = Random.onUnitSphere * 5f;
            Vector3 endPoint = startPoint + Random.onUnitSphere * lineLength;

            lineRenderers[i].positionCount = 2;
            lineRenderers[i].SetPosition(0, startPoint);
            lineRenderers[i].SetPosition(1, endPoint);

            // Optional: Customize LineRenderer settings
            lineRenderers[i].startWidth = 0.1f;
            lineRenderers[i].endWidth = 0.1f;

            lineRenderers[i].material = lineMaterial;
        }
    }

    void Update()
    {
        //Simulate dynamic movement by updating line positions each frame
        for (int i = 0; i < lineCount; i++)
        {
            Vector3 startPoint = Random.onUnitSphere * 5f;
            Vector3 endPoint = startPoint + Random.onUnitSphere * lineLength;

            lineRenderers[i].SetPosition(0, startPoint);
            lineRenderers[i].SetPosition(1, endPoint);
        }
    }
}