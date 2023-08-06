#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

using Gaskellgames;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames.CameraController3D
{
    [CustomEditor(typeof(CameraTriggerSensor))] [CanEditMultipleObjects]
    public class CameraTriggerSensorEditor : Editor
    {
        #region Serialized Properties / OnEnable

        SerializedProperty cameraRig;
        SerializedProperty alwaysShowZone;
        SerializedProperty sensorColour;
        SerializedProperty sensorOutlineColour;

        bool InfoGroup = false;

        private void OnEnable()
        {
            cameraRig = serializedObject.FindProperty("cameraRig");
            alwaysShowZone = serializedObject.FindProperty("alwaysShowZone");
            sensorColour = serializedObject.FindProperty("sensorColour");
            sensorOutlineColour = serializedObject.FindProperty("sensorOutlineColour");
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnInspectorGUI

        public override void OnInspectorGUI()
        {
            // get & update references
            CameraTriggerSensor cameraTriggerSensor = (CameraTriggerSensor)target;
            serializedObject.Update();

            /*
            // draw default inspector
            base.OnInspectorGUI();
            */

            // banner
            Texture banner = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Gaskellgames/Camera Controller 3D/Resources/Icons/inspectorBanner_CameraController3D.png", typeof(Texture));
            GUILayout.Box(banner, GUILayout.ExpandWidth(true), GUILayout.Height(Screen.width / 7.5f));

            // custom inspector
            EditorGUILayout.PropertyField(cameraRig);
            EditorGUILayout.Space();

            InfoGroup = EditorGUILayout.BeginFoldoutHeaderGroup(InfoGroup, "Sensor Info");
            if (InfoGroup)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(alwaysShowZone);
                EditorGUILayout.PropertyField(sensorColour);
                EditorGUILayout.PropertyField(sensorOutlineColour);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space();

            // apply reference changes
            serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}

#endif
