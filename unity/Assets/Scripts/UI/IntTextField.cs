using Clam;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

namespace Clam
{

    class IntTextField
    {
        Label m_Label;
        TextField m_MinField;
        TextField m_MaxField;
        MinMaxSlider m_Slider;
        int m_MinValueThreshold;
        int m_MaxValueThreshold;

        //Delegate m_Callback;
        Func<bool> m_Callback;

        //VisualTreeAsset m_Template;

        public IntTextField(string name, UIDocument document, int minValue, int maxValue, Func<bool> callback)
        {
            
            m_MinValueThreshold = minValue;
            m_MaxValueThreshold = maxValue;

            m_Callback = callback;


            m_Label = document.rootVisualElement.Q<Label>(name + "Label");
            m_MinField = document.rootVisualElement.Q<TextField>(name + "Min");
            m_MaxField = document.rootVisualElement.Q<TextField>(name + "Max");
            m_Slider = document.rootVisualElement.Q<MinMaxSlider>(name + "Slider");

            m_MinField.value = minValue.ToString();
            m_MaxField.value = maxValue.ToString();
            m_Slider.highLimit = m_Slider.maxValue = maxValue;
            m_Slider.lowLimit = m_Slider.minValue = minValue;

            m_Label.focusable = false;
            m_MinField.focusable = false;
            m_MaxField.focusable = false;
            m_Slider.focusable = false;

            m_MaxField.RegisterValueChangedCallback(MaxFieldCallback);
            m_MinField.RegisterValueChangedCallback(MinFieldCallback);
            m_Slider.RegisterValueChangedCallback(SliderCallback);

            m_MinField.tripleClickSelectsLine = true;
            m_MinField.doubleClickSelectsWord = true;
            m_MaxField.tripleClickSelectsLine = true;
            m_MaxField.doubleClickSelectsWord = true;
        }



        public IntTextField(string name, VisualElement parent, int minValue, int maxValue, Func<bool> callback)
        {
            var template = Resources.Load<VisualTreeAsset>("ui/SafeInputFieldTemplate");

            var instance = template.Instantiate();
            parent.Add(instance);
            m_MinValueThreshold = minValue;
            m_MaxValueThreshold = maxValue;

            m_Callback = callback;


            //m_Label = document.rootVisualElement.Q<Label>(name + "Label");
            //m_MinField = document.rootVisualElement.Q<TextField>(name + "Min");
            //m_MaxField = document.rootVisualElement.Q<TextField>(name + "Max");
            //m_Slider = document.rootVisualElement.Q<MinMaxSlider>(name + "Slider");

            m_MinField.value = minValue.ToString();
            m_MaxField.value = maxValue.ToString();
            m_Slider.highLimit = m_Slider.maxValue = maxValue;
            m_Slider.lowLimit = m_Slider.minValue = minValue;

            m_Label.focusable = false;
            m_MinField.focusable = false;
            m_MaxField.focusable = false;

            m_MaxField.RegisterValueChangedCallback(MaxFieldCallback);
            m_MinField.RegisterValueChangedCallback(MinFieldCallback);

            m_Slider.RegisterValueChangedCallback(SliderCallback);

            m_MinField.tripleClickSelectsLine = true;
            m_MinField.doubleClickSelectsWord = true;
            m_MaxField.tripleClickSelectsLine = true;
            m_MaxField.doubleClickSelectsWord = true;
        }

        void SliderCallback(ChangeEvent<Vector2> evt)
        {
            var slider = evt.target as MinMaxSlider;

            //Vector2 newValue = evt.newValue; // The new value from the event
            //Vector2 oldValue = evt.previousValue; // The previous value from the event

            //// Round both components based on the sign of the difference
            ////newValue.x = (newValue.x > oldValue.x) ? Mathf.Ceil(newValue.x) : Mathf.Floor(newValue.x);
            ////newValue.y = (newValue.y > oldValue.y) ? Mathf.Ceil(newValue.y) : Mathf.Floor(newValue.y);

            ////// Now you have the rounded newValue, you can use it as needed
            ////newValue = Vector2.Lerp(newValue, newValue, Time.deltaTime * 5.0f);
            ////slider.value = newValue;

            //m_MinField.value = evt.newValue.x.ToString();
            //m_MaxField.value = evt.newValue.y.ToString();
            //slider.value = evt.newValue;

            m_Callback();
        }

