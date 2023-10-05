using Clam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class ClusterUI_View : MonoBehaviour
{
    [SerializeField]
    VisualTreeAsset m_SafeInputFieldTemplate;

    private UIDocument m_UIDocument;
    private Dictionary<string, GameObject> m_Tree;
    Label m_ClusterInfo;
    Label m_ClusterInfoLabel;
    MenuSelector m_MenuSelector;

    //SafeTextField m_DepthField;
    //SafeTextField m_CardinalityField;

    Dictionary<string, IntTextField> m_IntInputFields;
    //Dictionary<string, DoubleTextField> m_DoubleInputFields;


    //public float radius;
    //public float lfd;
    //public int argCenter;
    //public int argRadius;


    void Start()
    {
        m_UIDocument = GetComponent<UIDocument>();

        m_ClusterInfo = m_UIDocument.rootVisualElement.Q<Label>("ClusterInfo");
        m_ClusterInfoLabel = m_UIDocument.rootVisualElement.Q<Label>("ClusterInfoLabel");

        InitClusterInfoLabel();
        var rightField = m_UIDocument.rootVisualElement.Q<VisualElement>("Right");
        m_MenuSelector = new MenuSelector(m_UIDocument, "MenuSelector");
        //m_MenuSelector = m_UIDocument.rootVisualElement.Q<DropdownField>("MenuSelector");

        //var myDelegate = new Func<string, DialogResult>(MessageBox.Show);
        //TryDo.Do(myDelegate, null)
        m_IntInputFields = new Dictionary<string, IntTextField>
        {
            { "Depth", new IntTextField("Depth", m_UIDocument, 0, FFI.TreeHeight(), new Func<bool>(InputFieldChangeCallback)) },
            { "Cardinality", new IntTextField("Cardinality", m_UIDocument, 0, FFI.Cardinality(), new Func<bool>(InputFieldChangeCallback)) },

            //{ "ArgRadius", new IntTextField("ArgRadius", rightField, 0, ClamFFI.ArgRadius(), new Func < bool >(InputFieldChangeCallback)) },
            //{ "ArgCenter", new IntTextField("ArgCenter", rightField, 0, ClamFFI.ArgCenter(), new Func < bool >(InputFieldChangeCallback)) }
        };

        //m_DoubleInputFields = new Dictionary<string, DoubleTextField>
        //{
        //    { "Radius", new DoubleTextField("Radius", rightField, 0, ClamFFI.Radius()) },
        //    { "lfd", new DoubleTextField("lfd", rightField, 0, ClamFFI.LFD()) }
        //};

        m_Tree = MenuEventManager.instance.GetTree();

        //InitMenuSelector();

        //var inputField = m_SafeInputFieldTemplate.Instantiate();

        //VisualTreeAsset uiAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ui/SafeInputFieldTemplate.uxml");
        //VisualTreeAsset uiAsset2 = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ui/MyInputField.uxml");k
        //VisualTreeAsset uiAsset3 = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ui/inputTemplate.uxml");

        //VisualElement ui = uiAsset.Instantiate();

        //rightField.Add(ui);
        //rightField.Add(uiAsset2.Instantiate());
        //rightField.Add(uiAsset3.Instantiate());
        //inputField.userData
        //inputField.Q<Label>()
        //rightField.Add(m_InputFieldTemplate.Instantiate());


        //m_DepthField = new SafeTextField("Depth", m_UIDocument, 0, ClamFFI.TreeHeight());
        //m_CardinalityField = new SafeTextField("Cardinality", m_UIDocument, 0, ClamFFI.Cardinality());
    }



    bool InputFieldChangeCallback()
    {
        foreach (var item in m_Tree.ToList())
        {
            var cluster = item.Value;
            if (!cluster.activeSelf)
            {
                continue;
            }

            ClusterWrapper wrapper = new ClusterWrapper(cluster.GetComponent<Node>().ToNodeData());
            FFI.GetClusterData(wrapper);
            {
                if (m_IntInputFields.TryGetValue("Depth", out var depthField))
                {
                    //var range = textField.MinMaxRange();
                    if (!depthField.IsWithinRange(wrapper.Data.depth))
                    {
                        cluster.GetComponent<Node>().Deselect();
                        continue;
                    }
                }
            }
            {
                if (m_IntInputFields.TryGetValue("Cardinality", out var cardField))
                {
                    //var range = textField.MinMaxRange();
                    if (!cardField.IsWithinRange(wrapper.Data.cardinality))
                    {
                        cluster.GetComponent<Node>().Deselect();

                        //cluster.GetComponent<NodeScript>().Select();
                        continue;
                    }
                }
            }

            cluster.GetComponent<Node>().Select();
        }
        return true;
    }


    public void IncludeHiddenInSelection()
    {
        foreach (var item in m_Tree.ToList())
        {
            var cluster = item.Value;
            //if (!cluster.activeSelf)
            //{
            //    continue;
            //}

            ClusterWrapper wrapper = new ClusterWrapper(cluster.GetComponent<Node>().ToNodeData());
            FFI.GetClusterData(wrapper);
            {
                if (m_IntInputFields.TryGetValue("Depth", out var depthField))
                {
                    //var range = textField.MinMaxRange();
                    if (!depthField.IsWithinRange(wrapper.Data.depth))
                    {
                        cluster.GetComponent<Node>().Deselect();
                        continue;
                    }
                }
            }
            {
                if (m_IntInputFields.TryGetValue("Cardinality", out var cardField))
                {
                    //var range = textField.MinMaxRange();
                    if (!cardField.IsWithinRange(wrapper.Data.cardinality))
                    {
                        cluster.GetComponent<Node>().Deselect();

                        //cluster.GetComponent<NodeScript>().Select();
                        continue;
                    }
                }
            }
            cluster.SetActive(true);
            cluster.GetComponent<Node>().Select();
        }
    }

    public void Lock()
    {

        m_IntInputFields.ToList().ForEach(item => item.Value.Lock());
        m_MenuSelector.Lock();
        var graphMenu = m_UIDocument.rootVisualElement.Q<VisualElement>("GraphMenuInstance");
        if (graphMenu != null)
        {
            graphMenu.Children().ToList().ForEach(c => c.focusable = false);
        }

        //m_DoubleInputFields.ToList().ForEach(item => item.Value.Lock());
        //m_DepthField.Lock();
        //m_CardinalityField.Lock();
    }



    public void UnLock()
    {

        m_IntInputFields.ToList().ForEach(item => item.Value.UnLock());
        m_MenuSelector.Unlock();

        var graphMenu = m_UIDocument.rootVisualElement.Q<VisualElement>("GraphMenuInstance");
        if (graphMenu != null)
        {
            graphMenu.Children().ToList().ForEach(c => c.focusable = true);
        }


        //m_DoubleInputFields.ToList().ForEach(item => item.Value.UnLock());

        //m_DepthField.UnLock();
        //m_CardinalityField.UnLock();
    }

    //void InitMenuSelector()
    //{
    //    m_MenuSelector.focusable = false;

    //    m_MenuSelector.choices = new List<string>()
    //    {
    //        "Cluster Details", "Graph Builder"
    //    };


    //}



    public void DisplayClusterInfo(ClusterData data)
    {
        if (m_ClusterInfo != null)
            m_ClusterInfo.text = data.GetInfoForUI();
    }

    public void ClearClusterInfo()
    {
        if (m_ClusterInfo != null)

            m_ClusterInfo.text = "";
    }

    public void InitClusterInfoLabel()
    {
        if (m_ClusterInfo != null)
        {

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("id: ");
            stringBuilder.AppendLine("depth: ");
            stringBuilder.AppendLine("card: ");
            stringBuilder.AppendLine("radius: ");
            stringBuilder.AppendLine("lfd: ");
            stringBuilder.AppendLine("argC: ");
            stringBuilder.AppendLine("argR: ");

            m_ClusterInfoLabel.text = stringBuilder.ToString();
        }
    }

    public void SetSelectedClusterInfo(string value)
    {
        if (m_ClusterInfo != null)

            m_ClusterInfo.text = value;
    }

    public void SetTree(Dictionary<string, GameObject> tree)
    {
        Debug.Log("serting tree");
        m_Tree = tree;

        //m_Tree.ToList().ForEach(item => { item.Value.GetComponent<NodeScript>().Select(); });
    }
}

