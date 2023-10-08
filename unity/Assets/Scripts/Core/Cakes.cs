using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Clam
{
    public class Cakes : MonoBehaviour
    {
        private static GameObject m_NodePrefab;
        private static GameObject m_SpringPrefab;

        private static Cakes instance;
        private static Tree m_Tree;
        //public static TreeStartupData m_StartupData;
        //FFIError m_InitResult;
        private static bool m_Initialized = false;

        private static Cakes Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<Cakes>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(Cakes).Name;
                        instance = obj.AddComponent<Cakes>();
                        
                        //m_Tree = obj.AddComponent<Tree>();
                    }

                    
                }
                if (instance.GetComponent<Tree>() == null)
                {
                    Debug.LogWarning("tree not added yet in instance");
                    InitTree();

                }
                return instance;
            }
        }

        public static Tree Tree
        {
            get
            {
                return Instance.GetComponent<Tree>();
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                //instance = this as Cakes;
                instance = FindObjectOfType<Cakes>();
                //if (instance.GetComponent<Tree>() == null)
                //{
                //    Debug.LogWarning("tree not added yet in instance");
                //    InitTree();

                //}
                //InitTree();
                //DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("Why is destroy here?...");
            }
        }

        void OnDestroy()
        {
            Debug.Log("OnDestroy cakes");

            //Debug.Log("Application ending after " + Time.time + " seconds");
            //m_Tree = new Dictionary<string, GameObject>();
            //m_SelectedNode = null;
            //if (m_InitResult == FFIError.Ok)
            if (m_Initialized)
            {
                Clam.FFI.NativeMethods.ForceShutdownPhysics();
                Clam.FFI.NativeMethods.ShutdownClam();
                //m_InitResult = FFIError.HandleInitFailed;
            }
        }

        private static void InitTree()
        {
            Tree tree = instance.AddComponent<Tree>();
            m_SpringPrefab = Resources.Load("Spring") as GameObject;
            m_NodePrefab = Resources.Load("Node") as GameObject;


            FFIError initResult = tree.Init(m_NodePrefab, m_SpringPrefab);

            if (initResult == FFIError.Ok)
            {
                m_Initialized = true;

                MenuEventManager.instance.SetTree(Cakes.Tree.GetTree());
                //MenuEventManager.instance.GetCurrentMenu().GetComponent<ClusterUI_View>().Init();
                MenuEventManager.instance.GetCurrentMenu().GetComponent<ClusterUI_View>().SetTree(Cakes.Tree.GetTree());
            }
            else
            {
                Debug.LogError("Tree initializtion failed with error " + initResult);
                Application.Quit();
            }
        }

        void OnApplicationQuit()
        {
            Debug.Log("Application ending after " + Time.time + " seconds");
            //m_Tree = new Dictionary<string, GameObject>();
            //m_SelectedNode = null;
            //if (m_InitResult == FFIError.Ok)
            //{
            //    Clam.FFI.NativeMethods.ShutdownClam();
            //}
        }


        //// Explicit static constructor to tell C# compiler
        //// not to mark type as beforefieldinit
        //static Cakes()
        //{
        //}

        //private Cakes()
        //{
        //    m_Tree = new Dictionary<string, GameObject>();
        //}

        //public static Cakes Instance
        //{
        //    get
        //    {
        //        return instance;
        //    }
        //}
    }
}

