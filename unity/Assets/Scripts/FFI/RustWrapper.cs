using System;
using System.Diagnostics;

public interface IRustResource
{
    void Free();
}


public class RustWrapper<T> where T : IRustResource, new()
{
    private T m_WappedObject;

    public RustWrapper(T data)
    {
        m_WappedObject = data;
    }

    public T GetData()
    {
        return m_WappedObject;
    }

    // ... other methods related to the Rust FFI

    ~RustWrapper()
    {
        // Assuming the free method is called Dispose for IDisposable objects
        m_WappedObject.Free();
    }
}