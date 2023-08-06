using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames
{
    public class PlayerInputController : MonoBehaviour
    {
        #region Variables

        [Space]
        [SerializeField] private bool legacyInputSystem = false;
        [SerializeField] private LegacyInputs legacyInputs;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField, ReadOnly] private UserInputs info;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Game Loop

        void Update()
        {
            if (legacyInputSystem)
            {
                UpdateUserInputLegacy();
            }
            else
            {
                UpdateUserInput();
            }
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region New Input System

        private void UpdateUserInput()
        {
            info.inputButton0.keydown = playerInput.actions["Input Button 0"].WasPressedThisFrame();
            info.inputButton0.keypressed = playerInput.actions["Input Button 0"].IsPressed();
            info.inputButton0.keyreleased = playerInput.actions["Input Button 0"].WasReleasedThisFrame();

            info.inputButton1.keydown = playerInput.actions["Input Button 1"].WasPressedThisFrame();
            info.inputButton1.keypressed = playerInput.actions["Input Button 1"].IsPressed();
            info.inputButton1.keyreleased = playerInput.actions["Input Button 1"].WasReleasedThisFrame();

            info.inputButton2.keydown = playerInput.actions["Input Button 2"].WasPressedThisFrame();
            info.inputButton2.keypressed = playerInput.actions["Input Button 2"].IsPressed();
            info.inputButton2.keyreleased = playerInput.actions["Input Button 2"].WasReleasedThisFrame();

            info.inputButton3.keydown = playerInput.actions["Input Button 3"].WasPressedThisFrame();
            info.inputButton3.keypressed = playerInput.actions["Input Button 3"].IsPressed();
            info.inputButton3.keyreleased = playerInput.actions["Input Button 3"].WasReleasedThisFrame();

            info.inputButton4.keydown = playerInput.actions["Input Button 4"].WasPressedThisFrame();
            info.inputButton4.keypressed = playerInput.actions["Input Button 4"].IsPressed();
            info.inputButton4.keyreleased = playerInput.actions["Input Button 4"].WasReleasedThisFrame();

            info.inputButton5.keydown = playerInput.actions["Input Button 5"].WasPressedThisFrame();
            info.inputButton5.keypressed = playerInput.actions["Input Button 5"].IsPressed();
            info.inputButton5.keyreleased = playerInput.actions["Input Button 5"].WasReleasedThisFrame();

            info.inputButton6.keydown = playerInput.actions["Input Button 6"].WasPressedThisFrame();
            info.inputButton6.keypressed = playerInput.actions["Input Button 6"].IsPressed();
            info.inputButton6.keyreleased = playerInput.actions["Input Button 6"].WasReleasedThisFrame();

            info.inputButton7.keydown = playerInput.actions["Input Button 7"].WasPressedThisFrame();
            info.inputButton7.keypressed = playerInput.actions["Input Button 7"].IsPressed();
            info.inputButton7.keyreleased = playerInput.actions["Input Button 7"].WasReleasedThisFrame();

            info.inputButton8.keydown = playerInput.actions["Input Button 8"].WasPressedThisFrame();
            info.inputButton8.keypressed = playerInput.actions["Input Button 8"].IsPressed();
            info.inputButton8.keyreleased = playerInput.actions["Input Button 8"].WasReleasedThisFrame();

            info.inputButton9.keydown = playerInput.actions["Input Button 9"].WasPressedThisFrame();
            info.inputButton9.keypressed = playerInput.actions["Input Button 9"].IsPressed();
            info.inputButton9.keyreleased = playerInput.actions["Input Button 9"].WasReleasedThisFrame();

            info.inputAxisX = playerInput.actions["Input Axis X Y"].ReadValue<Vector2>().x;
            info.inputAxisY = playerInput.actions["Input Axis X Y"].ReadValue<Vector2>().y;
            info.inputAxis4 = playerInput.actions["Input Axis 4 5"].ReadValue<Vector2>().x;
            info.inputAxis5 = playerInput.actions["Input Axis 4 5"].ReadValue<Vector2>().y;
            info.inputAxis6 = playerInput.actions["Input Axis 6 7"].ReadValue<Vector2>().x;
            info.inputAxis7 = playerInput.actions["Input Axis 6 7"].ReadValue<Vector2>().y;
            info.inputAxis9 = playerInput.actions["Input Axis 9"].ReadValue<float>();
            info.inputAxis10 = playerInput.actions["Input Axis 10"].ReadValue<float>();
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Legacy Input System

        private void UpdateUserInputLegacy()
        {
            info.inputButton0.keydown = Input.GetKeyDown(legacyInputs.button0);
            info.inputButton0.keypressed = Input.GetKey(legacyInputs.button0);
            info.inputButton0.keyreleased = Input.GetKeyUp(legacyInputs.button0);

            info.inputButton1.keydown = Input.GetKeyDown(legacyInputs.button1);
            info.inputButton1.keypressed = Input.GetKey(legacyInputs.button1);
            info.inputButton1.keyreleased = Input.GetKeyUp(legacyInputs.button1);

            info.inputButton2.keydown = Input.GetKeyDown(legacyInputs.button2);
            info.inputButton2.keypressed = Input.GetKey(legacyInputs.button2);
            info.inputButton2.keyreleased = Input.GetKeyUp(legacyInputs.button2);

            info.inputButton3.keydown = Input.GetKeyDown(legacyInputs.button3);
            info.inputButton3.keypressed = Input.GetKey(legacyInputs.button3);
            info.inputButton3.keyreleased = Input.GetKeyUp(legacyInputs.button3);

            info.inputButton4.keydown = Input.GetKeyDown(legacyInputs.button4);
            info.inputButton4.keypressed = Input.GetKey(legacyInputs.button4);
            info.inputButton4.keyreleased = Input.GetKeyUp(legacyInputs.button4);

            info.inputButton5.keydown = Input.GetKeyDown(legacyInputs.button5);
            info.inputButton5.keypressed = Input.GetKey(legacyInputs.button5);
            info.inputButton5.keyreleased = Input.GetKeyUp(legacyInputs.button5);

            info.inputButton6.keydown = Input.GetKeyDown(legacyInputs.button6);
            info.inputButton6.keypressed = Input.GetKey(legacyInputs.button6);
            info.inputButton6.keyreleased = Input.GetKeyUp(legacyInputs.button6);

            info.inputButton7.keydown = Input.GetKeyDown(legacyInputs.button7);
            info.inputButton7.keypressed = Input.GetKey(legacyInputs.button7);
            info.inputButton7.keyreleased = Input.GetKeyUp(legacyInputs.button7);

            info.inputButton8.keydown = Input.GetKeyDown(legacyInputs.button8);
            info.inputButton8.keypressed = Input.GetKey(legacyInputs.button8);
            info.inputButton8.keyreleased = Input.GetKeyUp(legacyInputs.button8);

            info.inputButton9.keydown = Input.GetKeyDown(legacyInputs.button9);
            info.inputButton9.keypressed = Input.GetKey(legacyInputs.button9);
            info.inputButton9.keyreleased = Input.GetKeyUp(legacyInputs.button9);

            info.inputAxisX = GetAxisValue(legacyInputs.axisXY_left, legacyInputs.axisXY_right);
            info.inputAxisY = GetAxisValue(legacyInputs.axisXY_up, legacyInputs.axisXY_down);
            info.inputAxis4 = GetAxisValue(legacyInputs.axis45_left, legacyInputs.axis45_right);
            info.inputAxis5 = GetAxisValue(legacyInputs.axis45_up, legacyInputs.axis45_down);
            info.inputAxis6 = GetAxisValue(legacyInputs.axis67_left, legacyInputs.axis67_right);
            info.inputAxis7 = GetAxisValue(legacyInputs.axis67_up, legacyInputs.axis67_down);
            info.inputAxis9 = GetAxisValue(legacyInputs.axis9);
            info.inputAxis10 = GetAxisValue(legacyInputs.axis10);
        }

        private float GetAxisValue(KeyCode button01)
        {
            if (Input.GetKey(button01))
            {
                return 1.0f;
            }
            else
            {
                return 0.0f;
            }
        }

        private float GetAxisValue(KeyCode button01, KeyCode button02)
        {
            if (Input.GetKey(button01) && !Input.GetKey(button02))
            {
                return -1.0f;
            }
            else if (!Input.GetKey(button01) && Input.GetKey(button02))
            {
                return 1.0f;
            }
            else
            {
                return 0.0f;
            }
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Public Functions

        public UserInputs GetUserInputs()
        {
            return info;
        }

        #endregion

    } // class end
}