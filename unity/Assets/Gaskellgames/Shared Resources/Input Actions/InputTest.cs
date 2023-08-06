using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames
{
    public class InputTest : MonoBehaviour
    {
        #region Variables

        [SerializeField] private InputActionProperty userInput;
        [SerializeField, ReadOnly] private bool buttonHeld;
        [Space][SerializeField] private InspectorEvent OnPressed;
        [Space][SerializeField] private InspectorEvent OnHeld;
        [Space][SerializeField] private InspectorEvent OnReleased;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Game loop

        private void OnEnable()
        {
            userInput.action.performed += userInputCallbackPerformed;
            userInput.action.canceled += userInputCallbackCanceled;
            userInput.action.Enable();
        }

        private void OnDisable()
        {
            userInput.action.performed -= userInputCallbackPerformed;
            userInput.action.canceled -= userInputCallbackCanceled;
            userInput.action.Disable();
        }

        private void Update()
        {
            if (buttonHeld)
            {
                OnHeld.Invoke();
            }
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Input Action Callbacks

        private void userInputCallbackPerformed(InputAction.CallbackContext context)
        {
            OnPressed.Invoke();
            buttonHeld = true;
        }

        private void userInputCallbackCanceled(InputAction.CallbackContext context)
        {
            OnReleased.Invoke();
            buttonHeld = false;
        }

        #endregion

    } // class end
}
