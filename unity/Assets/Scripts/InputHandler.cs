using Clam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public GameObject Tree;
    public GameObject ClusterUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnLMC(InputValue value)
    {
        //Tree.GetComponent<TreeScript>().HandleLMC();
        ClusterUI.GetComponent<ClusterUIScript>().OnLMC();
        
    }
}
