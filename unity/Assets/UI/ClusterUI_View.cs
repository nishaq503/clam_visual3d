using Clam;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ClusterUI_View : MonoBehaviour
{
    [SerializeField]
    VisualTreeAsset m_SafeInputFieldTemplate;

    private UIDocument m_UIDocument;

    Label m_ClusterInfo;
    Label m_ClusterInfoLabel;

    //SafeTextField m_DepthField;
    //SafeTextField m_CardinalityField;

    Dictionary<string, SafeTextField> m_InputFields;


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

        m_InputFields = new Dictionary<string, SafeTextField>
        {
            { "Depth", new SafeTextField("Depth", m_UIDocument, 0, ClamFFI.TreeHeight()) },
            { "Cardinality", new SafeTextField("Cardinality", m_UIDocument, 0, ClamFFI.Cardinality()) },
            //{ "Radius", new SafeTextField("Radius", rightField, 0, ClamFFI.Radius())
            //{ "lfd", new SafeTextField("Radius", rightField, 0, ClamFFI.Radius()) }
            { "ArgRadius", new SafeTextField("ArgRadius", rightField, 0, ClamFFI.ArgRadius()) },
            { "ArgCenter", new SafeTextField("ArgCenter", rightField, 0, ClamFFI.ArgCenter()) }


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

    public void Lock()
    {

        m_InputFields.ToList().ForEach(item => item.Value.Lock());
        //m_DepthField.Lock();
        //m_CardinalityField.Lock();
    }

    public void UnLock()
    {

        m_InputFields.ToList().ForEach(item => item.Value.UnLock());

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
}

