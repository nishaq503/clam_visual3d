using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(NodeScript))]
public class LevelScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        NodeScript myTarget = (NodeScript)target;

        myTarget.test = EditorGUILayout.IntField("Experience", myTarget.test);
        EditorGUILayout.LabelField("Level", myTarget.test.ToString());
    }
}

