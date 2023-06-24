using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class ClusterUI_Script
{

    VisualTreeAsset m_VisualAsset;

    TextField m_SelectedClusterInfo;
    Label m_ClusterInfo;
    MinMaxSlider m_DepthSlider;



    public void Intialize(VisualElement root, VisualTreeAsset listElementTemplate)
    {
        // Store a reference to the template for the list entries
        m_VisualAsset = listElementTemplate;

        m_SelectedClusterInfo = root.Q<TextField>("SelectedClusterInfo");
        //m_SelectedClusterInfo.selectionColor = Color.green;
        m_DepthSlider = root.Q<MinMaxSlider>("DepthSlider");

        m_ClusterInfo = root.Q<Label>("ClusterInfo");

        
        //SetSelectedClusterInfo("");
        m_SelectedClusterInfo.Unbind();
        //m_SelectedClusterInfo.style.unityTextOutlineColor = Color.red;
        // Apply the custom USS style
        //m_SelectedClusterInfo.styleSheets.Add(Resources.Load<StyleSheet>("text")); // Replace with the correct USS file path
        m_DepthSlider.label = "Depth: (" + m_DepthSlider.minValue.ToString() + ", " + m_DepthSlider.maxValue.ToString() + ")";
        SetSelectedClusterInfo("");
        //// Create a new TextField element
        //TextField textField = new TextField();

        //// Set the placeholder text
        //textField.value = "Enter text...";
        //textField.style.backgroundColor = Color.blue;
        //// Access the style of the TextField
        //var textFieldStyle = textField.style;

        //// Set the text color
        //textFieldStyle.color = Color.red; // Change to the desired color
        //// Add the TextField to the root visual element
        //// You can change 'rootVisualElement' to any other visual element you want to add the text field toD
        //root.Add(textField);
        //m_DepthSlider.RegisterCallback<ChangeEvent<Vector2>>(MyCallBack);
        //MyCallBac1234k();

    }

    public Vector2 GetDepthRange()
    {
        return m_DepthSlider.value;
    }

    public void SetSelectedClusterInfo(string value)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Selected Cluster");
        if (value == new string(""))
        {

            stringBuilder.AppendLine("id: ");
            stringBuilder.AppendLine("depth ");
            stringBuilder.AppendLine("card: ");
            stringBuilder.AppendLine("radius: ");
            stringBuilder.AppendLine("lfd: ");
            stringBuilder.AppendLine("argC: ");
            stringBuilder.AppendLine("argR: ");

            m_SelectedClusterInfo.value = stringBuilder.ToString();
            m_ClusterInfo.text = stringBuilder.ToString();

        }
        else
        {
            stringBuilder.AppendLine(value);
            m_SelectedClusterInfo.value = stringBuilder.ToString();
            m_ClusterInfo.text = stringBuilder.ToString();

        }
    }

    public MinMaxSlider DepthSlider()
    {
        return m_DepthSlider;
    }

    public Label ClusterInfo()
    {
        return m_ClusterInfo;
    }

    void MyCallBac1234k()
    {
        m_DepthSlider.RegisterValueChangedCallback<Vector2>((evt) =>
        {
            //Debug.LogFormat(" New slider values: {0} .. {1}", evt.newValue.x, evt.newValue.y);
            Debug.Log("Depth: (" + m_DepthSlider.minValue.ConvertTo<System.Int32>().ToString() + ", " + m_DepthSlider.maxValue.ConvertTo<System.Int32>().ToString() + ")");
        });


        // Register a callback for the trickle-down phase.
        //myElement.RegisterCallback<MouseDownEvent>(MyCallback, TrickleDown.TrickleDown);
    }
    void MyCallback(MouseDownEvent evt) { /* ... */ }
    void Update(EventCallback<ChangeEvent<Vector2>> cb)
    {
        Debug.Log("Depth: (" + m_DepthSlider.minValue.ConvertTo<System.Int32>().ToString() + ", " + m_DepthSlider.maxValue.ConvertTo<System.Int32>().ToString() + ")");
    }

}
