//using Clam;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using System.Text;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.InputSystem;
//using UnityEngine.UIElements;
//using static UnityEngine.Rendering.DebugUI;


//class SafeTextField<T>
//{
//    Label m_Label;
//    TextField m_MinField;
//    TextField m_MaxField;
//    T m_MinValueThreshold;
//    T m_MaxValueThreshold;

//    //VisualTreeAsset m_Template;

//    public SafeTextField(string name, UIDocument document, T minValue, T maxValue)
//    {
//        m_MinValueThreshold = minValue;
//        m_MaxValueThreshold = maxValue;


//        m_Label = document.rootVisualElement.Q<Label>(name + "Label");
//        m_MinField = document.rootVisualElement.Q<TextField>(name + "Min");
//        m_MaxField = document.rootVisualElement.Q<TextField>(name + "Max");

//        m_MinField.value = minValue.ToString();
//        m_MaxField.value = maxValue.ToString();
//        m_Label.focusable = false;
//        m_MinField.focusable = false;
//        m_MaxField.focusable = false;


//        m_MaxField.RegisterValueChangedCallback(MaxFieldCallback);
//        m_MinField.RegisterValueChangedCallback(MaxFieldCallback);

//        m_MinField.tripleClickSelectsLine = true;
//        m_MinField.doubleClickSelectsWord = true;
//        m_MaxField.tripleClickSelectsLine = true;
//        m_MaxField.doubleClickSelectsWord = true;
//    }



//    public SafeTextField(string name, VisualElement parent, T minValue, T maxValue)
//    {
//        m_MinValueThreshold = minValue;
//        m_MaxValueThreshold = maxValue;

//        var template = Resources.Load<VisualTreeAsset>("ui/SafeInputFieldTemplate");

//        var instance = template.Instantiate();
//        parent.Add(instance);
//        m_Label = instance.Q<Label>("DataLabel");
//        m_MinField = instance.Q<TextField>("MinField");
//        m_MaxField = instance.Q<TextField>("MaxField");

//        m_Label.text = name;

//        m_MinField.value = minValue.ToString();
//        m_MaxField.value = maxValue.ToString();
//        m_Label.focusable = false;
//        m_MinField.focusable = false;
//        m_MaxField.focusable = false;


//        m_MaxField.RegisterValueChangedCallback(MaxFieldCallback);
//        m_MinField.RegisterValueChangedCallback(MinFieldCallback);

//        m_MinField.tripleClickSelectsLine = true;
//        m_MinField.doubleClickSelectsWord = true;
//        m_MaxField.doubleClickSelectsWord = true;
//        m_MaxField.doubleClickSelectsWord = true;
//    }

//    bool ValidateMinNumericInput(ChangeEvent<string> changeEvent)
//    {
//        if (!ValidateCharacters(changeEvent.newValue, "0123456789."))
//        {
//            //textField.value = changeEvent.previousValue;
//            return false;

//        }
//        else
//        {
//            if (m_MinValueThreshold.GetType() == typeof(double))
//            {

//                double minValue = (double)(object)m_MinValueThreshold;
//                double maxValue = (double)(object)m_MaxValueThreshold;
//                double curMax = double.Parse(m_MaxField.value);
//                double value = double.Parse(changeEvent.newValue);

//                if (value < minValue || value > maxValue || value > curMax)
//                {
//                    //textField.value = changeEvent.previousValue;
//                    return false;
//                }
//            }

//            else if (m_MinValueThreshold.GetType() == typeof(int))
//            {
//                if (changeEvent.newValue.Contains('.'))
//                {
//                    //textField.value = changeEvent.previousValue;
//                    return false;
//                }
//                int minValue = (int)(object)m_MinValueThreshold;
//                int maxValue = (int)(object)m_MaxValueThreshold;
//                int curMax = int.Parse(m_MaxField.value);
//                int value = int.Parse(changeEvent.newValue);

//                if (value < minValue || value > maxValue || value > curMax)
//                {
//                    //textField.value = changeEvent.previousValue;
//                    return false;
//                }
//            }

//            return true;

//        }

//    }
//    void MinFieldCallback(ChangeEvent<string> changeEvent)
//    {
//        var textField = changeEvent.target as TextField;

//        if (!ValidateMinNumericInput(changeEvent))
//        {
//            textField.value = changeEvent.previousValue;
//        }
//        else
//        {
//            // do stuff
//        }

       
//    }

//    void MaxFieldCallback(ChangeEvent<string> changeEvent)
//    {
//        var textField = changeEvent.target as TextField;

//        if (!MaxValueValidation(changeEvent))
//        {
//            textField.value = changeEvent.previousValue;
//        }
//        else
//        {
//            // do stuff
//        }
//    }

//    bool MaxValueValidation(ChangeEvent<string> changeEvent)
//    {

//        if (!ValidateCharacters(changeEvent.newValue, "0123456789."))
//        {
//            //textField.value = changeEvent.previousValue;
//            return false;

//        }
//        else
//        {
//            if (m_MinValueThreshold.GetType() == typeof(double))
//            {

//                double minValue = (double)(object)m_MinValueThreshold;
//                double maxValue = (double)(object)m_MaxValueThreshold;
//                double curMin = double.Parse(m_MinField.value);
//                double value = double.Parse(changeEvent.newValue);

//                if (value < minValue || value > maxValue || value < curMin)
//                {
//                    //textField.value = changeEvent.previousValue;
//                    return false;
//                }
//            }
//            else if (m_MinValueThreshold.GetType() == typeof(int))
//            {
//                if (changeEvent.newValue.Contains('.'))
//                {
//                    //textField.value = changeEvent.previousValue;
//                    return false;
//                }
//                int minValue = (int)(object)m_MinValueThreshold;
//                int maxValue = (int)(object)m_MaxValueThreshold;
//                int curMin = int.Parse(m_MinField.value);
//                int value = int.Parse(changeEvent.newValue);

//                if (value < minValue || value > maxValue || value < curMin)
//                {
//                    //textField.value = changeEvent.previousValue;
//                    return false;
//                }
//            }

  
//            return true;

//        }
//    }
//    bool ValidateCharacters(string value, string validCharacters)
//    {
//        foreach (var c in value)
//        {
//            if ((c < '0' || c > '9') && c != '.')
//            {
//                return false;
//            }
//        }

//        return true;
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

//    public Vector2Int MinMaxInt()
//    {
//        return new Vector2Int(int.Parse(m_MinField.value), int.Parse(m_MaxField.value));
//    }

//    public Vector2 MinMaxFloat()
//    {
//        return new Vector2(float.Parse(m_MinField.value), float.Parse(m_MaxField.value));
//    }

//    //public bool IsValid(NodeWrapper wrapper)
//    //{

//    //}
//}

