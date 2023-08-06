#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

using Gaskellgames;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames.CameraController3D
{
    [CustomEditor(typeof(CameraFreelookRig))] [CanEditMultipleObjects]
    public class CameraFreelookRigEditor : Editor
    {
        #region Serialized Properties / OnEnable

        SerializedProperty lockCursorOnLoad;
        SerializedProperty gizmosOnSelected;
        SerializedProperty cameraCollisions;
        SerializedProperty customInputAction;

        SerializedProperty moveCamera;
        SerializedProperty playerInputController;

        SerializedProperty follow;
        SerializedProperty topRig;
        SerializedProperty middleRig;
        SerializedProperty bottomRig;

        SerializedProperty xSensitivity;
        SerializedProperty ySensitivity;
        SerializedProperty rotationOffset;
        SerializedProperty collisionOffset;

        bool InputGroup = false;
        bool CollisionGroup = false;

        private void OnEnable()
        {
            customInputAction = serializedObject.FindProperty("customInputAction");
            moveCamera = serializedObject.FindProperty("moveCamera");
            playerInputController = serializedObject.FindProperty("playerInputController");

            gizmosOnSelected = serializedObject.FindProperty("gizmosOnSelected");
            lockCursorOnLoad = serializedObject.FindProperty("lockCursorOnLoad");
            cameraCollisions = serializedObject.FindProperty("cameraCollisions");

            follow = serializedObject.FindProperty("follow");
            topRig = serializedObject.FindProperty("topRig");
            middleRig = serializedObject.FindProperty("middleRig");
            bottomRig = serializedObject.FindProperty("bottomRig");

            xSensitivity = serializedObject.FindProperty("xSensitivity");
            ySensitivity = serializedObject.FindProperty("ySensitivity");
            rotationOffset = serializedObject.FindProperty("rotationOffset");
            collisionOffset = serializedObject.FindProperty("collisionOffset");
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnInspectorGUI

        public override void OnInspectorGUI()
        {
            // get & update references
            CameraFreelookRig cameraFreelookRig = (CameraFreelookRig)target;
            serializedObject.Update();

            /*
            // draw default inspector
            base.OnInspectorGUI();
            */

            // banner
            Texture banner = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Gaskellgames/Camera Controller 3D/Resources/Icons/inspectorBanner_CameraController3D.png", typeof(Texture));
            GUILayout.Box(banner, GUILayout.ExpandWidth(true), GUILayout.Height(Screen.width / 7.5f));

            // custom inspector
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(lockCursorOnLoad);
            EditorGUILayout.PropertyField(gizmosOnSelected);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(cameraCollisions);
            EditorGUILayout.PropertyField(customInputAction);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            InputGroup = customInputAction.boolValue;
            if (InputGroup)
            {
                EditorGUILayout.PropertyField(moveCamera);
                EditorGUILayout.Space();
            }
            else
            {
                EditorGUILayout.PropertyField(playerInputController);
            }

            EditorGUILayout.PropertyField(follow);
            EditorGUILayout.PropertyField(topRig);
            EditorGUILayout.PropertyField(middleRig);
            EditorGUILayout.PropertyField(bottomRig);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(xSensitivity);
            EditorGUILayout.PropertyField(ySensitivity);
            EditorGUILayout.PropertyField(rotationOffset);

            CollisionGroup = cameraCollisions.boolValue;
            if(CollisionGroup)
            {
                EditorGUILayout.PropertyField(collisionOffset);
            }

            // apply reference changes
            serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}

#endif
