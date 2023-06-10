using ClamFFI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Video;
using static UnityEngine.Rendering.DebugUI;

public class TreeScript : MonoBehaviour
{
    public string dataName = "arrhythmia";
    public uint cardinality = 50;

    public GameObject _nodePrefab;
    private Dictionary<string, GameObject> _tree;

    void Start()
    {
        int clam_result = ClamFFI.Clam.InitClam(dataName, cardinality);
        if (clam_result == 0)
        {
            Debug.Log(System.String.Format("Error: tree for {0} not created. Check debug log file.", dataName));
            return;
        }
        _tree = new Dictionary<string, GameObject>();

        int numNodes = ClamFFI.Clam.GetNumNodes();
        Debug.Log(System.String.Format("created tree with num nodes {0}.", numNodes));

        ClamFFI.Clam.TraverseTreeDF(SetNodeNames);
        ClamFFI.Clam.CreateReingoldLayout(Reingoldify);

    }

    void Update()
    {
        //if(Input.GetKey(KeyCode.Space))
        //{
        //    ClamFFI.Clam.TraverseTreeDF(SetColorBlue);

        //    //node.GetComponent<NodeScript>().SetPosition(nodeData.pos);

        //}
        //if(_timer> 1000)
        //{
        //ClamFFI.Clam.TraverseTreeDF(MoveRight);
        //    _timer = 0;
        //}
        //_timer += 1;

    }

    unsafe void SetNodeNames(ClamFFI.NodeBaton baton)
    {
        //Debug.Log("hello");
        ClamFFI.Node nodeData = new ClamFFI.Node(baton);
        Debug.Log("x " + nodeData.pos.x);
        Debug.Log("y" + nodeData.pos.y);
        Debug.Log("z " + nodeData.pos.z);
        //Debug.Log("id " + nodeData.id);
        GameObject node = Instantiate(_nodePrefab);
        Debug.Log("adding node " + nodeData.id);
        node.GetComponent<NodeScript>().SetID(nodeData.id);
        node.GetComponent<NodeScript>().SetLeft(nodeData.leftID);
        node.GetComponent<NodeScript>().SetRight(nodeData.rightID);
        _tree.Add(nodeData.id, node);
    }

    unsafe void Reingoldify(ClamFFI.NodeBaton baton)
    {
        ClamFFI.Node nodeData = new ClamFFI.Node(baton);
        GameObject node;

        bool hasValue = _tree.TryGetValue(nodeData.id, out node);
        if (hasValue)
        {
            node.GetComponent<NodeScript>().SetColor(nodeData.color);
            node.GetComponent<NodeScript>().SetPosition(nodeData.pos);
        }
        else
        {
            Debug.Log("reingoldify key not found - " + nodeData.id);
        }
    }

    unsafe void SetColorBlue(ClamFFI.NodeBaton baton)
    {
        ClamFFI.Node nodeData = new ClamFFI.Node(baton);
        GameObject node;

        bool hasValue = _tree.TryGetValue(nodeData.id, out node);
        if (hasValue)
        {
            node.GetComponent<NodeScript>().SetColor(new Vector3(0.0f, 0.0f, 1.0f));
            //node.GetComponent<NodeScript>().SetPosition(nodeData.pos);
        }
        else
        {
            Debug.Log("reingoldify key not found - " + nodeData.id);
        }
    }


    unsafe void MoveRight(ClamFFI.NodeBaton baton)
    {
        ClamFFI.Node nodeData = new ClamFFI.Node(baton);
        GameObject node;

        bool hasValue = _tree.TryGetValue(nodeData.id, out node);
        if (hasValue)
        {
            //node.GetComponent<NodeScript>().SetColor(new Vector3(0.0f, 0.0f, 1.0f));
            //var curPos = node.GetComponent<Transform>().position;
            //node.GetComponent<NodeScript>().SetPosition(new Vector3(curPos.x + 0.1f, curPos.y,curPos.z));

            var pos = node.GetComponent<Transform>().position;
            node.GetComponent<Transform>().position = new Vector3(pos.x + 0.01f, pos.y, pos.z);
        }
        else
        {
            Debug.Log("reingoldify key not found - " + nodeData.id);
        }
    }
}
