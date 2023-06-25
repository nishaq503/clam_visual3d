using ClamFFI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

class MyMinMaxSlider
{
    public TextField min;
    public TextField max;
    public MinMaxSlider slider;

    public void UpdateFromSlider(Vector2 newValue)
    {
        min.value = newValue.x.ToString();
        max.value = newValue.y.ToString();
    }

    public MyMinMaxSlider(VisualElement root, string sliderTag, string minTag, string maxTag)
    {
        this.slider = root.Q<MinMaxSlider>(sliderTag);
        this.min = root.Q<TextField>(minTag);
        this.max = root.Q<TextField>(maxTag);
        this.min.value = this.slider.value.x.ToString();
        this.max.value = this.slider.value.y.ToString();

        this.min.doubleClickSelectsWord = true;
        this.max.doubleClickSelectsWord = true;
    }

    public void UpdateFromText(string key, string newValue)
    {
        if (key == "min")
        {
            slider.value = new Vector2(float.Parse(newValue), slider.value.y);
        }
        else
        {
            slider.value = new Vector2(slider.value.x, float.Parse(newValue));
        }

    }
}

public class ClusterUI_Script
{

    VisualTreeAsset m_VisualAsset;

    //TextField m_SelectedClusterInfo;
    Label m_ClusterInfo;
    Label m_ClusterInfoLabel;
    MinMaxSlider m_DepthSlider;
    MinMaxSlider m_CardinalitySlider;

    Dictionary<string, MyMinMaxSlider> m_Sliders;

    //void RemoveSpaces()
    //{
    //    inputField.text = inputField.text.Replace(" ", "");
    //}

    public void Initialize(VisualElement root, VisualTreeAsset listElementTemplate)
    {


        m_Sliders = new Dictionary<string, MyMinMaxSlider>();
        // Store a reference to the template for the list entries
        m_VisualAsset = listElementTemplate;



        m_Sliders.Add("Depth", new MyMinMaxSlider(root, "DepthSlider", "MinDepthText", "MaxDepthText"));
        m_Sliders.Add("Card", new MyMinMaxSlider(root, "CardSlider", "MinCardText", "MaxCardText"));

        //m_Sliders.ToList().ForEach(value => value.Value.min.Re.onValueChanged.AddListener(delegate { RemoveSpaces(); });
        //m_Sliders.ToList().ForEach(n => n.Value.slider.maxValue = n.Value.slider.highLimit = 1000);
        //m_Sliders.ToList().ForEach(n => n.Value.slider.minValue = n.Value.slider.lowLimit = 0);
        float min = 0.0f;
        float max = 1000.0f;
        foreach (var item in m_Sliders)
        {
            item.Value.slider.minValue = item.Value.slider.lowLimit = min;
            item.Value.slider.maxValue = item.Value.slider.highLimit = max;
            item.Value.min.value = min.ToString();
            item.Value.max.value = max.ToString();
        }

        m_ClusterInfo = root.Q<Label>("ClusterInfo");
        //var depthMinDropDown = root.Q<Dropdown>("DepthMin");
        //depthMinDropDown.
        m_ClusterInfoLabel = root.Q<Label>("ClusterInfoLabel");
        //m_DepthSlider.Unbind();

        //foreach (var slider in m_Sliders)
        //{
        //    slider.Value.label = slider.Key + ": (" + slider.Value.minValue.ToString() + ", " + slider.Value.maxValue.ToString() + ")";
        //}
        InitClusterInfoLabel();

        GameObject tree = GameObject.FindWithTag("Tree");

        m_Sliders.ToList().ForEach(item =>
        {
            item.Value.slider.RegisterValueChangedCallback<Vector2>((evt) =>
            {
                //slider.Value.label = slider.Key + ": (" + ((int)evt.newValue.x).ToString() + ", " + ((int)evt.newValue.y).ToString() + ")";
                tree.GetComponent<TreeScript>().SetActivityInRange(evt.newValue, item.Key);
                item.Value.min.value = item.Value.slider.value.x.ToString();
                item.Value.max.value = item.Value.slider.value.y.ToString();

            });
            //item.Value.min.r
            item.Value.min.RegisterValueChangedCallback<string>((evt) =>
            {
                
                //slider.Value.label = slider.Key + ": (" + ((int)evt.newValue.x).ToString() + ", " + ((int)evt.newValue.y).ToString() + ")";
                try
                {
                    item.Value.max.focusable = true;
                    item.Value.min.focusable = true;
                    float newMin = float.Parse(evt.newValue);
                    item.Value.slider.value = new Vector2(newMin, item.Value.slider.value.y);
                    tree.GetComponent<TreeScript>().SetActivityInRange(item.Value.slider.value, item.Key);
                }
                catch
                {
                    Debug.Log("Invalid float");
                    evt.StopPropagation();
                    item.Value.min.value = evt.previousValue;

                    //item.Value.max.focusable = false;
                    //item.Value.min.focusable = false;
                    //item.Value.min.
                }
                //item.Value.min.value = item.Value.slider.value.x.ToString();
                //item.Value.max.value = item.Value.slider.value.y.ToString();

            });

            item.Value.max.RegisterValueChangedCallback<string>((evt) =>
            {
                //slider.Value.label = slider.Key + ": (" + ((int)evt.newValue.x).ToString() + ", " + ((int)evt.newValue.y).ToString() + ")";
                try
                {

                    item.Value.max.focusable = true;
                    item.Value.min.focusable = true;
                    float newMax = float.Parse(evt.newValue);
                    item.Value.slider.value = new Vector2(item.Value.slider.value.x, newMax);
                    tree.GetComponent<TreeScript>().SetActivityInRange(item.Value.slider.value, item.Key);

                }
                catch
                {
                    Debug.Log("Invalid float");
                    item.Value.max.focusable = false;
                    item.Value.min.focusable = false;
                    evt.StopPropagation();
                    item.Value.min.value = evt.previousValue;

                }
                //item.Value.min.value = item.Value.slider.value.x.ToString();
                //item.Value.max.value = item.Value.slider.value.y.ToString();

            });

            item.Value.max.RegisterCallback<KeyDownEvent>(ev =>
            {

                // Open TextField
                // Stop propagation
                var ignoreCodes = new List<KeyCode>(){
                    KeyCode.W, KeyCode.S, KeyCode.D, KeyCode.A }
                ;
                foreach (KeyCode code in ignoreCodes)
                {
                    if (ev.keyCode == code)
                    {

                        ev.StopPropagation();
                        ev.PreventDefault();
                    }

                }

            }, TrickleDown.TrickleDown);

            item.Value.min.RegisterCallback<KeyUpEvent>(ev =>
            {

                // Open TextField
                // Stop propagation
                var ignoreCodes = new List<KeyCode>(){
                    KeyCode.W, KeyCode.S, KeyCode.D, KeyCode.A }
                ;
                foreach (KeyCode code in ignoreCodes)
                {
                    if (ev.keyCode == code)
                    {

                        ev.StopPropagation();
                        ev.PreventDefault();
                    }

                }

            }, TrickleDown.TrickleDown);

        });
       
        //m_ClusterUIScript.DepthSlider().RegisterValueChangedCallback<Vector2>((evt) =>
        //{

        //    m_ClusterUIScript.DepthSlider().label = "Depth: (" + ((int)evt.newValue.x).ToString() + ", " + ((int)evt.newValue.y).ToString() + ")";
        //    GetTree().SetDepthRange(evt.newValue);
        //});


    }

