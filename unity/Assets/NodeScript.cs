using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class NodeScript : MonoBehaviour
{

    private string _id;
    private string _leftChildID;
    private string _rightChildID;

    // Start is called before the first frame update
    void Start()
    {
        //SetColor(new Vector3(0.15f,0.85f,0.0f));
        //SetColor(new Vector3(0.15f,0.85f,0.0f));
        //Debug.Log(System.String.Format("Error: tree for {0} not created. Check debug log file.", ClamFFI.Clam.get_answer()));
        //int clam_result = ClamFFI.Clam.InitClam("arrhythmia", 50);
        //ClamFFI.Clam.CreateReingoldLayout();

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetPosition(Vector3 pos)
    {
        GetComponent<Transform>().position = new Vector3(pos.x, -pos.y, pos.z);
        //GetComponent<Transform>().position = pos;
    }

    public void SetColor(Vector3 color)
    {
        Debug.Log("setting node " + _id + " color to " + color);
        GetComponent<Renderer>().material.color = new Color(color.x, color.y, color.z);    
    }

    public void SetID(string id)
    {
        _id = id;
    }

    public void SetLeft(string id)
    {
        _leftChildID = id;
    }

    public void SetRight(string id)
    {
        _rightChildID = id;
    }




}
