using System.Runtime.InteropServices;
using System;
using UnityEngine;

namespace Clam
{
    namespace FFI
    {

        //public class StringFFIWrapper
        //{
        //    private StringFFI m_Data;

        //    public StringFFIWrapper(StringFFI nodeData)
        //    {
        //        m_Data = nodeData;
        //    }

        //    public StringFFI Data
        //    {
        //        get { return m_Data; }
        //        set
        //        {
        //            //m_Data.FreeStrings();
        //            m_Data = value;
        //        }
        //    }

        //    ~StringFFIWrapper()
        //    {
        //        Debug.Log("freeign allocated string in wrapper");
        //        NativeMethods.FreeString(ref m_Data);
        //    }
        //}

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public partial struct StringFFI : IRustResource
        {
            private IntPtr m_Data;
            private int m_Length;

            public static (StringFFI, FFIError) Alloc(string data)
            {
                var result = NativeMethods.AllocString(data, out var resource);
                return (resource, result);
            }

            private StringFFI(string data)
            {
                Debug.LogError("Don't create string ffi in c#");
                m_Data = IntPtr.Zero; 
                m_Length = 0;
            }

            public string AsString
            {
                get
                {
                    if (m_Data == null)
                    {
                        Debug.Log("Error id data is null");
                        return "";
                    }
                    return Marshal.PtrToStringAnsi(m_Data);
                }
            }
            public IntPtr AsPtr { get { return m_Data; } }

            public bool IsEmpty { get { return m_Length == 0; } }
            public bool IsNull { get { return m_Data == IntPtr.Zero; } }

            public void Free()
            {
                NativeMethods.FreeString(ref this);
            }
        }
    }
}