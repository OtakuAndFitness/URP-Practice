using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class SaveMesh
{
    [MenuItem("Tools/ExportMesh")]
    public static void MeshExport()
    {
        GameObject selectedItem = Selection.activeObject as GameObject;
        if (selectedItem != null)
        {
            Mesh m = selectedItem.GetComponent<MeshFilter>().mesh;
            AssetDatabase.CreateAsset(m,"Assets/Res/Textures/GrassMesh.mesh");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
    
    
}
