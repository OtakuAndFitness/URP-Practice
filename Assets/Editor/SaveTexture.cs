using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SaveTexture : EditorWindow
{
    public int textureSize = 1024;
    public string texName = "SkinLut";

    private Texture2D tex;

    [MenuItem("Tools/Create Skin Lut")]
    public static void ShowWindow()
    {
        GetWindow(typeof(SaveTexture));
    }

    private void OnGUI()
    {
        GUILayout.Label("Create Skin Lut", EditorStyles.boldLabel);
        textureSize = EditorGUILayout.IntField("Texture Size", textureSize);
        texName = EditorGUILayout.TextField("Texuture Name", texName);
        if (GUILayout.Button("Save Texture"))
        {
            Save();
        }
    }

    private void Save()
    {
        RenderTexture rt = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.ARGB32,
            RenderTextureReadWrite.sRGB);
        tex = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, true);
        MeshRenderer renderer = GameObject.Find("Quad").GetComponent<MeshRenderer>();
        Material mat = renderer.sharedMaterial;
        Graphics.Blit(null,rt,mat);
        RenderTexture previousActive = RenderTexture.active;
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0,0,textureSize,textureSize),0,0,false);
        tex.Apply();
        RenderTexture.active = previousActive;
        
        string savedPath = "/Resources/Textures/" + texName + ".tga";
        File.WriteAllBytes(Application.dataPath + savedPath, tex.EncodeToTGA());

        // DestroyImmediate(tex);
        
        AssetDatabase.ImportAsset(savedPath);
        
        AssetDatabase.Refresh();

        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath("Assets" + savedPath);
        importer.sRGBTexture = false;
        importer.SaveAndReimport();
            
        AssetDatabase.Refresh();
        
    }
    
    
    
}


