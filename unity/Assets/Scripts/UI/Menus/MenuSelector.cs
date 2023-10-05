

using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using System.Linq;

public class MenuSelector
{
    DropdownField m_DropdownField;
    UIDocument m_Document;
    GraphBuildMenu m_GraphBuildMenu;

    public MenuSelector(UIDocument document, string name)
    {
        m_DropdownField = document.rootVisualElement.Q<DropdownField>(name);
        m_Document = document;

        m_DropdownField.focusable = false;

        m_DropdownField.choices = new List<string>()
        {
            "Cluster Details", "Graph Builder"
        };

        m_DropdownField.RegisterValueChangedCallback(Callback);

        m_GraphBuildMenu = null;
    }

    void Callback(ChangeEvent<string> evt)
    {
        var dropdowntField = evt.target as DropdownField;

        if (evt.newValue == m_DropdownField.choices[0])
        {
            //var rightField = m_Document.rootVisualElement.Q<VisualElement>("Right").Children().ToList().Find(x => x.name = "GraphBuilder");
            //m_GraphBuildMenu = null;

            var rightField = m_Document.rootVisualElement.Q<VisualElement>("Right");

            var children = rightField.Children();
            var graphMenu = children.ToList().Find(x => x.name == "GraphBuildMenuInstance");
            //rightField.Remove();
            if (graphMenu != null)
            {
                rightField.Remove(graphMenu);
            }
            else
            {
                Debug.Log("cant finf graph menu");
            }
            var clusterDetailers = children.ToList().Find(x => x.name == "ClusterInfo");

            Debug.Log("changing menu to cluster details");
            //clusterDetailers.style.width = 0;
            //clusterDetailers.style.height = 0;
            //clusterDetailers.style.width = Length.Percent(100);
            clusterDetailers.style.display = DisplayStyle.Flex;
            //clusterDetailers.style.height = Length.Percent();

            //clusterDetailers.visible = true;

        }
        else if (evt.newValue == m_DropdownField.choices[1])
        {
            Debug.Log("changing menu to graph builder");
            var rightField = m_Document.rootVisualElement.Q<VisualElement>("Right");

            if (rightField != null)
            {
                Debug.Log("found right field");
                var children = rightField.Children();
                var clusterDetailersIndex = children.ToList().FindIndex(x => x.name == "ClusterInfo");
                var clusterDetailers = children.ToList().Find(x => x.name == "ClusterInfo");


                if (clusterDetailersIndex != -1)
                {
                    //clusterDetailers.style.width = 0;
                    //clusterDetailers.style.height = 0;
                    //clusterDetailers.visible = false;
                    clusterDetailers.style.display = DisplayStyle.None;

                    Debug.Log("found cluster Details");

                    var template = Resources.Load<VisualTreeAsset>("ui/GraphBuildMenu");
                    var instance = template.Instantiate();
                    instance.name = "GraphBuildMenuInstance";

                    rightField.Add(instance);
                    if (m_GraphBuildMenu == null)
                    {
                        m_GraphBuildMenu = new GraphBuildMenu(m_Document, "GraphBuildMenuInstance");
                    }

                    //rightField.Insert(clusterDetailersIndex, new Button());

                    //rightField.RemoveAt(clusterDetailersIndex);

                }
            }

        }
        else
        {
            Debug.Log("invalid selection...somehow?");
            //dropdowntField.value = evt.previousValue;
        }
    }

    public void Lock()
    {
        m_DropdownField.focusable = false;
    }
    public void Unlock()
    {
        m_DropdownField.focusable = true;
    }
}