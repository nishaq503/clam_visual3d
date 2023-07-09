using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class SpringScript : MonoBehaviour
{
    public GameObject nodePrefab;
    private GameObject m_Node1;
    private GameObject m_Node2;

    private LineRenderer m_LineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        m_LineRenderer = GetComponent<LineRenderer>();
        m_LineRenderer.startWidth = 0.1f;
        m_LineRenderer.endWidth = 0.1f;
        m_LineRenderer.useWorldSpace = true;
        //l.startColor = Color.black;
        //l.endColor = Color.black;
        m_LineRenderer.material.color = Color.black;
    }

    // Update is called once per frame
    void Update()
    {
        List<Vector3> positions = new List<Vector3>() { m_Node1.transform.position, m_Node2.transform.position };
        m_LineRenderer.SetPositions(positions.ToArray());
    }

    public void SetNodes(GameObject node1, GameObject node2)
    {
        m_Node1 = node1;
        m_Node2 = node2;
    }
}
