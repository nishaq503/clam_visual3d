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
    MinMaxSlider m_DepthSlider;



    public void Intialize(VisualElement root, VisualTreeAsset listElementTemplate)
    {
        // Store a reference to the template for the list entries
        m_VisualAsset = listElementTemplate;

        m_SelectedClusterInfo = root.Q<TextField>("SelectedClusterInfo");
        m_DepthSlider = root.Q<MinMaxSlider>("DepthSlider");

        SetSelectedClusterInfo("");
        m_SelectedClusterInfo.Unbind();

        m_DepthSlider.label = "Depth: (" + m_DepthSlider.minValue.ToString() + ", " + m_DepthSlider.maxValue.ToString() + ")";

        //m_DepthSlider.RegisterCallback<ChangeEvent<Vector2>>(MyCallBack);
        //MyCallBac1234k();

    }

    public Vector2 GetDepthRange()
    {
        return m_DepthSlider.value;
    }

    public void SetSelectedClusterInfo(string value)
    {
        if (value == new string(""))
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("id: ");
            stringBuilder.AppendLine("depth ");
            stringBuilder.AppendLine("card: ");
            stringBuilder.AppendLine("radius: ");
            stringBuilder.AppendLine("lfd: ");
            stringBuilder.AppendLine("argC: ");
            stringBuilder.AppendLine("argR: ");

            m_SelectedClusterInfo.value = stringBuilder.ToString();
        }
        else
        {
            m_SelectedClusterInfo.value = value;

        }
    }

    public MinMaxSlider DepthSlider()
    {
        return m_DepthSlider;
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
