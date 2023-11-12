using Clam;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class LoadTree : MonoBehaviour
{
    UIDocument m_UIDocument;

    TextField m_LoadTreeField;
    DropdownField m_LoadTreeDropdownField;

    Button m_LoadButton;
    Button m_BackButton;

    string m_DataDirectory = "../data/binaries/";


    // Start is called before the first frame update
    void Start()
    {
        m_UIDocument = GetComponent<UIDocument>();

        m_LoadTreeField = m_UIDocument.rootVisualElement.Q<TextField>("LoadTreeInputField");
        m_BackButton = m_UIDocument.rootVisualElement.Q<Button>("BackButton");
        m_LoadButton = m_UIDocument.rootVisualElement.Q<Button>("LoadButton");
        m_BackButton.RegisterCallback<ClickEvent>(BackButtonCallback);
        m_LoadButton.RegisterCallback<ClickEvent>(LoadButtonCallback);

        m_LoadTreeDropdownField = m_UIDocument.rootVisualElement.Q<DropdownField>("LoadTreeDropdown");

        var dirNames = Directory.GetDirectories(m_DataDirectory);

        m_LoadTreeDropdownField.choices = dirNames.ToList();

        m_LoadTreeDropdownField.RegisterValueChangedCallback(evt =>
        {
            m_LoadTreeField.value = evt.newValue;
        });

       
    }

    void BackButtonCallback(ClickEvent evt)
    {
        MenuEventManager.SwitchState(Menu.Main);
    }


    void LoadButtonCallback(ClickEvent evt)
    {
        var validNames = Directory.GetDirectories(m_DataDirectory);
        Debug.Log("Load name options");
        foreach(var validName in validNames)
        {
            Debug.Log(validName);
        }

        string dataName = m_LoadTreeField.text;

        if (validNames.Contains(dataName))
        {
            Clam.MenuEventManager.SwitchState(Menu.LoadClam);
        }
        else
        {
            ErrorDialoguePopup();
        }
    }

    

    void ErrorDialoguePopup()
    {
        var template = Resources.Load<VisualTreeAsset>("ui/InvalidInputPopup");
        var instance = template.Instantiate();
        m_UIDocument.rootVisualElement.Add(instance);

        UIHelpers.ShowPopup(m_UIDocument.rootVisualElement, instance);

        var overlay = m_UIDocument.rootVisualElement.Q<VisualElement>("Overlay");
        overlay.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0.71f));
        var okButton = m_UIDocument.rootVisualElement.Q<Button>("PopUpOkButton");

        okButton.clickable.clicked += () =>
        {
            UIHelpers.PopupClose(m_UIDocument.rootVisualElement, m_UIDocument.rootVisualElement.Q<VisualElement>("PopUpElement"));
        };
    }

    private HashSet<string> GetAnomalyFiles(string dir)
    {
        HashSet<string> fileNames = new HashSet<string>();
        try
        {
            foreach (string f in Directory.GetFiles(dir))
            {
                Debug.Log(Path.GetFileName(f));
                var name = GetUntilOrEmpty(Path.GetFileName(f), "_");
                Debug.Log(name);

                fileNames.Add(name);
            }
            fileNames.Add("rand");
            fileNames.Add("test");


            return fileNames;
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
        return new HashSet<string>();

    }

    private string GetUntilOrEmpty(string text, string stopAt = "-")
    {
        if (!String.IsNullOrWhiteSpace(text))
        {
            int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

            if (charLocation > 0)
            {
                return text.Substring(0, charLocation);
            }
        }

        return String.Empty;
    }


    // Update is called once per frame
    void Update()
    {

    }


}
