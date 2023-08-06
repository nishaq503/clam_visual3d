#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

using Gaskellgames;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames.CameraController3D
{
    [CustomEditor(typeof(CameraSwitcher))] [CanEditMultipleObjects]
    public class CameraSwitcherEditor : Editor
    {
        #region Serialized Properties / OnEnable

        SerializedProperty cameraBrain;
        SerializedProperty useRegisteredList;
        SerializedProperty switchCamera;
        SerializedProperty customCameraRigsList;

        bool CustomGroup = false;

        private void OnEnable()
        {
            cameraBrain = serializedObject.FindProperty("cameraBrain");
            useRegisteredList = serializedObject.FindProperty("useRegisteredList");
            switchCamera = serializedObject.FindProperty("switchCamera");
            customCameraRigsList = serializedObject.FindProperty("customCameraRigsList");
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnInspectorGUI

        public override void OnInspectorGUI()
        {
            // get & update references
            CameraSwitcher cameraSwitcher = (CameraSwitcher)target;
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
            EditorGUILayout.PropertyField(useRegisteredList);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(switchCamera);
            EditorGUILayout.Space();

            CustomGroup = useRegisteredList.boolValue;
            if (!CustomGroup)
            {
                EditorGUILayout.PropertyField(customCameraRigsList);
            }

            // apply reference changes
            serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}

#endif
