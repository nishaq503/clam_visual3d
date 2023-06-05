using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : MonoBehaviour
{
    public string dataName = "arrhythmia";
    public uint cardinality = 50;

    public GameObject _nodePrefab;
    private Dictionary<string, GameObject> _tree;
    // Start is called before the first frame update
    void Start()
    {
       
        int clam_result = ClamFFI.Clam.InitClam(dataName, cardinality);
        if (clam_result == 0)
        {
            Debug.Log(System.String.Format("Error: tree for {0} not created. Check debug log file.", dataName));
            return;
        }
        _tree = new Dictionary<string, GameObject>();

        Debug.Log(System.String.Format("created tree with num nodes {0}.", ClamFFI.Clam.GetNumNodes()));


    }

    // Update is called once per frame
    void Update()
    {
    }
}
