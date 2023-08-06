using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames.CameraController3D
{
    public class CameraMultiTarget : MonoBehaviour
    {
        #region Variables

        //[Header("Camera")] [Space]

        [SerializeField] private bool showTracked = true;
        [SerializeField] private Transform refCamLookAt;
        [SerializeField, Range(0, 10)] private float speed = 2.0f;
        [ReadOnly, SerializeField] private List<Transform> targetObjects;
        private Vector3 defaultPosition;
        private Vector3 averagePosition;
        private Vector3 targetPosition;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Game Loop

        private void Reset()
        {
            InitialiseSettings();
        }

        private void Start()
        {
            defaultPosition = refCamLookAt.position;
        }

        private void Update()
        {
            UpdateRefCamLookAt();
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

#if UNITY_EDITOR

        #region Editor Gizmos

        private void OnDrawGizmos()
        {
            if (showTracked)
            {
                UpdateGizmos();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!showTracked)
            {
                UpdateGizmos();
            }
        }

        #endregion

#endif

        //----------------------------------------------------------------------------------------------------

        #region Private Functions

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

        private void UpdateGizmos()
        {
            for (int i = 0; i < targetObjects.Count; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(refCamLookAt.position, targetObjects[i].position);
            }
        }

        private void UpdateRefCamLookAt()
        {
            if (0 < targetObjects.Count)
            {
                // calculate center point of the targets
                float averageX = 0.0f;
                float averageY = 0.0f;
                float averageZ = 0.0f;

                for(int i = 0; i < targetObjects.Count; i++)
                {
                    averageX += targetObjects[i].position.x;
                    averageY += targetObjects[i].position.y;
                    averageZ += targetObjects[i].position.z;
                }
                averagePosition = new Vector3(averageX, averageY, averageZ) / targetObjects.Count;
                targetPosition = averagePosition;
            }
            else
            {
                // return to default position
                targetPosition = defaultPosition;
            }

            // move refCamLookAt to target position
            if (refCamLookAt.position != targetPosition + new Vector3(0.01f, 0.01f, 0.01f))
            {
                // lerp refCamLookAt position
                var step = speed * Time.deltaTime; // calculate distance to move
                refCamLookAt.position = Vector3.MoveTowards(refCamLookAt.position, targetPosition, step);
            }
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Private Functions

        public void AddTargetToList(Transform newTarget)
        {
            if(!targetObjects.Contains(newTarget))
            {
                targetObjects.Add(newTarget);
            }
        }

        public void RemoveTargetFromList(Transform oldTarget)
        {
            if (targetObjects.Contains(oldTarget))
            {
                targetObjects.Remove(oldTarget);
            }
        }

        #endregion

    } //class end
}
