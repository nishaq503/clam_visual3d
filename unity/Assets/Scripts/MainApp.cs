using ClamFFI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainApp : MonoBehaviour
{

    public GameObject nodePrefab;
    public GameObject treePrefab;

    public GameObject userInterface;
    //public Canvas canvas;
    //private GameObject 


    private InputHandler m_InputHandler;
    // Start is called before the first frame update
    void Start()
    {
        TreeScript().Init();
        m_InputHandler = new InputHandler();

    }

    // Update is called once per frame
    void Update()
    {
        //m_InputHandler.Update(canvas, treePrefab);
        m_InputHandler.Update(userInterface, treePrefab);
    }

    TreeScript TreeScript()
    {
        return treePrefab.GetComponent<TreeScript>();
    }
}