    public Vector2 GetDepthRange()
    {
        return m_DepthSlider.value;
    }

    public void InitClusterInfoLabel()
    {
        StringBuilder stringBuilder = new StringBuilder();
        //stringBuilder.AppendLine("Selected Cluster");

        stringBuilder.AppendLine("id: ");
        stringBuilder.AppendLine("depth: ");
        stringBuilder.AppendLine("card: ");
        stringBuilder.AppendLine("radius: ");
        stringBuilder.AppendLine("lfd: ");
        stringBuilder.AppendLine("argC: ");
        stringBuilder.AppendLine("argR: ");

        //m_SelectedClusterInfo.value = stringBuilder.ToString();
        m_ClusterInfoLabel.text = stringBuilder.ToString();

    }
    public void SetSelectedClusterInfo(string value)
    {
        //StringBuilder stringBuilder = new StringBuilder();
        //stringBuilder.AppendLine("Selected Cluster");
        //if (value == new string(""))
        //{

        //    stringBuilder.AppendLine("id: ");
        //    stringBuilder.AppendLine("depth: ");
        //    stringBuilder.AppendLine("card: ");
        //    stringBuilder.AppendLine("radius: ");
        //    stringBuilder.AppendLine("lfd: ");
        //    stringBuilder.AppendLine("argC: ");
        //    stringBuilder.AppendLine("argR: ");

        //    //m_SelectedClusterInfo.value = stringBuilder.ToString();
        //    m_ClusterInfo.text = stringBuilder.ToString();

        //}
        //else
        //{
        //stringBuilder.AppendLine(value);
        //m_SelectedClusterInfo.value = stringBuilder.ToString();
        m_ClusterInfo.text = value;

        //}
    }

    //public MinMaxSlider DepthSlider()
    //{
    //    return m_Sliders.GetValueOrDefault("Depth");
    //}
    //public MinMaxSlider CardinalitySlider()
    //{
    //    return m_Sliders.GetValueOrDefault("Card");
    //}

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
