using UnityEngine;
using Gaskellgames;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames.CameraController3D
{
    public class CameraTarget : MonoBehaviour
    {
        #region Variables

        [Tooltip("Add a cameraBrain to set the active camera to the 'CameraSensor' cameraRig during OnTriggerEnter")]
        [Space] [SerializeField] private CameraBrain cameraBrain;
        [SerializeField] private bool revertOnExit;
        private CameraRig previousCamera;

        [Space] [SerializeField] private InspectorEvent OnEnterCameraSensor;
        [Space] [SerializeField] private InspectorEvent OnExitCameraSensor;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Events

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("CameraSensor"))
            {
                if(cameraBrain != null)
                {
                    CameraTriggerSensor cameraTriggerSensor = other.GetComponent<CameraTriggerSensor>();
                    if (cameraTriggerSensor != null)
                    {
                        previousCamera = cameraBrain.GetActiveCamera();
                        cameraBrain.SetActiveCamera(cameraTriggerSensor.GetCameraRig());
                    }
                }

                CameraMultiTarget cameraMultiTarget = other.GetComponent<CameraMultiTarget>();
                if(cameraMultiTarget != null)
                {
                    cameraMultiTarget.AddTargetToList(this.transform);
                }

                OnEnterCameraSensor.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("CameraSensor"))
            {
                if(revertOnExit)
                {
                    cameraBrain.SetActiveCamera(previousCamera);
                }

                CameraMultiTarget cameraMultiTarget = other.GetComponent<CameraMultiTarget>();
                if (cameraMultiTarget != null)
                {
                    cameraMultiTarget.RemoveTargetFromList(this.transform);
                }

                OnExitCameraSensor.Invoke();
            }
        }

        #endregion

    } //class end
}
