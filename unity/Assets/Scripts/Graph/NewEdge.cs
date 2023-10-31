using UnityEngine;

public class NewEdge : MonoBehaviour
{
    int m_ID;
    public Transform node1; // Reference to the first object's transform
    public Transform node2; // Reference to the second object's transform
    public float natLen;    // Your defined natural length
    public float scalar;    // Your defined scalar
    public float k;         // Your defined spring constant

    void ApplyForces()
    {
        Vector3 force = node2.position - node1.position;
        float forceMagnitude = force.magnitude;
        float targetLen = (natLen / Mathf.Max(float.Epsilon, forceMagnitude)) * scalar;
        float newMagnitude = k * (forceMagnitude - targetLen);

        Vector3 newForce = force.normalized * newMagnitude;

        // Apply forces at specific positions within each component's Rigidbody
        Rigidbody rb1 = node1.GetComponent<Rigidbody>();
        rb1.AddForceAtPosition(newForce, node1.position);

        // Reverse direction of force for the second component
        newForce *= -1.0f;

        Rigidbody rb2 = node2.GetComponent<Rigidbody>();
        rb2.AddForceAtPosition(newForce, node2.position);
    }

    void FixedUpdate()
    {
        ApplyForces();
    }
}