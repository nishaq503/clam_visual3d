using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateNewTree : MonoBehaviour
{
    UIDocument m_UIDocument;

    TextField m_DatasetField;
    TextField m_CardinalityField;

    Button m_CreateButton;
    Button m_BackButton;


    // Start is called before the first frame update
    void Start()
    {
        m_UIDocument = GetComponent<UIDocument>();

        //m_DatasetField = m_UIDocument.rootVisualElement.Q<Label>("DatasetField");
        //m_ClusterInfoLabel = m_UIDocument.rootVisualElement.Q<Label>("ClusterInfoLabel");
        m_BackButton = m_UIDocument.rootVisualElement.Q<Button>("BackButton");
        m_CreateButton = m_UIDocument.rootVisualElement.Q<Button>("CreateButton");
        m_BackButton.RegisterCallback<ClickEvent>(BackButtonCallback);
        m_CreateButton.RegisterCallback<ClickEvent>(CreateButtonCallback);


    }

    void BackButtonCallback(ClickEvent evt)
    {
        MenuEventManager.SwitchState(Menu.Main);
    }
    void CreateButtonCallback(ClickEvent evt)
    {
        MenuEventManager.SwitchState(Menu.StartClam);
    }

    // Update is called once per frame
    void Update()
    {

    }


}
