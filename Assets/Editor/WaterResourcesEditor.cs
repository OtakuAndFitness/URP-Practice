using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaterResources))]
public class WaterResourcesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.indentLevel += 1;

        SerializedProperty defaultFoamRamp = serializedObject.FindProperty("defaultFoamRamp");
        EditorGUILayout.ObjectField(defaultFoamRamp, new GUIContent("Default Foam Ramp"));
        
        SerializedProperty defaultFoamMap = serializedObject.FindProperty("defaultFoamMap");
        EditorGUILayout.ObjectField(defaultFoamMap, new GUIContent("Default Foam Map"));
        
        SerializedProperty defaultSurfaceMap = serializedObject.FindProperty("defaultSurfaceMap");
        EditorGUILayout.ObjectField(defaultSurfaceMap, new GUIContent("Default Surface Map"));
        
        EditorGUI.indentLevel -= 1;

        serializedObject.ApplyModifiedProperties();

    }
}
