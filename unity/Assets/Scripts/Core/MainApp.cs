using Clam;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Clam
{

    public class MainApp : MonoBehaviour
    {

        //public string dataName = "arrhythmia";
        //public uint cardinality = 25;

        //public ClamTreeData treeData;
        //public GameObject nodePrefab;
        //public GameObject springPrefab;
        //public GameObject user;
        //public GameObject userPrefab;
        //public GameObject clusterUI_Prefab;

        //public ClamTree clamTree;
        //private GameObject m_ClusterUI;
        FFIError m_InitResult;
        public GameObject m_Tree;


        // Start is called before the first frame update
        void Awake()
        {

            
            //Debug.Log("tree data" + treeData.dataName + " " + treeData.cardinality);
            //GetComponent<ClamTree>().Init(nodePrefab, springPrefab, treeData.dataName, treeData.cardinality);
            //userPrefab = Instantiate(userPrefab);
            //m_ClusterUI = Instantiate(clusterUI_Prefab);

        }

        public void Quit()
        {
            // save any game data here
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

        private void Start()
        {
            //GameObject child1 = GameObject.FindChild("child1").gameObject;
            //var user = this.GetComponent<Transform>().Find("User");
            //var user = GameObject.FindWithTag("Player");
            m_InitResult = m_Tree.GetComponent<Tree>().Init();

            if (m_InitResult != FFIError.Ok)
            {
                //Application.Quit();
                Quit();
            }
            MenuEventManager.instance.SetTree(m_Tree.GetComponent<Tree>().GetTree());
            //MenuEventManager.instance.GetCurrentMenu().GetComponent<ClusterUI_View>().Init();
            MenuEventManager.instance.GetCurrentMenu().GetComponent<ClusterUI_View>().SetTree(m_Tree.GetComponent<Tree>().GetTree());


            //if (user != null)
            //{
            //    //user.GetComponent<ClamUserInput>().SetTree(GetComponent<ClamTree>().GetTree());
            //}

            //user.GetComponent<ClamUserInput>().SetTree(clamTree.GetComponent<ClamTree>().GetTree());


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
            if (m_InitResult == FFIError.Ok)
            {
                Clam.FFI.NativeMethods.ShutdownClam();
            }
        }
    }
}

