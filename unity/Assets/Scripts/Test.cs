using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Clam;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Tests
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct ComplexStruct
    {
        public StringStruct myString;

        public ComplexStruct(string test)
        {
            myString = new StringStruct(test);
        }

        public string AsString { get { return Marshal.PtrToStringAnsi(myString.some_str); } }
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct StringStruct
    {
        public IntPtr some_str;
        public int str_len;

        public StringStruct(IntPtr test)
        {
            //this.some_str = test;
            var mystr = Marshal.PtrToStringAnsi(test);
            this.some_str = Marshal.StringToCoTaskMemUTF8(mystr);
            str_len = mystr.Length;
        }

        public StringStruct(string test)
        {
            this.some_str = Marshal.StringToCoTaskMemUTF8(test);
            str_len = test.Length;
        }

        public string AsString { get { return Marshal.PtrToStringAnsi(some_str); } }
        public IntPtr AsPtr { get { return some_str; } }
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
    }
    public unsafe class NodeData
    {
        public Vector3 pos;
        public Color color;
        public string id, leftID, rightID;
        public int cardinality;
        public int depth;
        public int argCenter;
        public int argRadius;

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

        public NodeData(string id, string leftID, string rightID, Vector3 position, Color color)
        {
            this.id = id; this.leftID = leftID;
            this.rightID = rightID;
            this.pos = position;
            this.color = color;
            this.cardinality = -1;
            this.depth = -1;
            this.argCenter = -1;
            this.argRadius = -1;
        }

        public NodeData(NodeFFI baton)
        {
            pos = new Vector3(baton.pos.x, baton.pos.y, baton.pos.z);
            color = new Color(baton.color.x, baton.color.y, baton.color.z);
            if (baton.id != null)
            {
                id = new String((sbyte*)baton.id);
            }
            else
            {
                Debug.Log("id null");

                id = "default";
            }
            if (baton.leftID != null)
            {
                leftID = new String((sbyte*)baton.leftID);
            }
            else
            {
                leftID = "default";
            }
            if (baton.rightID != null)
            {
                rightID = new String((sbyte*)baton.rightID);
            }
            else
            {
                rightID = "default";
            }

            cardinality = baton.cardinality;
            depth = baton.depth;
            argCenter = baton.argCenter;
            argRadius = baton.argRadius;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct NodeFFI
    {
        public Vec3 pos;
        public Vec3 color;
        public byte* id;
        public byte* leftID;
        public byte* rightID;

        public int cardinality;
        public int depth;
        public int argCenter;
        public int argRadius;

        public NodeFFI(NodeData node)
        {
            this.cardinality = node.cardinality;
            this.depth = node.depth;
            this.argCenter = node.argCenter;
            this.argRadius = node.argRadius;
            this.id = null;
            this.leftID = null;
            this.rightID = null;
            this.pos = new Vec3(node.pos.x, node.pos.y, node.pos.z);
            this.color = new Vec3(node.color.r, node.color.g, node.color.b);
        }
        public NodeFFI(int test)
        {
            cardinality = -1;
            depth = -1;
            argCenter = -1;
            argRadius = -1;
            id = null;
            leftID = null;
            rightID = null;
            pos = new Vec3();
            color = new Vec3();
        }
    }

    public static class Test
    {

        public const string __DllName = "clam_ffi_20230617184333";
        private static IntPtr _handle;

        public unsafe delegate void NodeVisitor2(ref Clam.NodeDataFFI baton);

        [DllImport(__DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "free_string")]
        public unsafe static extern void free_string(IntPtr context, IntPtr data);

        public unsafe static void FreeString(IntPtr data)
        {
            free_string(_handle, data);
        }

        [DllImport(__DllName, EntryPoint = "test_string_struct", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static unsafe extern void test_string_struct(ref StringStruct inNode, out StringStruct outNode);

        public static unsafe void TestStringStruct()
        {
            StringStruct nodeData = new StringStruct("hello again");
            test_string_struct(ref nodeData, out var outNode);

            Debug.Log("nodeData data after " + nodeData.AsString);

        }

        [DllImport(__DllName, EntryPoint = "test_string_struct_complex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static unsafe extern void test_string_struct_complex(ref Clam.NodeDataFFI inNode, out Clam.NodeDataFFI outNode);

        public static unsafe void TestStringStructComplex()
        {
            Clam.NodeDataFFI nodeData = new Clam.NodeDataFFI("ab", "123", "456", new Vector3(0.0f, 1.0f, 2.0f), new Color(0.5f, 0.6f, 0.7f));
            test_string_struct_complex(ref nodeData, out var outNode);

            Debug.Log("nodeData data after " + nodeData.id.AsString);

        }

        [DllImport(__DllName, EntryPoint = "test_string_struct_rust_alloc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static unsafe extern void test_string_struct_rust_alloc(ref StringStruct inNode, out StringStruct outNode);

        public static unsafe void TestStringStructRustAlloc()
        {
            StringStruct nodeData = new StringStruct();
            test_string_struct_rust_alloc(ref nodeData, out var outNode);

            Debug.Log("nodeData data after " + outNode.AsString);
            FreeString2(ref outNode);

        }

        [DllImport(__DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "test_string_fn")]
        public unsafe static extern void test_string_fn(string data);

        public unsafe static void TestStringFn(string data)
        {
            string testData = "hello world!";
            test_string_fn(testData);
        }


        [DllImport(__DllName, EntryPoint = "test_node_rust_alloc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static unsafe extern void test_node_rust_alloc(ref Clam.NodeDataFFI inNode, out Clam.NodeDataFFI outNode);

        public static unsafe void TestNodeRustAlloc()
        {
            Clam.NodeDataFFI nodeData = new Clam.NodeDataFFI();
            test_node_rust_alloc(ref nodeData, out var outNode);

            Debug.Log("nodeData data after 123456 " + outNode.id.AsString);
            //TakeOwnershipOfRustIDs(ref outNode);
            //Clam.test_node_rust_alloc(ref outNode);

        }

        [DllImport(__DllName, EntryPoint = "test_node_rust_alloc2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static unsafe extern void test_node_rust_alloc2(IntPtr handle, NodeVisitor2 visitor);

        public static unsafe void TestNodeRustAlloc2()
        {

            test_node_rust_alloc2(_handle, SetNodeNames);
            //NewClam.NodeData nodeData = new NewClam.NodeData();
            //test_node_rust_alloc2(ref nodeData, out var outNode);

            //Debug.Log("nodeData data after 123456 " + outNode.id.AsString);
            //TakeOwnershipOfRustIDs(ref outNode);
            //Clam.test_node_rust_alloc(ref outNode);

        }

        unsafe static void SetNodeNames(ref Clam.NodeDataFFI nodeData)
        {
            //ClamFFI.NodeData nodeData = new ClamFFI.NodeData(baton);
            Debug.Log("pos x " + nodeData.pos.x);
            Debug.Log("pos y" + nodeData.pos.y);
            Debug.Log("pos z " + nodeData.pos.z);
            //GameObject node = Instantiate(_nodePrefab);
            Debug.Log("adding nod123e " + nodeData.id.AsString);

            Debug.Log("card " + nodeData.cardinality);
            Debug.Log("left " + nodeData.leftID.AsString);
            Debug.Log("right " + nodeData.rightID.AsString);

            //node.GetComponent<NodeScript>().SetID(nodeData.id);
            //node.GetComponent<NodeScript>().SetLeft(nodeData.leftID);
            //node.GetComponent<NodeScript>().SetRight(nodeData.rightID);
            //_tree.Add(nodeData.id, node);
            //_nodeName = nodeData.id;
        }

        //[DllImport(__DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "take_ownership_of_rust_ids")]
        //public unsafe static extern void take_ownership_of_rust_ids(ref NewClam.NodeData inNode, out NewClam.NodeData outNode);

        //public unsafe static void TakeOwnershipOfRustIDs(ref NewClam.NodeData inNode)
        //{
        //    take_ownership_of_rust_ids(ref inNode, out var outNode);
        //}


        [DllImport(__DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "free_string2")]
        public unsafe static extern void free_string2(ref StringStruct inNode, out StringStruct outNode);

        public unsafe static void FreeString2(ref StringStruct inNode)
        {
            free_string2(ref inNode, out var outNode);
        }

        [DllImport(__DllName, EntryPoint = "test_string_struct2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static unsafe extern void test_string_struct2(ref StringStruct inNode, out StringStruct outNode);

        public static unsafe void TestStringStruct2()
        {
            StringStruct nodeData = new StringStruct("hello again");
            test_string_struct2(ref nodeData, out var outNode);

            Debug.Log("nodeData data after " + nodeData.AsString);

        }

        [DllImport(__DllName, EntryPoint = "free_node_string", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static unsafe extern void free_node_string(IntPtr handle, ref NodeFFI inNode, out NodeFFI outNode);


        [DllImport(__DllName, EntryPoint = "destroy_node_baton", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static unsafe extern void destroy_node_baton(NodeFFI context);
    }

   

}
