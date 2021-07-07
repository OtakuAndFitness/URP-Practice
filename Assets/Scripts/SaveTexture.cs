using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SaveTexture : MonoBehaviour
{
    public int textureSize = 1024;

    private Texture2D tex;

    public void Save()
    {
        RenderTexture rt = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.ARGB32,
            RenderTextureReadWrite.sRGB);
        tex = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, true);
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        Material mat = renderer.sharedMaterial;
        Graphics.Blit(null,rt,mat);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0,0,textureSize,textureSize),0,0,false);
        File.WriteAllBytes(Application.dataPath + "/Res/Textures/" + "SkinLUT2.tga", tex.EncodeToTGA());
    }
    
    
    
}


