using ClamFFI;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using System.Text;
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

    private string _nodeName;

    public void RunGameEngine()
    {
        var super1 = new SuperComplexEntity()
        {
            player_1 = new Vec3
            {
                x = 2,
                y = 4,
                z = 6,
            },

            player_2 = new Vec3
            {
                x = 2,
                y = 4,
                z = 6,
            },

            ammo = 10,
        };
        var node = new ClamFFI.NodeFromClam()
        {
            cardinality = -2,
            argCenter = -2,
            argRadius = -2,
            depth = -2,

        };
        Debug.Log("node card " + node.cardinality);

        var super2 = ClamFFI.Clam.ExampleDoubleEtc(ref node);
        Debug.Log("node card " + node.cardinality);
        Debug.Log("super2 card " + super2.cardinality);
        Debug.Log("super2 x card " + super2.pos.x);
    }


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

        //ClamFFI.Clam.Test();

        Debug.Log("nodename "+ _nodeName);
        //ClamFFI.Clam.GetNodeData(HexStringToBinary(_nodeName));

        //ClamFFI.NodeFromClam testNode = ClamFFI.Clam.GetNodeData(_nodeName);
        //Debug.Log("nodename " + _nodeName);

        //Debug.Log(testNode.id);
        //Debug.Log("Card " + testNode.cardinality);
        RunGameEngine();
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
        _nodeName = nodeData.id;
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


    private static readonly Dictionary<char, string> hexCharacterToBinary = new Dictionary<char, string> {
    { '0', "0000" },
    { '1', "0001" },
    { '2', "0010" },
    { '3', "0011" },
    { '4', "0100" },
    { '5', "0101" },
    { '6', "0110" },
    { '7', "0111" },
    { '8', "1000" },
    { '9', "1001" },
    { 'a', "1010" },
    { 'b', "1011" },
    { 'c', "1100" },
    { 'd', "1101" },
    { 'e', "1110" },
    { 'f', "1111" }
};

    public string HexStringToBinary(string hex)
    {
        StringBuilder result = new StringBuilder();
        foreach (char c in hex)
        {
            // This will crash for non-hex characters. You might want to handle that differently.
            result.Append(hexCharacterToBinary[char.ToLower(c)]);
        }
        return result.ToString();
    }
}
