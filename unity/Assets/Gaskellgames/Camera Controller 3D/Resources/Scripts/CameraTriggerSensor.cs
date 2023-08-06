using UnityEngine;

using Gaskellgames;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames.CameraController3D
{
    public class CameraTriggerSensor : MonoBehaviour
    {
        #region Variables

        //[Header("Camera")] [Space]

        [SerializeField] private CameraRig cameraRig;

        //[Header("Sensor Info")] [Space]

        [SerializeField] private bool alwaysShowZone = true;
        [SerializeField] private Color32 sensorColour = new Color32(050, 179, 050, 079);
        [SerializeField] private Color32 sensorOutlineColour = new Color32(050, 179, 050, 128);
        private Rigidbody rb;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Game Loop

        private void Reset()
        {
            InitialiseSettings();
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

#if UNITY_EDITOR

        #region Editor Gizmos

        private void OnDrawGizmos()
        {
            if (alwaysShowZone)
            {
                DrawZone();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!alwaysShowZone)
            {
                DrawZone();
            }
        }

        #endregion

#endif

        //----------------------------------------------------------------------------------------------------

        #region Triggers

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                cameraRig.GetComponent<CameraRig>();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                cameraRig.GetComponent<CameraRig>();
            }
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Functions

        private void InitialiseSettings()
        {
            SetupCollider();
        }

        private void SetupCollider()
        {
            if (gameObject.GetComponent<Collider>() == null)
            {
                gameObject.AddComponent<BoxCollider>();
            }
            gameObject.GetComponent<Collider>().isTrigger = true;
        }

        private void DrawZone()
        {
            Matrix4x4 resetMatrix = Gizmos.matrix;
            Gizmos.matrix = gameObject.transform.localToWorldMatrix;

            Gizmos.color = sensorColour;
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
            Gizmos.color = sensorOutlineColour;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

            Gizmos.matrix = resetMatrix;
        }

        public CameraRig GetCameraRig()
        {
            return cameraRig;
        }
        
        #endregion

    } //class end
}
