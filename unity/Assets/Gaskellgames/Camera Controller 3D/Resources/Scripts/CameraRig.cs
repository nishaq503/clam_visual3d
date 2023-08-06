using System.Collections;
using UnityEngine;

using Gaskellgames;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames.CameraController3D
{
    public class CameraRig : MonoBehaviour
    {
        #region Variables

        [Tooltip("If the freelookRig is assigned, then the cameraRig will ignore follow, lookAt, turnSpeed & followOffset")]
        [SerializeField] private CameraFreelookRig freelookRig;
        [Tooltip("Registered cameraRigs are added to the global camera list when enabled and removed when disabled")]
        [SerializeField] private bool registerCamera;
        [Space]
        [SerializeField] private Transform follow;
        [SerializeField] private Transform lookAt;
        [SerializeField, Range(0, 1)] private float turnSpeed = 1.0f;
        [SerializeField] private Vector3 followOffset;
        [SerializeField] private CameraLens lens;
        private float lerpSpeedMultiplier = 40f;
        private float desiredCameraTilt;
        private Quaternion rotationTarget;
        private Vector3 direction;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Game Loop

        private void OnEnable()
        {
            if(registerCamera)
            {
                CameraList.Register(GetComponent<CameraRig>());
            }
        }

        private void OnDisable()
        {
            CameraList.Unregister(GetComponent<CameraRig>());
        }

        private void Update()
        {
            UpdateCamera();
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Editor / Debug

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            UpdateCamera();
        }

        private void OnDrawGizmosSelected()
        {
            // draw camera frustrum
            if (lens.showFrustrum)
            {
                Matrix4x4 resetMatrix = Gizmos.matrix;
                Gizmos.matrix = gameObject.transform.localToWorldMatrix;
                Gizmos.color = Color.grey;

                float width = Screen.width * 1.000f;
                float height = Screen.height * 1.000f;
                float aspect = width / height;

                Gizmos.DrawFrustum(Vector3.zero, lens.verticalFOV, lens.farClipPlane, lens.nearClipPlane, aspect);
                Gizmos.matrix = resetMatrix;
            }
        }

#endif

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Private Functions

        private void UpdateCamera()
        {
            // update position
            if (freelookRig != null)
            {
                transform.position = freelookRig.GetFreelookCameraPosition();
            }
            else if (follow != null)
            {
                transform.position = follow.position + followOffset;
            }

            // update rotation
            bool updateRotation = false;
            if (freelookRig != null)
            {
                direction = (freelookRig.transform.position - transform.position).normalized;
                updateRotation = true;
            }
            else if (lookAt != null)
            {
                direction = (lookAt.position - transform.position).normalized;
                updateRotation = true;
            }
            if (updateRotation)
            {
                rotationTarget = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotationTarget, turnSpeed);
            }

            // update tilt
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, lens.tilt);
        }

        private IEnumerator SmoothlyLerpCameraTilt()
        {
            // smoothly lerp moveSpeed to moveState speed
            float time = 0;
            float difference = Mathf.Abs(desiredCameraTilt - lens.tilt);
            float startValue = lens.tilt;

            while (time < difference)
            {
                lens.tilt = Mathf.Lerp(startValue, desiredCameraTilt, time / difference);
                time += Time.deltaTime * lerpSpeedMultiplier;
                yield return null;
            }

            lens.tilt = desiredCameraTilt;
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Getters / Setters

        public void SetCameraTilt(float newCameraTilt)
        {
            StopAllCoroutines();
            desiredCameraTilt = newCameraTilt;
            StartCoroutine(SmoothlyLerpCameraTilt());
        }

        public CameraFreelookRig GetFreelookRig() { return freelookRig; }

        public void SetFreelookRig(CameraFreelookRig newFreelookRig) { freelookRig = newFreelookRig; }

        public Transform GetCameraFollow() { return follow; }

        public void SetCameraFollow(Transform newFollow) { follow = newFollow; }

        public Transform GetCameraLookAt() { return lookAt; }

        public void SetCameraLookAt(Transform newLookAt) { lookAt = newLookAt; }

        public CameraLens GetCameraLens() { return lens; }

        public void SetCameraLens(CameraLens newLens) { lens = newLens; }

        #endregion

    } //class end
}
