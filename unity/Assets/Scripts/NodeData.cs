using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEditor.UI;
using UnityEngine;

namespace ClamFFI
{

    public class NodeWrapper
    {
        private NodeData m_NodeData;

        public NodeWrapper(NodeData nodeData)
        {
            m_NodeData = nodeData;
        }

        public NodeData Data
        {
            get { return m_NodeData; }
            set { m_NodeData = value; }
        }

        ~NodeWrapper()
        {
            m_NodeData.FreeStrings();
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct NodeData
    {
        public Vec3 pos;
        public Vec3 color;
        public StringFFI id;
        public StringFFI  leftID;
        public StringFFI rightID;
        public int cardinality;
        public int depth;
        public int argCenter;
        public int argRadius;

        public NodeData(int n)
        {
            pos = new Vec3();
            color = new Vec3();

            id = new StringFFI();
            leftID = new StringFFI();
            rightID = new StringFFI();

            cardinality = -1;
            depth = -1;
            argCenter = -1;
            argRadius = -1;
        }

        public NodeData(string id, string leftID, string rightID, Vector3 pos, Color color)
        {
            this.pos = new ClamFFI.Vec3(pos);
            this.color = new Vec3(color);

            this.id = new StringFFI(id);
            this.leftID = new StringFFI(leftID);
            this.rightID = new StringFFI(rightID);

            cardinality = -1;
            depth = -1;
            argCenter = -1;
            argRadius = -1;
        }

        public void LogInfo()
        {
            Debug.Log("id: " + this.id.AsString);
            Debug.Log("pos: " + this.pos.AsVector3);
            Debug.Log("color: " + this.color.AsColor);

            Debug.Log("leftID: " + this.leftID.AsString);
            Debug.Log("rightID: " + this.rightID.AsString);
            Debug.Log("depth: " + this.depth);
            Debug.Log("cardinality: " + this.cardinality);
            Debug.Log("argCenter: " + this.argCenter);
            Debug.Log("argRadius: " + this.argRadius);
        }

        public string GetInfo()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("id: " + this.id.AsString);
            stringBuilder.AppendLine("depth " + depth.ToString());
            stringBuilder.AppendLine("card: " + cardinality.ToString());
            stringBuilder.AppendLine("argC: " + argCenter.ToString());
            stringBuilder.AppendLine("argR: " + argRadius.ToString());
            //stringBuilder.AppendLine(this.color.ToString());

           return stringBuilder.ToString();
        }

        public void FreeStrings()
        {
            this.id.Free();
            this.leftID.Free();
            this.rightID.Free();
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct StringFFI
    {
        private IntPtr data;
        private int len;
        private bool isOwnedByUnity;
        //private bool mustFree;

        //public StringFFI(int n)
        //{
        //    data = IntPtr.Zero;
        //    len = 0;
        //    isOwned = false;
        //}

        public StringFFI(string data)
        {
            this.data = Marshal.StringToCoTaskMemUTF8(data);
            len = data.Length;
            isOwnedByUnity = true;
            //mustFree = true;
        }

        public string AsString { get { return Marshal.PtrToStringAnsi(data); } }
        public IntPtr AsPtr { get { return data; } }

        public bool IsEmpty { get { return len == 0; } }
        public bool IsOwned { get { return isOwnedByUnity; } }
        public bool IsNull { get { return data == IntPtr.Zero; } }

        public void CopyToUnity()
        {
            if (!isOwnedByUnity)
            {
                var text = Marshal.PtrToStringUTF8(data);
                len = text.Length;
                //ClamFFI.Clam.FreeStringFFI(ref this);
                data = Marshal.StringToCoTaskMemUTF8(text);
                isOwnedByUnity = true;
            }
            else
            {
                Debug.Log("Error: string is already owned by c#");
            }
        }

        public void TakeOwnership()
        {
            if (!isOwnedByUnity)
            {
                var text = Marshal.PtrToStringUTF8(data);
                len = text.Length;
                ClamFFI.Clam.FreeStringFFI(ref this);
                data = Marshal.StringToCoTaskMemUTF8(text);
                isOwnedByUnity = true;
            }
            else
            {
                Debug.Log("Error: string is already owned by c#");
            }
        }

        public void Free()
        {
            if (!IsNull && isOwnedByUnity)
            {
                //Debug.Log("string ffi freeing memory");
                Marshal.FreeCoTaskMem(data);
                data = IntPtr.Zero;
                len = 0;
            }
            else if (!IsNull && !isOwnedByUnity)
            {
                Debug.Log("warning memory not freed from string ffi");
                //ClamFFI.Clam.FreeStringFFI(ref this);
                //Debug.Log("successfully freed rust string");
            }
            else
            {
                Debug.Log("Warning: string is already null");
            }
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct Vec3
    {
        public float x;
        public float y;
        public float z;

        public Vec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vec3(Vector3 other)
        {
            this.x = other.x;
            this.y = other.y;
            this.z = other.z;
        }

        public Vec3(Color other)
        {
            this.x = other.r;
            this.y = other.g;
            this.z = other.b;
        }

        public Color AsColor { get { return new Color(x, y, z); } }
        public Vector3 AsVector3 { get { return new Vector3(x, y, z); } }

    }
}
