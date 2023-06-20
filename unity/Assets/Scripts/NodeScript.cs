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

    public int test = 5;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ExpandSubtree()
    {
        //ClamFFI.Clam.ForEachDFT(m_ExpandSubtree, this._id);
    }

   

    public void SetPosition(Vector3 pos)
    {
        GetComponent<Transform>().position = new Vector3(pos.x, -pos.y, pos.z);
        //GetComponent<Transform>().position = pos;
    }

    public void SetColor(Color color)
    {
        //Debug.Log("setting node " + _id + " color to " + color);
        GetComponent<Renderer>().material.color = color;
    }

    public string GetId()
    {
        return _id;
    }

    public string GetLeftChildID()
    {
        return _leftChildID;
    }
    public string GetRightChildID()
    {
        return _rightChildID;
    }
    public Vector3 GetPosition()
    {
        return GetComponent<Transform>().position;
    }

    public Color GetColor()
    {
        return GetComponent<Renderer>().material.color;
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

 
    public ClamFFI.NodeData ToNodeData()
    {
        ClamFFI.NodeData node = new ClamFFI.NodeData(_id, _leftChildID, _rightChildID, GetPosition(), GetColor());

        return node;
    }
}
