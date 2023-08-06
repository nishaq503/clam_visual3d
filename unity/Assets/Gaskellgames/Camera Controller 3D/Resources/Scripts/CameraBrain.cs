using System;
using UnityEngine;

using Gaskellgames;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames.CameraController3D
{
    public class CameraBrain : MonoBehaviour
    {
        #region Variables

        [HideInInspector]
        private enum cameraBlendStyle
        {
            Cut,
            FadeToColor
        }

        [SerializeField] private CameraRig activeCamera;
        [ReadOnly, SerializeField] private CameraRig previousCamera;

        [SerializeField] private cameraBlendStyle blendingStyle = cameraBlendStyle.Cut;
        [SerializeField] private AnimationCurve fadeCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.5f, 0.5f, -1.5f, -1.5f), new Keyframe(1, 0));
        [SerializeField] private Color fadeColor = Color.black;
        [SerializeField, Range(0.01f, 2.0f)] private float fadeSpeed = 1.0f;
        [SerializeField] private bool fadeFullScreen = true;
        [SerializeField] private CanvasGroup canvasGroup;
        private bool triggerFade = false;
        private Texture2D texture;
        private float alpha = 0f;
        private float timer = 0f;
        private int fadeDirection = 0;

        [ReadOnly, SerializeField] private Transform follow;
        [ReadOnly, SerializeField] private Transform lookAt;
        [ReadOnly, SerializeField] private CameraLens lens;
        [ReadOnly, SerializeField] private CameraOrbits topRig;
        [ReadOnly, SerializeField] private CameraOrbits middleRig;
        [ReadOnly, SerializeField] private CameraOrbits bottomRig;
        private CameraRig activeCameraCheck;
        private Camera cam;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Game Loop

        private void Reset()
        {
            if (gameObject.GetComponent<Camera>() != null)
            {
                cam = GetComponent<Camera>();
                activeCameraCheck = null;
            }
        }

        private void Start()
        {
            InitializeVariables();
        }

        private void LateUpdate()
        {
            UpdateCamera();

            if (triggerFade)
            {
                triggerFade = false;
                UpdateFade();
            }
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region On Events

        public void OnGUI()
        {
            if(blendingStyle == cameraBlendStyle.FadeToColor)
            {
                // draw texture to screen
                if (alpha > 0f && fadeFullScreen)
                {
                    GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);
                }

                if (fadeDirection != 0)
                {
                    // set alpha based on timer value
                    timer += fadeDirection * Time.deltaTime * fadeSpeed;
                    alpha = fadeCurve.Evaluate(timer);

                    // apply alpha to screen texture
                    if (!fadeFullScreen && canvasGroup != null)
                    {
                        canvasGroup.alpha = 1 - alpha;
                    }
                    texture.SetPixel(0, 0, new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha));
                    texture.Apply();

                    // clamp alpha
                    if (alpha <= 0f || alpha >= 1f)
                    {
                        fadeDirection = 0;
                    }
                }
            }
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Editor / Debug

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying && activeCamera != null)
            {
                transform.position = activeCamera.transform.position;
                transform.rotation = activeCamera.transform.rotation;

                activeCameraCheck = activeCamera;
            }
        }

        public string GetBlendingStyle() { return blendingStyle.ToString(); }

#endif

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Functions

        private void InitializeVariables()
        {
            cam = GetComponent<Camera>();

            alpha = 0f;
            texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha));
            texture.Apply();

            if (activeCamera != null)
            {
                previousCamera = activeCamera;
            }
        }

        private void UpdateCamera()
        {
            if (activeCamera != null)
            {
                if (activeCameraCheck != activeCamera)
                {
                    if (cam == null)
                    {
                        cam = GetComponent<Camera>();
                    }
                    else
                    {
                        if (blendingStyle == cameraBlendStyle.Cut)
                        {
                            UpdateCameraSettings();
                        }
                        else if (blendingStyle == cameraBlendStyle.FadeToColor)
                        {
                            // fade to 'black'
                            if (CheckScreenClear() && previousCamera != null)
                            {
                                triggerFade = true;
                            }

                            if (CheckScreenFaded())
                            {
                                UpdateCameraSettings();

                                // unfade from 'black'
                                triggerFade = true;
                            }
                        }   
                    }
                }
                else
                {
                    transform.position = activeCamera.transform.position;
                    transform.rotation = activeCamera.transform.rotation;
                }
            }
        }

        private void UpdateCameraSettings()
        {
            CameraLens tempLens = activeCamera.GetComponent<CameraRig>().GetCameraLens();
            cam.fieldOfView = tempLens.verticalFOV;
            cam.nearClipPlane = tempLens.nearClipPlane;
            cam.farClipPlane = tempLens.farClipPlane;
            cam.cullingMask = tempLens.cullingMask;

            activeCameraCheck = activeCamera;
        }

        private void UpdateFade()
        {
            if (fadeDirection == 0)
            {
                if (alpha >= 1f) // Fully faded out
                {
                    alpha = 1f;
                    timer = 0f;
                    fadeDirection = 1;
                }
                else // Fully faded in
                {
                    alpha = 0f;
                    timer = 1f;
                    fadeDirection = -1;
                }
            }
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Getters / Setters

        public CameraRig GetActiveCamera() { return activeCamera; }

        public void SetActiveCamera(CameraRig newActiveCamera)
        {
            if (activeCamera != newActiveCamera)
            {
                previousCamera = activeCamera;
            }
            activeCamera = newActiveCamera;
        }

        public CameraRig GetPreviousCamera() { return previousCamera; }

        public void SetPreviousCamera() { activeCamera = previousCamera; }

        public bool CheckScreenFaded() { return alpha >= 1; }

        public bool CheckScreenClear() { return alpha <= 0; }

        public void ActivateFade() { triggerFade = true; }

        #endregion

    } // class end
}
