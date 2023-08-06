#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

using Gaskellgames;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames.CameraController3D
{
    [CustomEditor(typeof(CameraRig))] [CanEditMultipleObjects]
    public class CameraRigEditor : Editor
    {
        #region Serialized Properties / OnEnable

        SerializedProperty freelookRig;
        SerializedProperty registerCamera;
        SerializedProperty follow;
        SerializedProperty lookAt;
        SerializedProperty turnSpeed;
        SerializedProperty followOffset;
        SerializedProperty lens;

        private void OnEnable()
        {
            freelookRig = serializedObject.FindProperty("freelookRig");
            registerCamera = serializedObject.FindProperty("registerCamera");
            follow = serializedObject.FindProperty("follow");
            lookAt = serializedObject.FindProperty("lookAt");
            turnSpeed = serializedObject.FindProperty("turnSpeed");
            followOffset = serializedObject.FindProperty("followOffset");
            lens = serializedObject.FindProperty("lens");
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnInspectorGUI

        public override void OnInspectorGUI()
        {
            // get & update references
            CameraRig cameraRig = (CameraRig)target;
            serializedObject.Update();

            /*
            // draw default inspector
            base.OnInspectorGUI();
            */

            // banner
            Texture banner = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Gaskellgames/Camera Controller 3D/Resources/Icons/inspectorBanner_CameraController3D.png", typeof(Texture));
            GUILayout.Box(banner, GUILayout.ExpandWidth(true), GUILayout.Height(Screen.width / 7.5f));

            // custom inspector
            EditorGUILayout.PropertyField(freelookRig);
            EditorGUILayout.PropertyField(registerCamera);
            EditorGUILayout.PropertyField(follow);
            EditorGUILayout.PropertyField(lookAt);
            EditorGUILayout.PropertyField(turnSpeed);
            EditorGUILayout.PropertyField(followOffset);
            EditorGUILayout.PropertyField(lens);

            if(freelookRig.objectReferenceValue != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("follow, lookAt, turnSpeed & followOffset are being driven by the Freelook Rig", MessageType.Warning);
            }

            // apply reference changes
            serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}

#endif
