using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Clam
{
    public class PauseMenu : MonoBehaviour
    {
        Button m_Resume;
        Button m_Export;
        Button m_MainMenu;

        UIDocument m_UIDocument;

        // Start is called before the first frame update
        void Start()
        {
            m_UIDocument = GetComponent<UIDocument>();
            m_Resume = m_UIDocument.rootVisualElement.Q<Button>("Resume");
            m_Export = m_UIDocument.rootVisualElement.Q<Button>("Export");
            m_MainMenu = m_UIDocument.rootVisualElement.Q<Button>("MainMenu");

            var background = m_UIDocument.rootVisualElement.Q<VisualElement>("PauseMainBackground");

            background.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0.71f));

            m_MainMenu.RegisterCallback<ClickEvent>(MainMenuCallback);

            BlurFocus();
            //UnityEngine.Cursor.visible = true;
        }

        void MainMenuCallback(ClickEvent evt)
        {
            var template = Resources.Load<VisualTreeAsset>("ui/AreYouSure");
            var instance = template.Instantiate();

            UIHelpers.ShowPopup(m_UIDocument.rootVisualElement, instance);

            var overlay = m_UIDocument.rootVisualElement.Q<VisualElement>("Overlay");
            overlay.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0.71f));
            var yesButton = m_UIDocument.rootVisualElement.Q<Button>("PopUpYesButton");
            var noButton = m_UIDocument.rootVisualElement.Q<Button>("PopUpNoButton");

            noButton.clickable.clicked += () =>
            {
                UIHelpers.PopupClose(m_UIDocument.rootVisualElement, m_UIDocument.rootVisualElement.Q<VisualElement>("PopUpElement"));
            };

            yesButton.clickable.clicked += () =>
            {
                //Application.Quit();
                Debug.Log("quit?");
                MenuEventManager.SwitchState(Menu.Main);
            };
        }

        private void SetCreateButtonCallback()
        {
            m_Resume.RegisterCallback<ClickEvent>(CreateButtonCallback);
        }

        private void CreateButtonCallback(ClickEvent evt)
        {
            Button button = evt.currentTarget as Button;

            MenuEventManager.SwitchState(Menu.ResumePlay);

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void BlurFocus()
        {
            var focusedElement = GetFocusedElement();
            if (focusedElement != null)
            {

                //focusedElement.focusable = false;
                focusedElement.Blur();
            }
        }

        public static Focusable GetFocusedElement()
        {
            EventSystem eventSystem = EventSystem.current;
            if (eventSystem == null)
            {
                return null;
            }

            GameObject selectedGameObject = eventSystem.currentSelectedGameObject;
            if (selectedGameObject == null)
            {
                return null;
            }

            PanelEventHandler panelEventHandler = selectedGameObject.GetComponent<PanelEventHandler>();
            if (panelEventHandler != null)
            {
                return panelEventHandler.panel.focusController.focusedElement;
            }

            return null;
        }
    }
}