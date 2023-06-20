using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DropDownExample : MonoBehaviour
{

    public TMP_Text text;

    public void DropDownSample(int index)
    {
        switch (index)
        {
            case 0: text.text = "0"; break;
            case 1: text.text = "1"; break;
            case 2: text.text = "2"; break;
            case 3: text.text = "3"; break;
        }
    }
}
