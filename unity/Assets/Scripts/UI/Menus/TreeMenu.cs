using Clam.FFI;
using Clam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TreeMenu 
{

    RadioButtonGroup m_ColorOptions;
    private UIDocument m_UIDocument;

    IntTextField m_DepthField;
    Label m_DepthValue;
    Button m_ShowLess;
    Button m_ShowMore;

    TreeLayout m_Layout;

    public TreeMenu(UIDocument uidocument)
    {
        m_UIDocument = uidocument;
        m_ColorOptions = m_UIDocument.rootVisualElement.Q<RadioButtonGroup>("TreeColorOptions");
        m_ColorOptions.RegisterValueChangedCallback(ColorChangeCallback);

        // root id
        var foundRoot = NativeMethods.GetRootData(out var rootData);
        m_Layout = new TreeLayout(rootData.Data.id.AsString);

        m_DepthField = new IntTextField("TreeDepth",m_UIDocument, 0, Clam.FFI.NativeMethods.TreeHeight(), InputFieldChangeCallback);

        m_ShowMore = m_UIDocument.rootVisualElement.Q<Button>("TreeDepthMoreButton");
        m_ShowLess = m_UIDocument.rootVisualElement.Q<Button>("TreeDepthLessButton");
        m_DepthValue = m_UIDocument.rootVisualElement.Q<Label>("TreeDepthValue");
        m_ShowMore.RegisterCallback<ClickEvent>(ShowMoreCallback);
        m_ShowLess.RegisterCallback<ClickEvent>(ShowLessCallback);
    }

    void ShowMoreCallback(ClickEvent evt)
    {
        Debug.Log("Show more");
        m_Layout.ShowMore();
        m_DepthValue.text = m_Layout.CurrentDepth().ToString();
    }
    void ShowLessCallback(ClickEvent evt)
    {
        Debug.Log("Show less");
        m_Layout.ShowLess();
        m_DepthValue.text = m_Layout.CurrentDepth().ToString();
    }

    bool InputFieldChangeCallback()
    {
        Debug.Log("tree depth clbk");
        return true;
    }

    void ColorChangeCallback(ChangeEvent<int> changeEvent)
    {
        if (changeEvent != null)
        {
            if (changeEvent.newValue == 0)
            {
                NativeMethods.ColorClustersByLabel(ColorFiller);
            }
        }
    }

    unsafe void ColorFiller(ref Clam.FFI.ClusterData nodeData)
    {
        GameObject node;

        bool hasValue = Cakes.Tree.GetTree().TryGetValue(nodeData.id.AsString, out node);
        if (hasValue)
        {
            //Debug.Log("setting color to" + nodeData.color.AsVector3.ToString());
            node.GetComponent<Node>().Deselect();
            node.GetComponent<Node>().SetActualColor(nodeData.color.AsColor);
        }
        else
        {
            Debug.LogError("cluster key not found - color filler - " + nodeData.id);
        }
    }
}
