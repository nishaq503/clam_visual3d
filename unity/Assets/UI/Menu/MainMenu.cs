using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
 

    UIDocument m_UIDocument;

    Button m_CreateNewButton;
    //Button m_ImportButton;
    Button m_ExitButton;


    // Start is called before the first frame update
    void Start()
    {
        m_UIDocument = GetComponent<UIDocument>();

        m_CreateNewButton = m_UIDocument.rootVisualElement.Q<Button>("CreateTree");
        m_ExitButton = m_UIDocument.rootVisualElement.Q<Button>("ExitButton");

        SetExitButtonCallback(m_ExitButton);
        SetCreateButtonCallback();
    }

    private void SetCreateButtonCallback()
    {
        m_CreateNewButton.RegisterCallback<ClickEvent>(CreateButtonCallback);
    }

    private void CreateButtonCallback(ClickEvent evt)
    {
        Button button = evt.currentTarget as Button;

        MenuEventManager.SwitchState(Menu.CreateNewTree);

    }

    private void SetExitButtonCallback(Button button)
    {
        button.RegisterCallback<ClickEvent>(ExitButtonCallback);
    }

    private void ExitButtonCallback(ClickEvent evt)
    {
        Button button = evt.currentTarget as Button;


        evt.StopImmediatePropagation();

        var template = Resources.Load<VisualTreeAsset>("ui/AreYouSure");
        var instance = template.Instantiate();
        //m_UIDocument.rootVisualElement.Add(instance);
        
        UIHelpers.ShowPopup(m_UIDocument.rootVisualElement, instance);

        var overlay = m_UIDocument.rootVisualElement.Q<VisualElement>("Overlay");
        overlay.style.backgroundColor = new StyleColor(new Color(0,0,0,0.71f));
        var yesButton = m_UIDocument.rootVisualElement.Q<Button>("PopUpYesButton");
        var noButton = m_UIDocument.rootVisualElement.Q<Button>("PopUpNoButton");

        noButton.clickable.clicked += () =>
        {
            UIHelpers.PopupClose(m_UIDocument.rootVisualElement, m_UIDocument.rootVisualElement.Q<VisualElement>("PopUpElement"));
        };

        yesButton.clickable.clicked += () =>
        {
            Application.Quit();
        };

    }


    // Update is called once per frame
    void Update()
    {

    }

    //public static void ShowPopup(VisualElement rootElementForPopup,
    //    VisualElement popupContent,
    //    float widthInPercents = 100.0f,
    //    float heightInPercents = 100.0f)
    //{
    //    if (widthInPercents <= 0f || widthInPercents > 100f)
    //    {
    //        throw new ArgumentException($"Width should be in the range 0 < width < 100.", "widthInPercents");
    //    }

    //    if (heightInPercents <= 0f || heightInPercents > 100f)
    //    {
    //        throw new ArgumentException($"Height should be in the range 0 < height < 100.", "heightInPercents");
    //    }

    //    //Create visual element for popup
    //    var popupElement = new VisualElement();
    //    popupElement.name = "PopUpElement";
    //    popupElement.style.position = new StyleEnum<Position>(Position.Absolute);
    //    popupElement.style.top = 0;
    //    popupElement.style.left = 0;
    //    popupElement.style.flexGrow = new StyleFloat(1);
    //    popupElement.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
    //    popupElement.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

    //    //Popup background is button so that the popup is closed when the player
    //    //clicks anywhere outside the popup.
    //    //var backgroundButton = new Button();
    //    //backgroundButton.style.position = new StyleEnum<Position>(Position.Absolute);
    //    //backgroundButton.style.top = 0;
    //    //backgroundButton.style.left = 0;
    //    //backgroundButton.style.flexGrow = new StyleFloat(1);
    //    //backgroundButton.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
    //    //backgroundButton.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
    //    //backgroundButton.style.opacity = new StyleFloat(0.4f);
    //    //backgroundButton.style.backgroundColor = new StyleColor(Color.black);
    //    //backgroundButton.text = string.Empty;

        
    //    //popupElement.Add(backgroundButton);

    //    //Set content size
    //    popupContent.style.width = new StyleLength(new Length(widthInPercents, LengthUnit.Percent));
    //    popupContent.style.height = new StyleLength(new Length(heightInPercents, LengthUnit.Percent));

    //    //Show popupContent in the middle of the screen
    //    popupContent.style.position = new StyleEnum<Position>(Position.Absolute);

    //    float topAndBottom = (100f - heightInPercents) / 2f;
    //    popupContent.style.top = new StyleLength(new Length(topAndBottom, LengthUnit.Percent));
    //    popupContent.style.bottom = new StyleLength(new Length(topAndBottom, LengthUnit.Percent));

    //    float leftAndRight = (100f - widthInPercents) / 2f;
    //    popupContent.style.left = new StyleLength(new Length(leftAndRight, LengthUnit.Percent));
    //    popupContent.style.right = new StyleLength(new Length(leftAndRight, LengthUnit.Percent));

    //    popupElement.Add(popupContent);

    //    rootElementForPopup.Add(popupElement);
    //}

    //private static void PopupClose(VisualElement popupRoot, VisualElement popup)//,
    //    //Action callbackAfterPopupIsClosed)
    //{
    //    popupRoot.Remove(popup);

    //    //callbackAfterPopupIsClosed?.Invoke();
    //}


}
