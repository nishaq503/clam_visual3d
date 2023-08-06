using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainApp : MonoBehaviour
{

    public string dataName = "arrhythmia";
    public uint cardinality = 25;
    public GameObject nodePrefab;
    public GameObject springPrefab;
    //public GameObject clusterUI_Prefab;

    private ClamTree m_ClamTree;
    //private GameObject m_ClusterUI;




    // Start is called before the first frame update
    void Awake()
    {
        m_ClamTree = new ClamTree(nodePrefab, springPrefab, dataName, cardinality);
        //m_ClusterUI = Instantiate(clusterUI_Prefab);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
        //m_Tree = new Dictionary<string, GameObject>();
        //m_SelectedNode = null;
        Clam.ClamFFI.ShutdownClam();
    }
}
