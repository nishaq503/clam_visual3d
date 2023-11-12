
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Clam
{
    namespace FFI
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct StartupDataFFI
        {
            StringFFI dataName;
            public DistanceMetric metric;
            public uint cardinality;
            public bool isExpensive;

            public StartupDataFFI(TreeStartupData data)
            {
                NativeMethods.AllocString(data.dataName, out dataName);
                metric = data.distanceMetric;
                cardinality = data.cardinality;
                isExpensive = data.isExpensive;

                Debug.Log("data ffi name " + this.dataName.AsString);
            }
        }
    }
}