
#pragma warning disable CS8500
#pragma warning disable CS8981
using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

namespace ClamFFI
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

        public StringStruct(string test)
        {
            this.some_str = Marshal.StringToCoTaskMemUTF8(test);
            str_len = test.Length;
        }

        public string AsString { get { return Marshal.PtrToStringAnsi(some_str); } }
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

    public static class Clam
    {
	public const string __DllName = "clam_ffi_20230615185911";
        private static IntPtr _handle;

        public unsafe delegate void NodeVisitor(NodeFFI baton);

        [DllImport(__DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "free_string")]
        public unsafe static extern void free_string(IntPtr context, byte* data);

        public unsafe static void FreeString(byte* data)
        {
            free_string(_handle, data);
        }


        [DllImport(__DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "test_string_fn")]
        public unsafe static extern void test_string_fn(string data);

        public unsafe static void TestStringFn(string data)
        {
            string testData = "hello world!";
            test_string_fn(testData);
        }

        [DllImport(__DllName, EntryPoint = "test_string_struct", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static unsafe extern void test_string_struct(ref StringStruct inNode, out StringStruct outNode);

        public static unsafe void TestStringStruct()
        {
            ClamFFI.StringStruct nodeData = new ClamFFI.StringStruct("hello again");
            test_string_struct(ref nodeData, out var outNode);

            Debug.Log("nodeData data after " + nodeData.AsString);

        }

        [DllImport(__DllName, EntryPoint = "test_string_struct2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static unsafe extern void test_string_struct2(ref StringStruct inNode, out StringStruct outNode);

        public static unsafe void TestStringStruct2()
        {
            ClamFFI.StringStruct nodeData = new ClamFFI.StringStruct("hello again");
            test_string_struct2(ref nodeData, out var outNode);

            Debug.Log("nodeData data after " + nodeData.AsString);

        }

        [DllImport(__DllName, EntryPoint = "get_node_data", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static unsafe extern void get_node_data(IntPtr handle, byte[] binary_id, int idLen, ref NodeFFI inNode, out NodeFFI outNode);

        public static unsafe NodeData FindClamData(ClamFFI.NodeData nodeData)
        {
           
            NodeFFI baton = new NodeFFI(1);
            string binaryID = ClamHelpers.HexToTrimmedBinaryString(nodeData.id);
            byte[] byteName = Encoding.UTF8.GetBytes(binaryID);
            int len = byteName.Length;
            Debug.Log("bytename " + binaryID);
           
            get_node_data(_handle, byteName, byteName.Length, ref baton, out var outNode);

            NodeData finalNode = new NodeData(outNode);
            free_node_string(_handle, ref baton, out var freedBaton);
            return finalNode;
        }

        [DllImport(__DllName, EntryPoint = "free_node_string", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static unsafe extern void free_node_string(IntPtr handle, ref NodeFFI inNode, out NodeFFI outNode);


        [DllImport(__DllName, EntryPoint = "destroy_node_baton", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static unsafe extern void destroy_node_baton(NodeFFI context);


        [DllImport(__DllName, EntryPoint = "create_reingold_layout", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        private static extern int create_reingold_layout(IntPtr ptr, NodeVisitor callback);

        public static int CreateReingoldLayout(NodeVisitor callback)
        {
            return create_reingold_layout(_handle, callback);
        }

        [DllImport(__DllName, EntryPoint = "get_num_nodes", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int get_num_nodes(IntPtr handle);

        public static int GetNumNodes()
        {
            return get_num_nodes(_handle);
        }

        [DllImport(__DllName, EntryPoint = "test", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int test();

        public static int Test()
        {
            return test();
        }

        [DllImport(__DllName, EntryPoint = "traverse_tree_df", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        private static extern int traverse_tree_df(IntPtr ptr, NodeVisitor callback);

        public static int TraverseTreeDF(NodeVisitor callback)
        {
            return traverse_tree_df(_handle, callback);
        }

        [DllImport(__DllName, EntryPoint = "init_clam", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        private static extern int init_clam(out IntPtr ptr, byte[] data_name, int name_len, uint cardinality);

        public static int InitClam(string dataName, uint cardinality)
        {
            byte[] byteName = Encoding.UTF8.GetBytes(dataName);
            int len = byteName.Length;

            return init_clam(out _handle, byteName, len, cardinality);
        }
    }


}
