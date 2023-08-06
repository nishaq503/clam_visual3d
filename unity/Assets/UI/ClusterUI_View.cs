using Clam;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;


class SafeTextField
{
    //Toggle m_Toggle;
    Label m_Label;
    TextField m_MinField;
    TextField m_MaxField;
    int m_MinValueThreshold;
    int m_MaxValueThreshold;
    //IntegerField m_IntegerField;

    public SafeTextField(string name, UIDocument document, int minValue, int maxValue)
    {
        m_MinValueThreshold = minValue;
        m_MaxValueThreshold = maxValue;


        //m_Toggle = document.rootVisualElement.Q<Toggle>(name + "Toggle");
        m_Label = document.rootVisualElement.Q<Label>(name + "Label");
        m_MinField = document.rootVisualElement.Q<TextField>(name + "Min");
        m_MaxField = document.rootVisualElement.Q<TextField>(name + "Max");

        m_MinField.value = minValue.ToString();
        m_MaxField.value = maxValue.ToString();
        //m_Toggle.value = false;
        //m_Toggle.focusable = false;
        m_Label.focusable = false;
        m_MinField.focusable = false;
        m_MaxField.focusable = false;


        m_MaxField.RegisterValueChangedCallback(IntegerValidation);
        m_MinField.RegisterValueChangedCallback(IntegerValidation);
    }

    void IntegerValidation(ChangeEvent<string> changeEvent)
    {
        var textField = changeEvent.target as TextField;
        //if (!int.TryParse(changeEvent.newValue, out int value))
        //{
        //    textField.value = changeEvent.previousValue;
        //}
        ////else
        ////{
        ////    if ( value < m_MinValueThreshold || value > m_MaxValueThreshold)
        ////    {
        ////        textField.value = changeEvent.previousValue;
        ////    }
        ////}

        if (!ValidateCharacters(changeEvent.newValue, "0123456789"))
        {
            textField.value = changeEvent.previousValue;

        }
        else
        {
            int value = int.Parse(changeEvent.newValue);
            if (value < m_MinValueThreshold || value > m_MaxValueThreshold)
            {
                textField.value = changeEvent.previousValue;

            }
        }
    }
    bool ValidateCharacters(string value, string validCharacters)
    {
        foreach (var c in value)
        {
            if( c < '0' || c > '9')
            {
                return false;
            }
        }

        return true;
    }

    public void Lock()
    {
        m_MinField.focusable = m_MaxField.focusable = false;
        m_MaxField.isReadOnly = m_MinField.isReadOnly = true;
    }

    public void UnLock()
    {
        m_MinField.focusable = m_MaxField.focusable = true;
        m_MaxField.isReadOnly = m_MinField.isReadOnly = false;
    }
}

//class MinMaxIntField
//{
//    public Label m_Label;
//    public IntegerField m_MinField;
//    public IntegerField m_MaxField;

//    public MinMaxIntField(string name, UIDocument document)
//    {
//        var depth = document.rootVisualElement.Q<VisualElement>(name);
//        m_Label = new Label(name);
//        m_Label.name = name + "Label";
//        depth.Add(m_Label);

//        m_MinField = new IntegerField();
//        m_MinField.name = name + "Min";
//        depth.Add(m_MinField);
        
//        m_MaxField = new IntegerField();
//        m_MaxField.name = name + "Min";
//        depth.Add(m_MinField);

//        m_Label.focusable = false;
//        m_MinField.focusable = false;
//        m_MaxField.focusable = false;
//    }

//    public void Lock()
//    {
//        m_MinField.focusable = m_MaxField.focusable = false;
//        m_MaxField.isReadOnly = m_MinField.isReadOnly = true;
//    }

//    public void UnLock()
//    {
//        m_MinField.focusable = m_MaxField.focusable = true;
//        m_MaxField.isReadOnly = m_MinField.isReadOnly = false;
//    }
//}

public class ClusterUI_View : MonoBehaviour
{
    [SerializeField]
    VisualTreeAsset m_ListEntryTemplate;

    private UIDocument m_UIDocument;

    VisualTreeAsset m_VisualAsset;

    //TextField m_SelectedClusterInfo;
    Label m_ClusterInfo;
    Label m_ClusterInfoLabel;
    Button m_Button;
    Toggle m_Toggle;
    TextField m_TextField;
    private List<GameObject> m_SelectedClusters;

    VisualElement depth;

    SafeTextField m_DepthField;
    SafeTextField m_CardinalityField;

    void Start()
    {
        Debug.Log("hello from cluster_View");
        m_SelectedClusters = new List<GameObject>();
        m_UIDocument = GetComponent<UIDocument>();

        m_ClusterInfo = m_UIDocument.rootVisualElement.Q<Label>("ClusterInfo");
        m_ClusterInfoLabel = m_UIDocument.rootVisualElement.Q<Label>("ClusterInfoLabel");
        //m_Button = m_UIDocument.rootVisualElement.Q<Button>("MyButton");
        //m_Toggle = m_UIDocument.rootVisualElement.Q<Toggle>("MyToggle");
        m_TextField = m_UIDocument.rootVisualElement.Q<TextField>("MyTextField");

        InitClusterInfoLabel();

        m_DepthField = new SafeTextField("Depth", m_UIDocument, 0, ClamFFI.TreeHeight());
        m_CardinalityField = new SafeTextField("Cardinality", m_UIDocument, 0, ClamFFI.Cardinality());
    }

    public void Lock()
    {
        m_DepthField.Lock();
        m_CardinalityField.Lock();
    }

    public void UnLock()
    {
        m_DepthField.UnLock();
        m_CardinalityField.UnLock();
    }

    public void DisplayClusterInfo(NodeDataFFI data)
    {
        m_ClusterInfo.text = data.GetInfoForUI();
    }

    public void ClearClusterInfo()
    {
        m_ClusterInfo.text = "";
    }

    void InitializeTextFields()
    {

    }

    private void OnFocusInTextField(FocusInEvent evt)
    {
        // If the text field just received focus and the user might want to write
        // or edit the text inside, the placeholder text should be cleared (if active)

        var textField = evt.target as TextField;
        textField.value = "";

    }

    void PrintHello()
    {
        Debug.Log("hello from button!");
    }

    private void RegisterHandler(Button button)
    {
        //button.RegisterCallback<ClickEvent>(PrintClickMessage);
        //button.RegisterValueChangedCallback( (evt) => { Debug.Log("Change Event received"); });
        button.RegisterCallback<MouseDownEvent>(evt => { Debug.Log("testing for fucks sake work"); });
    }

    private void PrintClickMessage(ClickEvent evt)
    {

        //Because of the names we gave the buttons and toggles, we can use the
        //button name to find the toggle name.
        Button button = evt.currentTarget as Button;

        Debug.Log("Button was clicked!");
    }

    public void InitClusterInfoLabel()
    {
        StringBuilder stringBuilder = new StringBuilder();
        //stringBuilder.AppendLine("Selected Cluster");

        stringBuilder.AppendLine("id: ");
        stringBuilder.AppendLine("depth: ");
        stringBuilder.AppendLine("card: ");
        stringBuilder.AppendLine("radius: ");
        stringBuilder.AppendLine("lfd: ");
        stringBuilder.AppendLine("argC: ");
        stringBuilder.AppendLine("argR: ");

        //m_SelectedClusterInfo.value = stringBuilder.ToString();
        m_ClusterInfoLabel.text = stringBuilder.ToString();

    }

    public void SetSelectedClusterInfo(string value)
    {
        m_ClusterInfo.text = value;
    }

}