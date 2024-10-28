using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaterReflectionData))]
public class WaterReflectionDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty refType = serializedObject.FindProperty("reflectionType");
        refType.enumValueIndex = GUILayout.Toolbar(refType.enumValueIndex, refType.enumDisplayNames);

        switch (refType.enumValueIndex)
        {
            case 0:
                SerializedProperty cubeType = serializedObject.FindProperty("cubemapType");
                EditorGUILayout.PropertyField(cubeType, new GUIContent("Cubemap Texture"));
                break;
            case 1:
                EditorGUILayout.HelpBox("Reflection Probe setting has no options, it automatically uses the nearest reflection probe to the main camera", MessageType.Info);
                break;
            case 2:
                SerializedProperty planarSettings = serializedObject.FindProperty("planarReflectionSettings");
                EditorGUILayout.PropertyField(planarSettings, true);
                break;
        }
        

        serializedObject.ApplyModifiedProperties();
    }
}
