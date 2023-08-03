using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 verticalMove;
        public Vector2 look;
		public bool jump;
		public bool sprint;



		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM

        //public void OnChangeMap(InputValue value)
        //{
        //    Debug.Log("change map!");
        //    bool uiActive = Cursor.visible;

        //    if (uiActive)
        //    {
        //        //_playerInput.SwitchCurrentActionMap("Player");
        //        Cursor.lockState = CursorLockMode.Locked;

        //    }
        //    else
        //    {
        //        //_playerInput.SwitchCurrentActionMap("WorldUI");
        //        Cursor.lockState = CursorLockMode.None;

        //    }
        //    Cursor.visible = !Cursor.visible;

        //}
        public void OnMove(InputValue value)
		{
			Debug.Log("enable on move");
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnUpDown(InputValue value)
		{
			Debug.Log("jump");

			VerticalInput(value.Get<Vector2>());
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
            Debug.Log("after endif on move");

            move = newMoveDirection;
		}

        public void VerticalInput(Vector2 newMoveDirection)
        {
            Debug.Log("after endif on move");

            verticalMove = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}
		
		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);

			Debug.Log("application focused");
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}