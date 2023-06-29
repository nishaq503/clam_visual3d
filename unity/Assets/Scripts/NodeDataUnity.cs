using ClamFFI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public class NodeDataUnity
{
    public Vector3 pos;
    public Color color;
    public string id;
    public string leftID;
    public string rightID;
    public int cardinality;
    public int depth;
    public float radius;
    public float lfd;
    public int argCenter;
    public int argRadius;

    public NodeDataUnity(string id, string leftID, string rightID, Vector3 pos, Color color)
    {
        this.pos = pos;
        this.color = color;

        this.id = id;
        this.leftID =leftID;
        this.rightID = rightID;

        cardinality = -1;
        depth = -1;
        radius = -1.0f;
        lfd = -1.0f;
        argCenter = -1;
        argRadius = -1;
    }

    public NodeDataUnity(ClamFFI.NodeDataFFI data)
    {
        this.pos = data.pos.AsVector3;
        this.color = data.color.AsColor;

        this.id = data.id.AsString;
        this.leftID = data.leftID.AsString;
        this.rightID = data.rightID.AsString;

        cardinality = data.cardinality;
        depth = data.depth;
        radius = data.radius;
        lfd = data.lfd;
        argCenter = data.argCenter;
        argRadius = data.argRadius;
    }

    public NodeDataUnity(NodeDataUnity data)
    {
        this.pos = data.pos;
        this.color = data.color;

        this.id = data.id;
        this.leftID = data.leftID;
        this.rightID = data.rightID;

        cardinality = data.cardinality;
        depth = data.depth;
        radius = data.radius;
        lfd = data.lfd;
        argCenter = data.argCenter;
        argRadius = data.argRadius;
    }

    public void LogInfo()
    {
        Debug.Log("id: " + this.id);
        Debug.Log("pos: " + this.pos);
        Debug.Log("color: " + this.color);

        Debug.Log("leftID: " + this.leftID);
        Debug.Log("rightID: " + this.rightID);
        Debug.Log("depth: " + this.depth);
        Debug.Log("cardinality: " + this.cardinality);
        Debug.Log("argCenter: " + this.argCenter);
        Debug.Log("argRadius: " + this.argRadius);
    }

    public string GetInfo()
    {
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.AppendLine("id: " + this.id);
        stringBuilder.AppendLine("depth " + depth.ToString());
        stringBuilder.AppendLine("card: " + cardinality.ToString());
        stringBuilder.AppendLine("radius: " + radius.ToString());
        stringBuilder.AppendLine("lfd: " + lfd.ToString());
        stringBuilder.AppendLine("argC: " + argCenter.ToString());
        stringBuilder.AppendLine("argR: " + argRadius.ToString());
        //stringBuilder.AppendLine(this.color.ToString());

        return stringBuilder.ToString();
    }
    
}