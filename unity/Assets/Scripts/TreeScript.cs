using ClamFFI;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using System.Text;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Assertions;
using UnityEngine.Video;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.InputSystem.HID.HID;
using System.Xml;
using UnityEditor.SearchService;

public class TreeScript : MonoBehaviour
{
    public string dataName = "arrhythmia";
    public uint cardinality = 50;

    public GameObject _nodePrefab;
    private Dictionary<string, GameObject> _tree;

    private string _nodeName;

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

        ClamFFI.Node testNode = GetNodeData(_nodeName);
        //ClamFFI.Node testNode = ClamFFI.Clam.GetNodeData2(_nodeName);
        Debug.Log("nodename " + _nodeName);

        Debug.Log(testNode.id);
        Debug.Log("Card " + testNode.cardinality);
        Debug.Log("Card " + testNode.argCenter);
        Debug.Log("Card " + testNode.argRadius);
        Debug.Log("Card " + testNode.depth);
        //RunGameEngine();
    }

    ClamFFI.Node GetNodeData(string id)
    {
        GameObject node;

        bool hasValue = _tree.TryGetValue(id, out node);
        if (hasValue)
        {
            //var script = node.GetComponent<NodeScript>();
            
            //NodeBaton2 baton = new NodeBaton2(node.GetComponent<NodeScript>().ToNodeData());
            Debug.Log("here---");
            
            ClamFFI.Node outNode = ClamFFI.Clam.GetNodeData3(node.GetComponent<NodeScript>().ToNodeData());
            Debug.Log("object searched for name " + outNode.id);
            return outNode;
        }
        else
        {
            Debug.Log("reingoldify key not found - " + id);
        }

        return null;
    }


    void FixedUpdate()
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
        //foreach (KeyValuePair<string, GameObject> entry in _tree)
        //{
        //    // do something with entry.Value or entry.Key
        //    Debug.DrawLine(Vector3.zero, new Vector3(5, 0, 0), Color.white, 2.5f);
        //}

        foreach (var item in _tree.Values)
        {
            bool hasLeft = _tree.TryGetValue(item.GetComponent<NodeScript>().GetLeftChildID(), out var leftChild);
            bool hasRight = _tree.TryGetValue(item.GetComponent<NodeScript>().GetRightChildID(), out var rightChild);
            if(hasLeft && hasRight)
            {
                Debug.DrawLine(item.GetComponent<Transform>().position, leftChild.GetComponent<Transform>().position, Color.black, 2.5f);
                Debug.DrawLine(item.GetComponent<Transform>().position, rightChild.GetComponent<Transform>().position, Color.white, 2.5f);
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("lmb was pressed");

            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            //if (Physics.Raycast(ray, out RaycastHit hit))
            //{
            //    // Use the hit variable to determine what was clicked on.
            //    Debug.Log("something was pressed");
            //    Debug.Log(hit.colliderInstanceID);
            //}

            RaycastHit hitInfo;

            if (Physics.Raycast(ray.origin, ray.direction * 10, out hitInfo, Mathf.Infinity))
            {
                var objectSelected = hitInfo.collider.gameObject;
                Debug.Log(objectSelected.GetComponent<NodeScript>().GetId() +  " was clicked");
                Debug.Log("name in binary " + ClamFFI.Clam.HexStringToBinary(objectSelected.GetComponent<NodeScript>().GetId()));
            }
        }
    }

    unsafe void SetNodeNames(ClamFFI.NodeBaton baton)
    {
        //Debug.Log("hello");
        ClamFFI.Node nodeData = new ClamFFI.Node(baton);
        Debug.Log("pos x " + nodeData.pos.x);
        Debug.Log("pos y" + nodeData.pos.y);
        Debug.Log("pos z " + nodeData.pos.z);
        //Debug.Log("id " + nodeData.id);
        GameObject node = Instantiate(_nodePrefab);
        Debug.Log("adding nod123e " + nodeData.id);
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
            node.GetComponent<NodeScript>().SetColor(new Color(0.0f, 0.0f, 1.0f));
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
