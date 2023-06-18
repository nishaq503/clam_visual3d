using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ClamFFI
{
    public static class FFIHelpers
    {
        //[DllImport(ClamFFI.Clam.__DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "free_string")]
        //public unsafe static extern void free_string(IntPtr context, IntPtr data);

        //public unsafe static void FreeString(IntPtr data)
        //{
        //    free_string(ClamFFI.Clam._handle, data);
        //}

        //[DllImport(ClamFFI.Clam.__DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "free_string_ffi")]
        //public unsafe static extern void free_string_ffi(ref NewClam.StringFFI inNode, out NewClam.StringFFI outNode);

        //public unsafe static void FreeStringFFI(ref NewClam.StringFFI inNode)
        //{
        //    free_string_ffi(ref inNode, out var outNode);
        //}
    }
}
