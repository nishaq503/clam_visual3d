

using UnityEngine;
using UnityEngine.UIElements;

public class GraphBuildMenu
{
    Button m_CreateGraph;
    Button m_HideSelected;
    Button m_HideOthers;
    Toggle m_IncludeHidden;

    public GraphBuildMenu(UIDocument document, string name)
    {
        m_CreateGraph = document.rootVisualElement.Q<Button>("CreateGraphButton");
        m_HideSelected = document.rootVisualElement.Q<Button>("HideSelected");
        m_HideOthers = document.rootVisualElement.Q<Button>("HideOthers");
        m_IncludeHidden = document.rootVisualElement.Q<Toggle>("ToggleHidden");

        m_HideOthers.RegisterCallback<ClickEvent>(HideOthersCallback);
        m_HideSelected.RegisterCallback<ClickEvent>(HideSelectedCallback);
        m_IncludeHidden.RegisterCallback<ClickEvent>(IncludeHiddenCallback);
    }

    void HideOthersCallback(ClickEvent evt)
    {
        Debug.Log("clicked hide others");
        foreach (var (key, value) in MenuEventManager.instance.GetTree())
        {
            if (!value.GetComponent<NodeScript>().Selected)
            {
                value.SetActive(false);
            }
        }
    }

    void HideSelectedCallback(ClickEvent evt)
    {
        Debug.Log("clicked hide others");
        foreach (var (key, value) in MenuEventManager.instance.GetTree())
        {
            if (value.GetComponent<NodeScript>().Selected)
            {
                value.SetActive(false);
            }
        }
    }

    void IncludeHiddenCallback(ClickEvent evt)
    {
        //foreach (var (key, value) in MenuEventManager.instance.GetTree())
        //{
        //    if (value.GetComponent<NodeScript>().Selected)
        //    {
        //        value.SetActive(false);
        //    }
        //}
        Debug.Log("toggled");
        MenuEventManager.SwitchState(Menu.IncludeHidden);
    }
}