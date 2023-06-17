using System.Collections.Generic;
using UnityEngine;

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

        //ClamFFI.Clam.TraverseTreeDF(SetNodeNames);
        ClamFFI.Clam.TraverseTreeDF2(SetNodeNames2);
        //ClamFFI.Clam.CreateReingoldLayout(Reingoldify);
        ClamFFI.Clam.CreateReingoldLayout2(Reingoldify2);
        //ClamFFI.Clam.TestStringStructComplex();

        //ClamFFI.NodeData testNode = GetNodeData(_nodeName);

        //ClamFFI.Clam.TestStringFn("Asd");
        //ClamFFI.Clam.TestStringStruct();
        //ClamFFI.Clam.TestStringStruct2();
        //ClamFFI.Clam.TestStringStructRustAlloc();
        //ClamFFI.Clam.TestNodeRustAlloc();
        //ClamFFI.Clam.TestNodeRustAlloc2();
        //ClamFFI.Clam.FreeString2();
    }

    //ClamFFI.NodeData GetNodeData(string id)
    //{
    //    GameObject node;

    //    bool hasValue = _tree.TryGetValue(id, out node);
    //    if (hasValue)
    //    {
    //        Debug.Log("here---");
            
    //        ClamFFI.NodeData outNode = ClamFFI.Clam.FindClamData(node.GetComponent<NodeScript>().ToNodeData());
    //        Debug.Log("object searched for name " + outNode.id);
    //        return outNode;
    //    }
    //    else
    //    {
    //        Debug.Log("reingoldify key not found - " + id);
    //    }

    //    return null;
    //}


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
            if (item.activeSelf)
            {
                bool hasLeft = _tree.TryGetValue(item.GetComponent<NodeScript>().GetLeftChildID(), out var leftChild);
                bool hasRight = _tree.TryGetValue(item.GetComponent<NodeScript>().GetRightChildID(), out var rightChild);
                if (hasLeft && hasRight)
                {
                    Debug.DrawLine(item.GetComponent<Transform>().position, leftChild.GetComponent<Transform>().position, Color.black, 2.5f);
                    Debug.DrawLine(item.GetComponent<Transform>().position, rightChild.GetComponent<Transform>().position, Color.white, 2.5f);
                }
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
            RaycastHit hitInfo;

            if (Physics.Raycast(ray.origin, ray.direction * 10, out hitInfo, Mathf.Infinity))
            {
                var objectSelected = hitInfo.collider.gameObject;
                Debug.Log(objectSelected.GetComponent<NodeScript>().GetId() +  " was clicked");
                Debug.Log("searching for node " + objectSelected.GetComponent<NodeScript>().GetId());
                var before = objectSelected.GetComponent<NodeScript>().ToNodeData2();

                //ClamFFI.NodeData nodeData = ClamFFI.Clam.FindClamData(objectSelected.GetComponent<NodeScript>().ToNodeData());
                NewClam.NodeData nodeData = ClamFFI.Clam.FindClamData(objectSelected.GetComponent<NodeScript>().ToNodeData2());
                nodeData.LogInfo();

                nodeData.FreeStrings();
            }
        }
    }

    unsafe void SetNodeNames(ClamFFI.NodeFFI baton)
    {
        ClamFFI.NodeData nodeData = new ClamFFI.NodeData(baton);
        Debug.Log("pos x " + nodeData.pos.x);
        Debug.Log("pos y" + nodeData.pos.y);
        Debug.Log("pos z " + nodeData.pos.z);
        GameObject node = Instantiate(_nodePrefab);
        Debug.Log("adding nod123e " + nodeData.id);
        node.GetComponent<NodeScript>().SetID(nodeData.id);
        node.GetComponent<NodeScript>().SetLeft(nodeData.leftID);
        node.GetComponent<NodeScript>().SetRight(nodeData.rightID);
        _tree.Add(nodeData.id, node);
        _nodeName = nodeData.id;
    }

    unsafe void SetNodeNames2(ref NewClam.NodeData nodeData)
    {
        Debug.Log("pos x " + nodeData.pos.x);
        Debug.Log("pos y" + nodeData.pos.y);
        Debug.Log("pos z " + nodeData.pos.z);
        Debug.Log("adding nod123e " + nodeData.id.AsString);

        Debug.Log("card " + nodeData.cardinality);
        Debug.Log("left " + nodeData.leftID.AsString);
        Debug.Log("right " + nodeData.rightID.AsString);

        GameObject node = Instantiate(_nodePrefab);

        node.GetComponent<NodeScript>().SetID(nodeData.id.AsString);
        node.GetComponent<NodeScript>().SetLeft(nodeData.leftID.AsString);
        node.GetComponent<NodeScript>().SetRight(nodeData.rightID.AsString);
        _tree.Add(nodeData.id.AsString, node);
        _nodeName = nodeData.id.AsString;
    }

    unsafe void Reingoldify(ClamFFI.NodeFFI baton)
    {
        ClamFFI.NodeData nodeData = new ClamFFI.NodeData(baton);
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

    unsafe void Reingoldify2(ref NewClam.NodeData nodeData)
    {
        //ClamFFI.NodeData nodeData = new ClamFFI.NodeData(baton);
        GameObject node;

        bool hasValue = _tree.TryGetValue(nodeData.id.AsString, out node);
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

    unsafe void SetColorBlue(ClamFFI.NodeFFI baton)
    {
        ClamFFI.NodeData nodeData = new ClamFFI.NodeData(baton);
        GameObject node;

        bool hasValue = _tree.TryGetValue(nodeData.id, out node);
        if (hasValue)
        {
            node.GetComponent<NodeScript>().SetColor(new Color(0.0f, 0.0f, 1.0f));
        }
        else
        {
            Debug.Log("reingoldify key not found - " + nodeData.id);
        }
    }


    unsafe void MoveRight(ClamFFI.NodeFFI baton)
    {
        ClamFFI.NodeData nodeData = new ClamFFI.NodeData(baton);
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
