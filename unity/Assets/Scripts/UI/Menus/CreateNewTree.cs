using Clam;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateNewTree : MonoBehaviour
{
    UIDocument m_UIDocument;

    TextField m_DatasetField;
    TextField m_CardinalityField;
    DropdownField m_DropdownField;

    Button m_CreateButton;
    Button m_BackButton;

    string m_DataDirectory = "../data/anomaly_data/preprocessed";


    // Start is called before the first frame update
    void Start()
    {
        m_UIDocument = GetComponent<UIDocument>();

        m_DatasetField = m_UIDocument.rootVisualElement.Q<TextField>("DatasetInputField");
        m_CardinalityField = m_UIDocument.rootVisualElement.Q<TextField>("CardinalityInputField");
        m_BackButton = m_UIDocument.rootVisualElement.Q<Button>("BackButton");
        m_CreateButton = m_UIDocument.rootVisualElement.Q<Button>("CreateButton");
        m_BackButton.RegisterCallback<ClickEvent>(BackButtonCallback);
        m_CreateButton.RegisterCallback<ClickEvent>(CreateButtonCallback);

        m_DropdownField = m_UIDocument.rootVisualElement.Q<DropdownField>("DatasetDropdown");

        var fileNames = GetFilesInDirectory(m_DataDirectory);

        m_DropdownField.choices = fileNames.ToList();

        m_DropdownField.RegisterValueChangedCallback(evt =>
        {
            m_DatasetField.value = evt.newValue;
        });


    }

    void DropdownCallback(EventCallback<ChangeEvent<string>> evt)
    {

    }
    void BackButtonCallback(ClickEvent evt)
    {
        MenuEventManager.SwitchState(Menu.Main);
    }
    void CreateButtonCallback(ClickEvent evt)
    {
        var validNames = GetFilesInDirectory(m_DataDirectory);
        validNames.Add("rand");
        validNames.Add("test");

        string dataName = m_DatasetField.text;

        if (uint.TryParse(m_CardinalityField.text ,out uint cardinality) && validNames.Contains(dataName))
        {
            Clam.MenuEventManager.SwitchState(Menu.StartClam);
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

    private HashSet<string> GetFilesInDirectory(string dir)
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
