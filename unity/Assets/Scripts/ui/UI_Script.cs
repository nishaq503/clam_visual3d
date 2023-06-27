using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Script : MonoBehaviour
{
    public Canvas canvas;
    public TMP_Text selectedClusterInfo;
    //[SerializeField]
    //private TMP_Text m_SelectedClusterInfo;
    // Start is called before the first frame update
    void Start()
    {
        SetSelectedClusterInfo("");

        Slider slider;
    }

    // Update is called once per frame
    void Update()
    {
        
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

            selectedClusterInfo.text = stringBuilder.ToString();
            //m_ClusterInfo.text = stringBuilder.ToString();

        }
        else
        {
            stringBuilder.AppendLine(value);
            //selectedClusterInfo.value = stringBuilder.ToString();
            selectedClusterInfo.text = stringBuilder.ToString();

        }
    }
}
