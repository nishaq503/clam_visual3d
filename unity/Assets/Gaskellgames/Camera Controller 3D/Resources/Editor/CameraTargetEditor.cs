#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

using Gaskellgames;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames.CameraController3D
{
    [CustomEditor(typeof(CameraTarget))] [CanEditMultipleObjects]
    public class CameraTargetEditor : Editor
    {
        #region Serialized Properties / OnEnable

        SerializedProperty cameraBrain;
        SerializedProperty revertOnExit;
        SerializedProperty OnEnterCameraSensor;
        SerializedProperty OnExitCameraSensor;

        private void OnEnable()
        {
            cameraBrain = serializedObject.FindProperty("cameraBrain");
            revertOnExit = serializedObject.FindProperty("revertOnExit");
            OnEnterCameraSensor = serializedObject.FindProperty("OnEnterCameraSensor");
            OnExitCameraSensor = serializedObject.FindProperty("OnExitCameraSensor");
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnInspectorGUI

        public override void OnInspectorGUI()
        {
            // get & update references
            CameraTarget cameraTarget = (CameraTarget)target;
            serializedObject.Update();

            /*
            // draw default inspector
            base.OnInspectorGUI();
            */

            // banner
            Texture banner = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Gaskellgames/Camera Controller 3D/Resources/Icons/inspectorBanner_CameraController3D.png", typeof(Texture));
            GUILayout.Box(banner, GUILayout.ExpandWidth(true), GUILayout.Height(Screen.width / 7.5f));

            // custom inspector
            EditorGUILayout.PropertyField(cameraBrain);
            EditorGUILayout.PropertyField(revertOnExit);
            EditorGUILayout.PropertyField(OnEnterCameraSensor);
            EditorGUILayout.PropertyField(OnExitCameraSensor);

            // apply reference changes
            serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}

#endif
