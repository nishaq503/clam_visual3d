using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Clam
{
    namespace FFI
    {

        public class ClusterDataWrapper
        {
            private ClusterData m_Data;

            //public ClusterDataWrapper(GameObject node)
            //{
            //    //m_NodeData = nodeData;
            //    //m_Data = new ClusterData(node);
            //}

            public ClusterDataWrapper(ClusterData nodeData)
            {
                m_Data = nodeData;
            }

            public ClusterData Data
            {
                get { return m_Data; }
                set
                {
                    //m_Data.FreeStrings();
                    m_Data = value;
                }
            }

            ~ClusterDataWrapper()
            {
                //m_Data.FreeStrings();
                //var data = wrapper.Data;
                Clam.FFI.NativeMethods.DeleteClusterData(ref m_Data);
            }
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public partial struct ClusterData
        {
            public Vec3 pos;
            public Vec3 color;
            public StringFFI id;
            //public StringFFI leftID;
            //public StringFFI rightID;
            // a string storing 100 bytes for passing misc values between rust and unity - allocated by rust
            public StringFFI message;
            public int cardinality;
            public int depth;
            public float radius;
            public float lfd;
            public int argCenter;
            public int argRadius;

            public float distToQuery;

            public void SetPos(Vector3 pos)
            {
                this.pos.x = pos.x;
                this.pos.y = pos.y;
                this.pos.z = pos.z;
            }
            public void SetColor(Color color)
            {
                this.color.x = color.r;
                this.color.y = color.g;
                this.color.z = color.b;
            }

            //public void SetMessage(string msg)
            //{
            //    Clam.FFI.NativeMethods.SetMessage(msg, ref this);
            //}




            //public NodeData(int n)
            //{
            //    pos = new Vec3();
            //    color = new Vec3();

            //    id = new StringFFI();
            //    leftID = new StringFFI();
            //    rightID = new StringFFI();

            //    cardinality = -1;
            //    depth = -1;
            //    argCenter = -1;
            //    argRadius = -1;
            //}

            //public ClusterData(GameObject node)
            //{
            //    var script = node.GetComponent<Node>();
            //    this.pos = new Vec3(script.GetPosition());
            //    this.color = new Vec3(script.GetColor());
            //    this.id = new StringFFI(script.GetId());
            //    this.leftID = new StringFFI(script.GetLeftChildID());
            //    this.rightID = new StringFFI(script.GetRightChildID());
            //    //m_NodeData = nodeData;

            //    cardinality = -1;
            //    depth = -1;
            //    radius = -1.0f;
            //    lfd = -1.0f;
            //    argCenter = -1;
            //    argRadius = -1;
            //    distToQuery = -1;
            //}

            //public ClusterData(string id, string leftID, string rightID, Vector3 pos, Color color)
            //{
            //    this.pos = new global::Clam.FFI.Vec3(pos);
            //    this.color = new Vec3(color);

            //    this.id = new StringFFI(id);
            //    this.leftID = new StringFFI(leftID);
            //    this.rightID = new StringFFI(rightID);

            //    cardinality = -1;
            //    depth = -1;
            //    radius = -1.0f;
            //    lfd = -1.0f;
            //    argCenter = -1;
            //    argRadius = -1;
            //    distToQuery = -1;

            //}
            //public NodeDataFFI(NodeDataUnity data)
            //{
            //    this.pos = new Vec3(data.pos);
            //    this.color = new Vec3(data.color);

            //    this.id = new StringFFI(data.id);
            //    this.leftID = new StringFFI(data.leftID);
            //    this.rightID = new StringFFI(data.rightID);

            //    cardinality = data.cardinality;
            //    depth = data.depth;
            //    radius = data.radius;
            //    lfd = data.lfd;
            //    argCenter = data.argCenter;
            //    argRadius = data.argRadius;
            //    distToQuery = -1;

            //}
            //public ClusterData(int n)
            //{
            //    this.pos = new global::Clam.FFI.Vec3(n, n, n);
            //    this.color = new Vec3(n, n, n);

            //    this.id = new StringFFI("testing " + n.ToString());
            //    this.leftID = new StringFFI(n.ToString());
            //    this.rightID = new StringFFI(n.ToString());

            //    cardinality = n;
            //    depth = n;
            //    radius = n;
            //    lfd = n;
            //    argCenter = n;
            //    argRadius = n;
            //    distToQuery = n;

            //}

            public void LogInfo()
            {
                Debug.Log("id: " + this.id.AsString);
                Debug.Log("pos: " + this.pos.AsVector3);
                Debug.Log("color: " + this.color.AsColor);

                //Debug.Log("leftID: " + this.leftID.AsString);
                //Debug.Log("rightID: " + this.rightID.AsString);
                Debug.Log("depth: " + this.depth);
                Debug.Log("cardinality: " + this.cardinality);
                Debug.Log("argCenter: " + this.argCenter);
                Debug.Log("argRadius: " + this.argRadius);
            }

            public string GetInfo()
            {
                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.AppendLine("id: " + this.id.AsString);
                //stringBuilder.AppendLine("leftID: " + this.leftID.AsString);
                //stringBuilder.AppendLine("rightID: " + this.rightID.AsString);
                stringBuilder.AppendLine("depth " + depth.ToString());
                stringBuilder.AppendLine("card: " + cardinality.ToString());
                stringBuilder.AppendLine("radius: " + radius.ToString());
                stringBuilder.AppendLine("lfd: " + lfd.ToString());
                stringBuilder.AppendLine("argC: " + argCenter.ToString());
                stringBuilder.AppendLine("argR: " + argRadius.ToString());
                //stringBuilder.AppendLine(this.color.ToString());

                return stringBuilder.ToString();
            }
            public string GetInfoForUI()
            {
                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(this.id.AsString);
                stringBuilder.AppendLine(depth.ToString());
                stringBuilder.AppendLine(cardinality.ToString());
                stringBuilder.AppendLine(radius.ToString());
                stringBuilder.AppendLine(lfd.ToString());
                stringBuilder.AppendLine(argCenter.ToString());
                stringBuilder.AppendLine(argRadius.ToString());

                return stringBuilder.ToString();

            }



            public void FreeStrings()
            {
                //this.id.Free();
                //this.leftID.Free();
                //this.rightID.Free();
                Debug.Log("This should never be called");
            }
        }

        public class ClusterIDsWrapper
        {
            private ClusterIDs m_Data;

            //public ClusterDataWrapper(GameObject node)
            //{
            //    //m_NodeData = nodeData;
            //    //m_Data = new ClusterData(node);
            //}

            public ClusterIDsWrapper(ClusterIDs nodeData)
            {
                m_Data = nodeData;
            }

            public ClusterIDs Data
            {
                get { return m_Data; }
                set
                {
                    //m_Data.FreeStrings();
                    m_Data = value;
                }
            }

            ~ClusterIDsWrapper()
            {
                //m_Data.FreeStrings();
                //var data = wrapper.Data;
                Clam.FFI.NativeMethods.DeleteClusterIDs(ref m_Data);
            }
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public partial struct ClusterIDs
        {
            //public Vec3 pos;
            //public Vec3 color;
            public StringFFI id;
            public StringFFI leftID;
            public StringFFI rightID;
            // a string storing for passing misc values between rust and unity - allocated by rust
            //public StringFFI message;
            //public int cardinality;
            //public int depth;
            //public float radius;
            //public float lfd;
            //public int argCenter;
            //public int argRadius;

            //public float distToQuery;

            //public void SetPos(Vector3 pos)
            //{
            //    this.pos.x = pos.x;
            //    this.pos.y = pos.y;
            //    this.pos.z = pos.z;
            //}
            //public void SetColor(Color color)
            //{
            //    this.color.x = color.r;
            //    this.color.y = color.g;
            //    this.color.z = color.b;
            //}

            //public void SetMessage(string msg)
            //{
            //    Clam.FFI.NativeMethods.SetMessage(msg, ref this);
            //}




            //public NodeData(int n)
            //{
            //    pos = new Vec3();
            //    color = new Vec3();

            //    id = new StringFFI();
            //    leftID = new StringFFI();
            //    rightID = new StringFFI();

            //    cardinality = -1;
            //    depth = -1;
            //    argCenter = -1;
            //    argRadius = -1;
            //}

            //public ClusterData(GameObject node)
            //{
            //    var script = node.GetComponent<Node>();
            //    this.pos = new Vec3(script.GetPosition());
            //    this.color = new Vec3(script.GetColor());
            //    this.id = new StringFFI(script.GetId());
            //    this.leftID = new StringFFI(script.GetLeftChildID());
            //    this.rightID = new StringFFI(script.GetRightChildID());
            //    //m_NodeData = nodeData;

            //    cardinality = -1;
            //    depth = -1;
            //    radius = -1.0f;
            //    lfd = -1.0f;
            //    argCenter = -1;
            //    argRadius = -1;
            //    distToQuery = -1;
            //}

            //public ClusterData(string id, string leftID, string rightID, Vector3 pos, Color color)
            //{
            //    this.pos = new global::Clam.FFI.Vec3(pos);
            //    this.color = new Vec3(color);

            //    this.id = new StringFFI(id);
            //    this.leftID = new StringFFI(leftID);
            //    this.rightID = new StringFFI(rightID);

            //    cardinality = -1;
            //    depth = -1;
            //    radius = -1.0f;
            //    lfd = -1.0f;
            //    argCenter = -1;
            //    argRadius = -1;
            //    distToQuery = -1;

            //}
            //public NodeDataFFI(NodeDataUnity data)
            //{
            //    this.pos = new Vec3(data.pos);
            //    this.color = new Vec3(data.color);

            //    this.id = new StringFFI(data.id);
            //    this.leftID = new StringFFI(data.leftID);
            //    this.rightID = new StringFFI(data.rightID);

            //    cardinality = data.cardinality;
            //    depth = data.depth;
            //    radius = data.radius;
            //    lfd = data.lfd;
            //    argCenter = data.argCenter;
            //    argRadius = data.argRadius;
            //    distToQuery = -1;

            //}
            //public ClusterData(int n)
            //{
            //    this.pos = new global::Clam.FFI.Vec3(n, n, n);
            //    this.color = new Vec3(n, n, n);

            //    this.id = new StringFFI("testing " + n.ToString());
            //    this.leftID = new StringFFI(n.ToString());
            //    this.rightID = new StringFFI(n.ToString());

            //    cardinality = n;
            //    depth = n;
            //    radius = n;
            //    lfd = n;
            //    argCenter = n;
            //    argRadius = n;
            //    distToQuery = n;

            //}

            //public void LogInfo()
            //{
            //    Debug.Log("id: " + this.id.AsString);
            //    Debug.Log("pos: " + this.pos.AsVector3);
            //    Debug.Log("color: " + this.color.AsColor);

            //    Debug.Log("leftID: " + this.leftID.AsString);
            //    Debug.Log("rightID: " + this.rightID.AsString);
            //    Debug.Log("depth: " + this.depth);
            //    Debug.Log("cardinality: " + this.cardinality);
            //    Debug.Log("argCenter: " + this.argCenter);
            //    Debug.Log("argRadius: " + this.argRadius);
            //}

            //public string GetInfo()
            //{
            //    StringBuilder stringBuilder = new StringBuilder();

            //    stringBuilder.AppendLine("id: " + this.id.AsString);
            //    stringBuilder.AppendLine("leftID: " + this.leftID.AsString);
            //    stringBuilder.AppendLine("rightID: " + this.rightID.AsString);
            //    stringBuilder.AppendLine("depth " + depth.ToString());
            //    stringBuilder.AppendLine("card: " + cardinality.ToString());
            //    stringBuilder.AppendLine("radius: " + radius.ToString());
            //    stringBuilder.AppendLine("lfd: " + lfd.ToString());
            //    stringBuilder.AppendLine("argC: " + argCenter.ToString());
            //    stringBuilder.AppendLine("argR: " + argRadius.ToString());
            //    //stringBuilder.AppendLine(this.color.ToString());

            //    return stringBuilder.ToString();
            //}
            //public string GetInfoForUI()
            //{
            //    StringBuilder stringBuilder = new StringBuilder();

            //    stringBuilder.AppendLine(this.id.AsString);
            //    stringBuilder.AppendLine(depth.ToString());
            //    stringBuilder.AppendLine(cardinality.ToString());
            //    stringBuilder.AppendLine(radius.ToString());
            //    stringBuilder.AppendLine(lfd.ToString());
            //    stringBuilder.AppendLine(argCenter.ToString());
            //    stringBuilder.AppendLine(argRadius.ToString());

            //    return stringBuilder.ToString();

            //}



            //public void FreeStrings()
            //{
            //    //this.id.Free();
            //    //this.leftID.Free();
            //    //this.rightID.Free();
            //    Debug.Log("This should never be called");
            //}
        }

    }





}
