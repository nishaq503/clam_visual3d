using Clam.FFI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class SideMenu : MonoBehaviour
{
    public List<VisualTreeAsset> m_TreeAssets;
    UIDocument m_UIDocument;

    string m_CurrentMenuName = "TreeMenu";

    DropdownField m_DropdownField;

    ClusterMenu m_ClusterMenu;
    GraphBuildMenu m_GraphBuildMenu;

    // Start is called before the first frame update
    void Start()
    {
        m_UIDocument = GetComponent<UIDocument>();
        m_DropdownField = m_UIDocument.rootVisualElement.Q<DropdownField>("MenuSelector");
        m_DropdownField.focusable = false;

        m_DropdownField.choices = new List<string>()
        {
            "TreeMenu", "ClusterMenu", "GraphBuildMenu"
        };

        var rightField = m_UIDocument.rootVisualElement.Q<VisualElement>("Right");

        for (int i = 0; i < m_TreeAssets.Count; i++)
        {
            var asset = m_TreeAssets[i];
            var instance = asset.Instantiate();
            if (i != 0)
            {
                instance.style.display = DisplayStyle.None;
            }

            instance.name = m_DropdownField.choices[i];
            rightField.Add(instance);
        }
        m_DropdownField.RegisterValueChangedCallback(Callback);

        m_ClusterMenu = new ClusterMenu(GetComponent<UIDocument>());
        m_GraphBuildMenu = new GraphBuildMenu(GetComponent<UIDocument>(), "GraphBuildMenu");
    }

    public void DisplayClusterInfo(ClusterData data)
    {
        m_ClusterMenu.DisplayClusterInfo(data);
    }
    public void ClearClusterInfo()
    {
        m_ClusterMenu.ClearClusterInfo();
    }

    void Callback(ChangeEvent<string> evt)
    {
        var rightFieldChildrenList = m_UIDocument.rootVisualElement.Q<VisualElement>("Right").Children().ToList();

        var currentMenu = rightFieldChildrenList.Find(x => x.name == m_CurrentMenuName);
        if (currentMenu != null)
        {
            currentMenu.style.display = DisplayStyle.None;
        }
        else
        {
            Debug.LogError("couldnt find  current menu");
        }

        var nextMenu = rightFieldChildrenList.Find(x => x.name == evt.newValue);
        if (nextMenu != null)
        {
            nextMenu.style.display = DisplayStyle.Flex;
            m_CurrentMenuName = nextMenu.name;
        }
        else
        {
            Debug.LogError("couldnt find next menu");
        }
    }

    public void Lock()
    {
        var rightField = m_UIDocument.rootVisualElement.Q<VisualElement>("Right");

        if (rightField != null)
        {
            rightField.Children().ToList().ForEach(c => c.focusable = true);
        }
        m_DropdownField.focusable = false;

        m_ClusterMenu.Lock();

    }



    public void UnLock()
    {
        var rightField = m_UIDocument.rootVisualElement.Q<VisualElement>("Right");

        if (rightField != null)
        {
            rightField.Children().ToList().ForEach(c => c.focusable = true);
        }
        m_DropdownField.focusable = true;
        m_ClusterMenu.UnLock();


    }

    // Update is called once per frame
    void Update()
    {

    }
}
