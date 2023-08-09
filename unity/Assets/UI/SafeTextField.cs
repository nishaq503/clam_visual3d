using Clam;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


class SafeTextField
{
    Label m_Label;
    TextField m_MinField;
    TextField m_MaxField;
    int m_MinValueThreshold;
    int m_MaxValueThreshold;

    VisualTreeAsset m_Template;

    public SafeTextField(string name, UIDocument document, int minValue, int maxValue)
    {
        m_MinValueThreshold = minValue;
        m_MaxValueThreshold = maxValue;


        m_Label = document.rootVisualElement.Q<Label>(name + "Label");
        m_MinField = document.rootVisualElement.Q<TextField>(name + "Min");
        m_MaxField = document.rootVisualElement.Q<TextField>(name + "Max");

        m_MinField.value = minValue.ToString();
        m_MaxField.value = maxValue.ToString();
        m_Label.focusable = false;
        m_MinField.focusable = false;
        m_MaxField.focusable = false;


        m_MaxField.RegisterValueChangedCallback(MaxIntegerValidation);
        m_MinField.RegisterValueChangedCallback(MinIntegerValidation);

        m_MinField.tripleClickSelectsLine = true;
        m_MinField.doubleClickSelectsWord = true;
        m_MaxField.doubleClickSelectsWord = true;
        m_MaxField.doubleClickSelectsWord = true;
    }

    public SafeTextField(string name, VisualElement element, int minValue, int maxValue)
    {
        m_MinValueThreshold = minValue;
        m_MaxValueThreshold = maxValue;

        //m_Template = template;
        //var instance = m_Template.Instantiate();
        m_Template = Resources.Load<VisualTreeAsset>("ui/SafeInputFieldTemplate");
        var cube = Resources.Load("cube");
        

        if (m_Template == null)
        {
            Debug.Log("what the fuck");

            m_Template = Resources.Load<VisualTreeAsset>("Resources/ui/SafeInputFieldTemplate.uxml");

            if (m_Template == null)
            {
                Debug.Log("what the fuck again");
            }

            m_Template = Resources.Load<VisualTreeAsset>("Scripts/Resources/SafeInputFieldTemplate.uxml");

            if (m_Template == null)
            {
                Debug.Log("what the fuck again2");
            }
        }

        var instance = m_Template.Instantiate();
        element.Add(instance);
        m_Label = instance.Q<Label>("DataLabel");
        m_MinField = instance.Q<TextField>("MinField");
        m_MaxField = instance.Q<TextField>("MaxField");

        m_Label.text = name;
       

        //element.Add(instance);
        //element.Add(new Label(name));
        //element.Add(new TextField(name + "Min"));
        //element.Add(new TextField(name + "Max"));
        //element.Add(new SafeTextFieldElement(name + "Max"));

        //m_Label = element.Q<Label>(name + "Label");
        //m_MinField = element.Q<TextField>(name + "Min");
        //m_MaxField = element.Q<TextField>(name + "Max");

        m_MinField.value = minValue.ToString();
        m_MaxField.value = maxValue.ToString();
        m_Label.focusable = false;
        m_MinField.focusable = false;
        m_MaxField.focusable = false;


        m_MaxField.RegisterValueChangedCallback(MaxIntegerValidation);
        m_MinField.RegisterValueChangedCallback(MinIntegerValidation);

        m_MinField.tripleClickSelectsLine = true;
        m_MinField.doubleClickSelectsWord = true;
        m_MaxField.doubleClickSelectsWord = true;
        m_MaxField.doubleClickSelectsWord = true;
    }

    void MinIntegerValidation(ChangeEvent<string> changeEvent)
    {
        var textField = changeEvent.target as TextField;

        if (!ValidateCharacters(changeEvent.newValue, "0123456789."))
        {
            textField.value = changeEvent.previousValue;

        }
        else
        {
            int value = int.Parse(changeEvent.newValue);
            if (value < m_MinValueThreshold || value > m_MaxValueThreshold || value > int.Parse(m_MinField.value))
            {
                textField.value = changeEvent.previousValue;

            }
        }
    }

    void MaxIntegerValidation(ChangeEvent<string> changeEvent)
    {
        var textField = changeEvent.target as TextField;

        if (!ValidateCharacters(changeEvent.newValue, "0123456789"))
        {
            textField.value = changeEvent.previousValue;

        }
        else
        {
            int value = int.Parse(changeEvent.newValue);
            if (value < m_MinValueThreshold || value > m_MaxValueThreshold || value < int.Parse(m_MinField.value))
            {
                textField.value = changeEvent.previousValue;

            }
        }
    }
    bool ValidateCharacters(string value, string validCharacters)
    {
        foreach (var c in value)
        {
            if (c < '0' || c > '9')
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