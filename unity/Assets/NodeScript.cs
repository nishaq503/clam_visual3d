using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using static UnityEditor.PlayerSettings;

public class NodeScript : MonoBehaviour
{

    private string _id;
    private string _leftChildID;
    private string _rightChildID;

    // Start is called before the first frame update
    void Start()
    {

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