        bool ValidateMinNumericInput(ChangeEvent<string> changeEvent)
        {
            if (!ValidateCharacters(changeEvent.newValue, "0123456789."))
            {
                //textField.value = changeEvent.previousValue;
                return false;

            }
            else
            {
                //if (m_MinValueThreshold.GetType() == typeof(double))
                //{

                //    double minValue = (double)(object)m_MinValueThreshold;
                //    double maxValue = (double)(object)m_MaxValueThreshold;
                //    double curMax = double.Parse(m_MaxField.value);
                //    double value = double.Parse(changeEvent.newValue);

                //    if (value < minValue || value > maxValue || value > curMax)
                //    {
                //        //textField.value = changeEvent.previousValue;
                //        return false;
                //    }
                //}

                {
                    if (changeEvent.newValue.Contains('.'))
                    {
                        //textField.value = changeEvent.previousValue;
                        return false;
                    }
                    int minValue = (int)(object)m_MinValueThreshold;
                    int maxValue = (int)(object)m_MaxValueThreshold;
                    int curMax = int.Parse(m_MaxField.value);
                    int value = int.Parse(changeEvent.newValue);

                    if (value < minValue || value > maxValue || value > curMax)
                    {
                        //textField.value = changeEvent.previousValue;
                        return false;
                    }
                }

                return true;

            }

        }


        void MinFieldCallback(ChangeEvent<string> changeEvent)
        {
            var textField = changeEvent.target as TextField;

            if (!ValidateMinNumericInput(changeEvent))
            {
                textField.value = changeEvent.previousValue;
            }
            else
            {
                // do stuff
                m_Slider.minValue = int.Parse(textField.value);

                m_Callback();


            }


        }

        void MaxFieldCallback(ChangeEvent<string> changeEvent)
        {
            var textField = changeEvent.target as TextField;

            if (!MaxValueValidation(changeEvent))
            {
                textField.value = changeEvent.previousValue;
            }
            else
            {
                // do stuff
                m_Slider.maxValue = int.Parse(textField.value);
                m_Callback();
            }
        }

        bool MaxValueValidation(ChangeEvent<string> changeEvent)
        {

            if (!ValidateCharacters(changeEvent.newValue, "0123456789."))
            {
                //textField.value = changeEvent.previousValue;
                return false;

            }
            else
            {

                {
                    if (changeEvent.newValue.Contains('.'))
                    {
                        //textField.value = changeEvent.previousValue;
                        return false;
                    }
                    int minValue = (int)(object)m_MinValueThreshold;
                    int maxValue = (int)(object)m_MaxValueThreshold;
                    int curMin = int.Parse(m_MinField.value);
                    int value = int.Parse(changeEvent.newValue);

                    if (value < minValue || value > maxValue || value < curMin)
                    {
                        //textField.value = changeEvent.previousValue;
                        return false;
                    }
                }


                return true;

            }
        }
        bool ValidateCharacters(string value, string validCharacters)
        {
            foreach (var c in value)
            {
                if ((c < '0' || c > '9'))
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

        public Tuple<int, int> MinMaxRange()
        {
            return new Tuple<int, int>(int.Parse(m_MinField.value), int.Parse(m_MaxField.value));
        }

        public bool IsWithinRange(FFI.ClusterDataWrapper wrapper)
        {

            List<Tuple<string, int>> comparisons = new List<Tuple<string, int>>();
            comparisons.Add(new Tuple<string, int>("Depth", wrapper.Data.depth));
            comparisons.Add(new Tuple<string, int>("Cardinality", wrapper.Data.cardinality));
            comparisons.Add(new Tuple<string, int>("ArgRadius", wrapper.Data.argRadius));
            comparisons.Add(new Tuple<string, int>("ArgCenter", wrapper.Data.argCenter));

            foreach ((string name, int value) in comparisons)
            {
                if (m_Label.text == name)
                {
                    Debug.Log("foudn comp for " + name);
                    (int min, int max) = MinMaxRange();
                    if (value < min || value > max)
                    {
                        return false;
                    }
                }
            }

            return true;

            //if (m_Label.text == "Depth")
            //{
            //    var range = MinMaxRange();
            //    if (wrapper.Data.depth < range.Item1 || wrapper.Data.depth > range.Item2)
            //    {
            //        return false;
            //    }
            //}
            //else if(m_Label.text == "Cardinality")
        }

        public bool IsWithinRange(int value)
        {

            (int min, int max) = MinMaxRange();
            //if (value < min || value > max)
            //{
            //    return false;
            //}
            //return true;

            return (value >= min && value <= max);
        }


    }

}

