using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using static UnityEngine.Rendering.DebugUI;

public class TreeScript : MonoBehaviour
{
    public string dataName = "arrhythmia";
    public uint cardinality = 50;

    public GameObject _nodePrefab;
    private Dictionary<string, GameObject> _tree;
    float _test = 0.0f;
    List<float> test_values;
    // Start is called before the first frame update
    void Start()
    {
        test_values = new List<float>();
        int clam_result = ClamFFI.Clam.InitClam(dataName, cardinality);
        if (clam_result == 0)
        {
            Debug.Log(System.String.Format("Error: tree for {0} not created. Check debug log file.", dataName));
            return;
        }
        _tree = new Dictionary<string, GameObject>();

        int numNodes = ClamFFI.Clam.GetNumNodes();
        Debug.Log(System.String.Format("created tree with num nodes {0}.", numNodes));


        unsafe
        {
            int isOk = ClamFFI.Clam.InitNodeObjects(InitNode);
            Debug.Log("nodes initalized ok? " + isOk);
            int layoutInitialized = ClamFFI.Clam.CreateReingoldLayout(reingoldify);
            Debug.Log("layout initialized ? " + layoutInitialized);
        }




       //ClamFFI.Clam.FreeReingoldLayout();

        //for(int i =0; i < numNodes; i++)
        //{
        //    GameObject node = Instantiate(_nodePrefab);
        //    //node.GetComponent<NodeScript>().SetPosition();
        //}

        //TestCallBack();

    }

    unsafe void TestCallBack()
    {
        float r2 = ClamFFI.Clam.retcall(pow);
        Debug.Log(_test);
        ClamFFI.Clam.voidcall(set_test);
        Debug.Log(r2);
        Debug.Log(_test);
        Debug.Log("test values" + test_values[0]);

        ClamFFI.Clam.set_stringcb(test_str_callback);


    }

    float pow(float a, float b)
    {
        return Mathf.Pow(a, b);
    }

    void set_test(float a)
    {
        _test = a;
        test_values.Add(a);
    }
    void set_test2(float a)
    {
        //float b = a;
    }

    unsafe void test_str_callback(byte* id)
    {
        // null-terminated `byte*` or sbyte* can materialize by new String()
        //var cString = ClamFFI.Clam.alloc_c_string();
        var str = new String((sbyte*)id);
        //ClamFFI.Clam.free_c_string(cString);
        Debug.Log(str);
    }

    //void init_node_objects()
    //{
    //    ClamFFI.Clam.InitNodeObjects();
    //}

    unsafe void InitNode(byte* idPtr, byte* leftPtr, byte* rightPtr)
    {
        //var cString = ClamFFI.Clam.alloc_c_string();
        var id = new String((sbyte*)idPtr);
        var lid = new String((sbyte*)leftPtr);
        var rid = new String((sbyte*)rightPtr);


        Debug.Log("adding node " + id);

        GameObject node = Instantiate(_nodePrefab);
        node.GetComponent<NodeScript>().SetID(id);
        node.GetComponent<NodeScript>().SetLeft(lid);
        node.GetComponent<NodeScript>().SetRight(rid);
        _tree.Add(id, node);
    }

    unsafe void reingoldify(ClamFFI.NodeBaton baton, byte* idPtr)
    {
        ClamFFI.Node nodeData = new ClamFFI.Node(baton);
        var id = new String((sbyte*)idPtr); 

        GameObject node;// = _tree.GetValueOrDefault(nodeData.id);

        bool hasValue = _tree.TryGetValue(id, out node);
        if (hasValue)
        {
            node.GetComponent<NodeScript>().SetColor(nodeData.color);
            //node.GetComponent<NodeScript>().SetColor(new Vector3(baton.r, baton.g, baton.b));
            node.GetComponent<NodeScript>().SetPosition(nodeData.pos);
        }
        else
        {
            Debug.Log("reingoldify key not found - " + nodeData.id);
            // do something when the value is not there
        }
    }


    // Update is called once per frame
    void Update()
    {
    }
}
