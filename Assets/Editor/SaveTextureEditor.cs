using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(SaveTexture))]
public class SaveTextureEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        SaveTexture st = (SaveTexture) target;

        if (GUILayout.Button("SaveTexture"))
        {
            st.Save();
            AssetDatabase.Refresh();
        }
    }
}
