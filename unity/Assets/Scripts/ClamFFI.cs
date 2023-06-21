
#pragma warning disable CS8500
#pragma warning disable CS8981
using ClamFFI;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ClamFFI
{
    public unsafe delegate void NodeVisitor(ref ClamFFI.NodeData baton);

    public static partial class Clam
    {
	public const string __DllName = "clam_ffi_20230621160825";
        private static IntPtr _handle;

        [DllImport(__DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "free_string")]
        private unsafe static extern void free_string(IntPtr context, IntPtr data);
     

        [DllImport(__DllName, EntryPoint = "for_each_dft", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        private static extern FFIError for_each_dft(IntPtr ptr, NodeVisitor callback, string startNode);

        public static FFIError ForEachDFT(NodeVisitor callback, string startNode = "root")
        {
            return for_each_dft(_handle, callback, startNode);
        }

        [DllImport(__DllName, EntryPoint = "get_cluster_data", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static unsafe extern FFIError get_cluster_data(IntPtr handle,ref ClamFFI.NodeData inNode, out ClamFFI.NodeData outNode);

        public static unsafe FFIError GetClusterData(ClamFFI.NodeWrapper nodeWrapper)
        {
            NodeData nodeData = nodeWrapper.Data;

            FFIError found = get_cluster_data(_handle, ref nodeData, out var outNode);
            if(found == FFIError.Ok)
            {
                nodeWrapper.Data = outNode;
            }
            return found;
        }

        [DllImport(__DllName, EntryPoint = "create_reingold_layout", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        private static extern FFIError create_reingold_layout(IntPtr ptr, NodeVisitor callback);

        public static FFIError CreateReingoldLayout(NodeVisitor callback)
        {
            return create_reingold_layout(_handle, callback);
        }

        [DllImport(__DllName, EntryPoint = "get_num_nodes", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int get_num_nodes(IntPtr handle);

        public static int GetNumNodes()
        {
            return get_num_nodes(_handle);
        }

        [DllImport(__DllName, EntryPoint = "init_clam", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        private static extern FFIError init_clam(out IntPtr ptr, byte[] data_name, int name_len, uint cardinality);

        public static FFIError InitClam(string dataName, uint cardinality)
        {
            byte[] byteName = Encoding.UTF8.GetBytes(dataName);
            int len = byteName.Length;

            return init_clam(out _handle, byteName, len, cardinality);
        }

        

        [DllImport(__DllName, EntryPoint = "shutdown_clam", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        private static extern FFIError shutdown_clam(out IntPtr ptr);

        public static FFIError ShutdownClam()
        {
            return shutdown_clam(out _handle);
        }

        public unsafe static void FreeString(IntPtr data)
        {
            free_string(_handle, data);
        }

        [DllImport(__DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "free_string_ffi")]
        public unsafe static extern void free_string_ffi(ref ClamFFI.StringFFI inNode, out ClamFFI.StringFFI outNode);

        public unsafe static void FreeStringFFI(ref ClamFFI.StringFFI inNode)
        {
            free_string_ffi(ref inNode, out var outNode);
        }
    }
}
