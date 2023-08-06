using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

using Gaskellgames;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames.CameraController3D
{
    public class CameraFreelookRig : MonoBehaviour
    {
        #region Variables

        [SerializeField] private bool customInputAction;
        [SerializeField] private InputActionProperty moveCamera;
        [SerializeField] private PlayerInputController playerInputController;
        private UserInputs userInputs;

        [SerializeField] private bool cameraCollisions;
        [SerializeField] private bool gizmosOnSelected;
        [SerializeField] private bool lockCursorOnLoad;
        
        [SerializeField] private Transform follow;
        [SerializeField] private CameraOrbits topRig;
        [SerializeField] private CameraOrbits middleRig;
        [SerializeField] private CameraOrbits bottomRig;
        private Vector3 freelookCameraTargetPosition;
        private Vector3 freelookCameraPosition;
        private bool collisionDetected;

        [SerializeField, Range(0, 100)] private int xSensitivity = 80;
        [SerializeField, Range(0, 100)] private int ySensitivity = 80;
        [SerializeField, Range(-180, 180)] private float rotationOffset = 0f;
        [SerializeField, Range(0, 1)] private float collisionOffset = 0.2f;
        private float xRotation;
        private float yRotation;
        private float xClampUp;
        private float xClampDown;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Game Loop

        private void OnEnable()
        {
            moveCamera.action.Enable();
        }

        private void OnDisable()
        {
            moveCamera.action.Disable();
        }

        private void Start()
        {
            // lock cursor to screen
            if (lockCursorOnLoad)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void Update()
        {
            GetUserInputs();
            GetViewInput();
            UpdateFollowPosition();

            if (follow != null)
            {
                transform.position = follow.position;
            }
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Debug [Editor]

#if UNITY_EDITOR

        private void Reset()
        {
            StartCoroutine(PauseForSeconds(0.1f));
        }

        private IEnumerator PauseForSeconds(float value)
        {
            yield return new WaitForSeconds(value);

            topRig.height = 1;
            topRig.radius = 1;
            middleRig.height = 0;
            middleRig.radius = 2;
            bottomRig.height = -1;
            bottomRig.radius = 1;

            CameraRig newCameraRig = GetComponentInChildren<CameraRig>();
            GameObject newFreelookRig;

            if (newCameraRig == null)
            {
                newFreelookRig = new GameObject("CameraRig (Freelook)");
                newFreelookRig.transform.parent = gameObject.transform;
                newFreelookRig.transform.localPosition = Vector3.zero;
                CameraRig cameraRig = newFreelookRig.AddComponent<CameraRig>();
                cameraRig.SetFreelookRig(this);
            }
            else
            {
                newCameraRig.SetFreelookRig(this);
            }
        }

        private void OnDrawGizmos()
        {
            UpdateFollowPosition();

            if (follow != null)
            {
                transform.position = follow.position;
            }

            if (!gizmosOnSelected)
            {
                DrawCameraGizmos();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if(gizmosOnSelected)
            {
                DrawCameraGizmos();
            }
        }

        private void DrawCameraGizmos()
        {
            // set gizmo matrix to local transform
            Matrix4x4 resetMatrix = Gizmos.matrix;
            Gizmos.matrix = gameObject.transform.localToWorldMatrix;

            // draw camera orbits
            UnityEditor.Handles.color = Color.cyan;
            UnityEditor.Handles.DrawWireDisc(new Vector3(transform.position.x, transform.position.y + topRig.height, transform.position.z), transform.up, topRig.radius);
            UnityEditor.Handles.DrawWireDisc(new Vector3(transform.position.x, transform.position.y + middleRig.height, transform.position.z), transform.up, middleRig.radius);
            UnityEditor.Handles.DrawWireDisc(new Vector3(transform.position.x, transform.position.y + bottomRig.height, transform.position.z), transform.up, bottomRig.radius);

            // draw freelook current orbit
            if(collisionDetected) { UnityEditor.Handles.color = Color.red; }
            else { UnityEditor.Handles.color = Color.yellow; }
            float currentRadius = Vector3.Magnitude(new Vector2(freelookCameraTargetPosition.x, freelookCameraTargetPosition.z));
            UnityEditor.Handles.DrawWireDisc(new Vector3(transform.position.x, transform.position.y + freelookCameraTargetPosition.y, transform.position.z), transform.up, currentRadius);

            // draw line between orbits
            Gizmos.color = Color.cyan;
            Vector3 top = Quaternion.AngleAxis(-rotationOffset, Vector3.up) * new Vector3(topRig.radius, topRig.height, 0);
            Vector3 middle = Quaternion.AngleAxis(-rotationOffset, Vector3.up) * new Vector3(middleRig.radius, middleRig.height, 0);
            Vector3 bottom = Quaternion.AngleAxis(-rotationOffset, Vector3.up) * new Vector3(bottomRig.radius, bottomRig.height, 0);
            Gizmos.DrawLine(top, middle);
            Gizmos.DrawLine(middle, bottom);

            // draw line for rotation directions
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(Vector3.zero, freelookCameraPosition);

            if (collisionDetected) { UnityEditor.Handles.color = Color.red; }
            else { UnityEditor.Handles.color = Color.yellow; }
            Vector3 lineOut = new Vector3(freelookCameraTargetPosition.x, 0, freelookCameraTargetPosition.z);
            Gizmos.DrawLine(Vector3.zero, lineOut);
            Gizmos.DrawLine(lineOut, freelookCameraTargetPosition);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(freelookCameraPosition, freelookCameraTargetPosition);

            // reset gizmo matrix to public position
            Gizmos.matrix = resetMatrix;
        }

#endif

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Private Functions

        private void GetUserInputs()
        {
            if (playerInputController != null)
            {
                userInputs = playerInputController.GetUserInputs();
            }
        }

        private void UpdateFollowPosition()
        {
            UpdateTargetPosition();
            UpdateCameraCollisions();
        }

        private void UpdateCameraCollisions()
        {
            if(cameraCollisions)
            {
                float maxDistance = freelookCameraTargetPosition.magnitude;
                Vector3 direction = freelookCameraTargetPosition.normalized;

                RaycastHit hit;
                Ray ray = new Ray(transform.position, direction);

                if (Physics.Raycast(ray, out hit))
                {
                    if(hit.distance < maxDistance)
                    {
                        freelookCameraPosition = direction * (hit.distance - collisionOffset);
                        collisionDetected = true;
                    }
                    else
                    {
                        freelookCameraPosition = freelookCameraTargetPosition;
                        collisionDetected = false;
                    }
                }
                else
                {
                    freelookCameraPosition = freelookCameraTargetPosition;
                    collisionDetected = false;
                }
            }
            else
            {
                freelookCameraPosition = freelookCameraTargetPosition;
                collisionDetected = false;
            }
        }

        private void UpdateTargetPosition()
        {
            xClampUp = 90.0f - GetAngleOnRightAngleTriangle(Mathf.Abs(topRig.radius), Mathf.Abs(topRig.height));
            xClampDown = 90.0f - GetAngleOnRightAngleTriangle(Mathf.Abs(bottomRig.radius), Mathf.Abs(bottomRig.height));

            float distance = 0.00f;
            if(0 <= xRotation)
            {
                float ratio = Mathf.Abs(xRotation) / xClampUp;
                distance = (middleRig.radius * (1.0f - ratio)) + (Vector3.Magnitude(new Vector2(topRig.height, topRig.radius)) * ratio);
            }
            else
            {
                float ratio = Mathf.Abs(xRotation) / xClampDown;
                distance = (middleRig.radius * (1.0f - ratio)) + (Vector3.Magnitude(new Vector2(bottomRig.height, bottomRig.radius)) * ratio);
            }

            float longitude = (rotationOffset - yRotation) * Mathf.Deg2Rad;
            float latitude = xRotation * Mathf.Deg2Rad;
            float equatorX = Mathf.Cos(longitude);
            float equatorZ = Mathf.Sin(longitude);
            float multiplier = Mathf.Cos(latitude);
            float x = multiplier * equatorX;
            float y = Mathf.Sin(latitude);
            float z = multiplier * equatorZ;

            freelookCameraTargetPosition = new Vector3(x, y, z) * distance;
        }

        private float GetAngleOnRightAngleTriangle(float Opposite, float Adjacent)
        {
            float angle = Mathf.Atan2(Opposite, Adjacent);
            angle = angle * Mathf.Rad2Deg;

            return angle;
        }

        private void GetViewInput()
        {
            // get mouse input
            float mouseX;
            float mouseY;

            if (customInputAction)
            {
                mouseX = moveCamera.action.ReadValue<Vector2>().x * Time.deltaTime * xSensitivity * 2;
                mouseY = moveCamera.action.ReadValue<Vector2>().y * Time.deltaTime * ySensitivity * 2;
            }
            else
            {
                mouseX = userInputs.inputAxis4 * Time.deltaTime * xSensitivity * 2;
                mouseY = userInputs.inputAxis5 * Time.deltaTime * ySensitivity * 2;
            }

            // set rotation & maximum up/down rotation
            yRotation += mouseX;
            xRotation -= mouseY;

            xRotation = Mathf.Clamp(xRotation, -xClampDown, xClampUp);
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Getters / Setters

        public Vector3 GetFreelookCameraPosition() { return transform.position + freelookCameraPosition; }

        public Transform GetFreelookRigFollow() { return follow; }

        public void SetFreelookRigFollow(Transform newFollow) { follow = newFollow; }

        public CameraOrbits GetFreelookRigOrbitTop() { return topRig; }

        public void SetFreelookRigOrbitTop(float newHeight, float newRadius) { topRig.height = newHeight; topRig.radius = newRadius; }

        public CameraOrbits GetFreelookRigOrbitMiddle() { return middleRig; }

        public void SetFreelookRigOrbitMiddle(float newHeight, float newRadius) { middleRig.height = newHeight; middleRig.radius = newRadius; }

        public CameraOrbits GetFreelookRigOrbitBottom() { return bottomRig; }

        public void SetFreelookRigOrbitBottom(float newHeight, float newRadius) { bottomRig.height = newHeight; bottomRig.radius = newRadius; }

        public void SetSensitivity(Vector2 newSensitivity) { xSensitivity = (int)newSensitivity.x; ySensitivity = (int)newSensitivity.y; }

        #endregion

    } //class end
}
