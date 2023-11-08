using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGeneric : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Clam.FFI.NativeMethods.CreateHandleF32_I32_U8(9.0f,10,7);

        Debug.Log("handle value " + Clam.FFI.NativeMethods.GetF32_I32_U8_Value1());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
