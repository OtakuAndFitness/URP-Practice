using System.Collections;
using System.Collections.Generic;
using Cinemachine.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Water))]
public class WaterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Water water = (Water)target;

        SerializedProperty waterReflectionData = serializedObject.FindProperty("waterReflectionData");
        EditorGUILayout.PropertyField(waterReflectionData, true);
        if (waterReflectionData.objectReferenceValue != null)
        {
            CreateEditor(waterReflectionData.objectReferenceValue).OnInspectorGUI();
        }
        
        SerializedProperty waterSurfaceData = serializedObject.FindProperty("waterSurfaceData");
        EditorGUILayout.PropertyField(waterSurfaceData, true);
        if (waterSurfaceData.objectReferenceValue != null)
        {
            CreateEditor(waterSurfaceData.objectReferenceValue).OnInspectorGUI();
        }

        SerializedProperty waterResourcesData = serializedObject.FindProperty("waterResources");
        EditorGUILayout.PropertyField(waterResourcesData, true);
        if (waterResourcesData.objectReferenceValue != null)
        {
            CreateEditor(waterResourcesData.objectReferenceValue).OnInspectorGUI();
        }

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            water.Init();
        }
    }
}
