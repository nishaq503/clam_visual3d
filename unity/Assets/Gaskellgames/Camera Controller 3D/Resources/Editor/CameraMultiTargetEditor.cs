#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

using Gaskellgames;
using System.Collections.Generic;
using Unity.VisualScripting;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames.CameraController3D
{
    [CustomEditor(typeof(CameraMultiTarget))] [CanEditMultipleObjects]
    public class CameraMultiTargetEditor : Editor
    {
        #region Serialized Properties / OnEnable

        SerializedProperty showTracked;
        SerializedProperty refCamLookAt;
        SerializedProperty speed;
        SerializedProperty targetObjects;

        private void OnEnable()
        {
            showTracked = serializedObject.FindProperty("showTracked");
            refCamLookAt = serializedObject.FindProperty("refCamLookAt");
            speed = serializedObject.FindProperty("speed");
            targetObjects = serializedObject.FindProperty("targetObjects");
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnInspectorGUI

        public override void OnInspectorGUI()
        {
            // get & update references
            CameraMultiTarget cameraMultiTarget = (CameraMultiTarget)target;
            serializedObject.Update();

            /*
            // draw default inspector
            base.OnInspectorGUI();
            */

            // banner
            Texture banner = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Gaskellgames/Camera Controller 3D/Resources/Icons/inspectorBanner_CameraController3D.png", typeof(Texture));
            GUILayout.Box(banner, GUILayout.ExpandWidth(true), GUILayout.Height(Screen.width / 7.5f));

            // custom inspector
            EditorGUILayout.PropertyField(showTracked);
            EditorGUILayout.PropertyField(refCamLookAt);
            EditorGUILayout.PropertyField(speed);
            EditorGUILayout.Space();

            GUI.contentColor = Color.gray;
            EditorGUILayout.PropertyField(targetObjects);

            // apply reference changes
            serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}

#endif
