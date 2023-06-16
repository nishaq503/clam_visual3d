using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace NewClam
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct NodeData
    {
        public Vec3 pos;
        public Vec3 color;
        public StringFFI id, leftID, rightID;
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

    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct StringFFI
    {
        private IntPtr data;
        private int len;
        private bool isOwned;

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
            isOwned = true;
        }

        public string AsString { get { return Marshal.PtrToStringAnsi(data); } }
        public IntPtr AsPtr { get { return data; } }

        public bool IsEmpty { get { return len == 0; } }
        public bool IsOwned { get { return isOwned; } }
        public bool IsNull { get { return data == IntPtr.Zero; } }

        public void TakeOwnership()
        {
            if (!isOwned)
            {
                var text = Marshal.PtrToStringUTF8(data);
                len = text.Length;
                ClamFFI.Clam.FreeStringFFI(ref this);
                data = Marshal.StringToCoTaskMemUTF8(text);
                isOwned = true;
            }
            else
            {
                Debug.Log("Error: string is already owned by c#");
            }
        }

        void Free()
        {
            if (!IsNull)
            {
                Debug.Log("string ffi freeing memory");
                Marshal.FreeCoTaskMem(data);
                data = IntPtr.Zero;
                len = 0;
            }
            else
            {
                Debug.Log("Error: string is already null");
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

        public Color AsColor { get { return new Color(x, y, z); } }
        public Vector3 AsVector3 { get { return new Vector3(x, y, z); } }

    }
}
