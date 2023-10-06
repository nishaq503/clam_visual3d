//using System.Collections;
//using System.Collections.Generic;
//using System.Text;
//using UnityEngine;

//public static class ClamHelpers 
//{

//    private static readonly Dictionary<char, string> hexCharacterToBinary = new Dictionary<char, string> {
//    { '0', "0000" },
//    { '1', "0001" },
//    { '2', "0010" },
//    { '3', "0011" },
//    { '4', "0100" },
//    { '5', "0101" },
//    { '6', "0110" },
//    { '7', "0111" },
//    { '8', "1000" },
//    { '9', "1001" },
//    { 'a', "1010" },
//    { 'b', "1011" },
//    { 'c', "1100" },
//    { 'd', "1101" },
//    { 'e', "1110" },
//    { 'f', "1111" }
//};

//    public static string HexStringToBinary(string hex)
//    {
//        StringBuilder result = new StringBuilder();
//        foreach (char c in hex)
//        {
//            // This will crash for non-hex characters. You might want to handle that differently.
//            result.Append(hexCharacterToBinary[char.ToLower(c)]);
//        }
//        return result.ToString();
//    }

//    public static string HexToTrimmedBinaryString(string hex)
//    {
//        return HexStringToBinary(hex).TrimStart('0');
//    }
//}

//[DllImport(__DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "free_string_ffi")]
//public unsafe static extern void free_string_ffi(ref global::Clam.FFI.StringFFI inNode, out global::Clam.FFI.StringFFI outNode);

//public unsafe static void FreeStringFFI(ref global::Clam.FFI.StringFFI inNode)
//{
//    free_string_ffi(ref inNode, out var outNode);
//}

//[System.Security.SecurityCritical]
//[DllImport(__DllName, EntryPoint = "test_struct_array", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
//private static unsafe extern void test_struct_array(IntPtr handle, [In, Out] ClusterData[] arr, int len);
//struct float1
//{
//    public float f;
//}
//public static unsafe void TestStructArray()
//{
//    int numItems = 5;
//    ClusterData[] items = new ClusterData[numItems];
//    for (int i = 0; i < numItems; i++)
//    {
//        items[i] = new ClusterData(i);
//    }

//    test_struct_array(m_Handle, items, items.Length);
//}

//public unsafe static void FreeString(IntPtr data)
//{
//    free_string(m_Handle, data);
//}

//[DllImport(__DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "free_string")]
//private unsafe static extern void free_string(IntPtr context, IntPtr data);

//[System.Security.SecurityCritical]
//[DllImport(__DllName, EntryPoint = "color_by_dist_to_query", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
//private static unsafe extern void color_by_dist_to_query(IntPtr handle, [In, Out] NodeDataFFI[] arr, int len, NodeVisitor cb_fn);
//public static unsafe void ColorByDistToQuery(List<NodeDataUnity> nodes, NodeVisitor cbFn)
//{
//    NodeDataFFI[] items = new NodeDataFFI[nodes.Count];
//    for (int i = 0; i < nodes.Count; i++)
//    {
//        //var id = nodes[i].id;
//        items[i] = new NodeDataFFI(nodes[i]);
//    }

//    color_by_dist_to_query(_handle, items, items.Length, cbFn);



//}

//[System.Security.SecurityCritical]
//[DllImport(__DllName, EntryPoint = "detect_edges", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
//private static unsafe extern void detect_edges(IntPtr handle, [In, Out] NodeDataFFI[] arr, int len, NodeVisitor cb_fn);
//public static unsafe void DetectEdges(List<NodeDataUnity> nodes, NodeVisitor cbFn)
//{
//    NodeDataFFI[] items = new NodeDataFFI[nodes.Count];
//    for (int i = 0; i < nodes.Count; i++)
//    {
//        //var id = nodes[i].id;
//        items[i] = new NodeDataFFI(nodes[i]);
//    }

//    detect_edges(_handle, items, items.Length, cbFn);
//}

//[System.Security.SecurityCritical]
//[DllImport(__DllName, EntryPoint = "init_force_directed_sim", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
//private static unsafe extern void init_force_directed_sim(IntPtr handle, [In, Out] NodeDataFFI[] arr, int len, NodeVisitor cb_fn);
//public static unsafe void InitForceDirectedSim(List<NodeDataUnity> nodes, NodeVisitor cbFn)
//{
//    NodeDataFFI[] items = new NodeDataFFI[nodes.Count];
//    for (int i = 0; i < nodes.Count; i++)
//    {
//        //var id = nodes[i].id;
//        items[i] = new NodeDataFFI(nodes[i]);
//    }

//    init_force_directed_sim(_handle, items, items.Length, cbFn);

//}