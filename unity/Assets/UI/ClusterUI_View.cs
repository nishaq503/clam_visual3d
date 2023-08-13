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

    //SafeTextField m_DepthField;
    //SafeTextField m_CardinalityField;

    Dictionary<string, IntTextField> m_IntInputFields;
    Dictionary<string, DoubleTextField> m_DoubleInputFields;


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
        //var myDelegate = new Func<string, DialogResult>(MessageBox.Show);
        //TryDo.Do(myDelegate, null)
        m_IntInputFields = new Dictionary<string, IntTextField>
        {
            { "Depth", new IntTextField("Depth", m_UIDocument, 0, ClamFFI.TreeHeight(), new Func<bool>(InputFieldChangeCallback)) },
            { "Cardinality", new IntTextField("Cardinality", m_UIDocument, 0, ClamFFI.Cardinality(), new Func<bool>(InputFieldChangeCallback)) },

            //{ "ArgRadius", new IntTextField("ArgRadius", rightField, 0, ClamFFI.ArgRadius(), new Func < bool >(InputFieldChangeCallback)) },
            //{ "ArgCenter", new IntTextField("ArgCenter", rightField, 0, ClamFFI.ArgCenter(), new Func < bool >(InputFieldChangeCallback)) }
        };

        m_DoubleInputFields = new Dictionary<string, DoubleTextField>
        {
            { "Radius", new DoubleTextField("Radius", rightField, 0, ClamFFI.Radius()) },
            { "lfd", new DoubleTextField("lfd", rightField, 0, ClamFFI.LFD()) }
        };

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

            NodeWrapper wrapper = new NodeWrapper(cluster.GetComponent<NodeScript>().ToNodeData());
            ClamFFI.GetClusterData(wrapper);

            //foreach(var textField in m_IntInputFields)
            //{
            //    if (textField.Value.IsWithinRange(wrapper))
            //    {
            //        cluster.GetComponent<NodeScript>().Select();
            //    }
            //}
            bool valid = true;
            {
                if (m_IntInputFields.TryGetValue("Depth", out var depthField))
                {
                    //var range = textField.MinMaxRange();
                    if (!depthField.IsWithinRange(wrapper.Data.depth))
                    {
                        cluster.GetComponent<NodeScript>().Deselect();
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
                        cluster.GetComponent<NodeScript>().Deselect();

                        //cluster.GetComponent<NodeScript>().Select();
                        continue;
                    }
                }
            }

            //{
            //    if (m_IntInputFields.TryGetValue("ArgCenter", out var argCenterField))
            //    {
            //        //var range = textField.MinMaxRange();
            //        if (!argCenterField.IsWithinRange(wrapper.Data.argCenter))
            //        {
            //            cluster.GetComponent<NodeScript>().Deselect();

            //            //cluster.GetComponent<NodeScript>().Select();
            //            continue;
            //        }
            //    }
            //}

            //{
            //    if (m_IntInputFields.TryGetValue("ArgRadius", out var argRadiusField))
            //    {
            //        //var range = textField.MinMaxRange();
            //        if (!argRadiusField.IsWithinRange(wrapper.Data.argRadius))
            //        {
            //            cluster.GetComponent<NodeScript>().Deselect();

            //            continue;
            //        }
            //    }
            //}
            cluster.GetComponent<NodeScript>().Select();


            //List<Tuple<string, int>> comparisons = new List<Tuple<string, int>>();
            //comparisons.Add(new Tuple<string, int>("Depth", wrapper.Data.depth));
            //comparisons.Add(new Tuple<string, int>("Cardinality", wrapper.Data.cardinality));   
            //comparisons.Add(new Tuple<string, int>("ArgRadius", wrapper.Data.argRadius));
            //comparisons.Add(new Tuple<string, int>("ArgCenter", wrapper.Data.argCenter));

            //foreach (var comp in comparisons)
            //{
            //    if (m_IntInputFields.TryGetValue("Depth", out var textField))
            //    {
            //        var range = textField.MinMaxRange();
            //        if (wrapper.Data.depth >= range.Item1 && wrapper.Data.depth <= range.Item2)
            //        {
            //            cluster.GetComponent<NodeScript>().Select();
            //        }
            //    }
            //}

            //{
            //    if (m_IntInputFields.TryGetValue("Cardinality", out var textField))
            //    {
            //        var range = textField.MinMaxRange();
            //        if (wrapper.Data.cardinality >= range.Item1 && wrapper.Data.cardinality <= range.Item2)
            //        {
            //            cluster.GetComponent<NodeScript>().Select();
            //        }
            //    }
            //}




            //foreach (var param in m_IntInputFields.ToList())
            //{

            //    //if (wrapper.Data.depth >= m_IntInputFields["Depth"])


            //}
        }
        return true;
    }

    public void Lock()
    {

        m_IntInputFields.ToList().ForEach(item => item.Value.Lock());
        m_DoubleInputFields.ToList().ForEach(item => item.Value.Lock());
        //m_DepthField.Lock();
        //m_CardinalityField.Lock();
    }

    public void UnLock()
    {

        m_IntInputFields.ToList().ForEach(item => item.Value.UnLock());
        m_DoubleInputFields.ToList().ForEach(item => item.Value.UnLock());

        //m_DepthField.UnLock();
        //m_CardinalityField.UnLock();
    }

    public void DisplayClusterInfo(NodeDataFFI data)
    {
        m_ClusterInfo.text = data.GetInfoForUI();
    }

    public void ClearClusterInfo()
    {
        m_ClusterInfo.text = "";
    }

    public void InitClusterInfoLabel()
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

    public void SetSelectedClusterInfo(string value)
    {
        m_ClusterInfo.text = value;
    }

    public void SetTree(Dictionary<string, GameObject> tree)
    {
        Debug.Log("serting tree");
        m_Tree = tree;

        //m_Tree.ToList().ForEach(item => { item.Value.GetComponent<NodeScript>().Select(); });
    }
}

