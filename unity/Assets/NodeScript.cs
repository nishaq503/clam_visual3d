using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SetColor(new Vector3(0.1f,0.0f,0.0f));
        //Debug.Log(System.String.Format("Error: tree for {0} not created. Check debug log file.", ClamFFI.Clam.get_answer()));


    }

    // Update is called once per frame
    void Update()
    {
    }

    void SetPosition(Vector3 pos)
    {
        GetComponent<Transform>().position = pos;
    }

    void SetColor(Vector3 color)
    {
        GetComponent<Renderer>().material.color = new Color(color.x, color.y, color.z);    
    }
}
