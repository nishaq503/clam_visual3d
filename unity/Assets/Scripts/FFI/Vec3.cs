using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Clam
{

    namespace FFI

    {


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

            public Vec3 Add(Vec3 other)
            {
                return new Vec3(this.x + other.x, this.y + other.y, this.z + other.z);
            }
            public Vector3 Add(Vector3 other)
            {
                return new Vector3(this.x + other.x, this.y + other.y, this.z + other.z);
            }

        }
    }
}

