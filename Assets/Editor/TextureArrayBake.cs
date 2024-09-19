using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TextureArrayBake : ScriptableObject
{
    [MenuItem("Assets/Textures/Bake TextureArray")]
    public static void BakeTextureArray()
    {
        List<Texture2D> albedoArray = new List<Texture2D>();
        List<Texture2D> normalArray = new List<Texture2D>();
        List<Texture2D> rmoArray = new List<Texture2D>();

        var obj = Selection.activeObject;
        if (obj != null)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (Directory.Exists(path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                FileInfo[] files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);

                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".meta"))
                    {
                        continue;
                    }
                    
                    if (Path.GetFileNameWithoutExtension(files[i].Name).EndsWith("_D"))
                    {
                        Texture2D tex = GetTexture(files[i]);
                        albedoArray.Add(tex);
                    }
                    
                    if (Path.GetFileNameWithoutExtension(files[i].Name).EndsWith("_N"))
                    {
                        Texture2D tex = GetTexture(files[i]);
                        normalArray.Add(tex);
                    }
                    
                    if (Path.GetFileNameWithoutExtension(files[i].Name).EndsWith("_RMO"))
                    {
                        Texture2D tex = GetTexture(files[i]);
                        rmoArray.Add(tex);
                    }
                }

                if (albedoArray.Count > 0)
                {
                    string fileName = albedoArray[0].name + "_AlbedoArray.asset";
                    Bake(albedoArray, path, fileName, false);
                }
                
                if (normalArray.Count > 0)
                {
                    string fileName = normalArray[0].name + "_NormalArray.asset";
                    Bake(normalArray, path, fileName, true);
                }
                
                if (rmoArray.Count > 0)
                {
                    string fileName = rmoArray[0].name + "_RMOArray.asset";
                    Bake(rmoArray, path, fileName, true);
                }
            }
        }
    }

    private static Texture2D GetTexture(FileInfo file)
    {
        string address = Path.GetRelativePath(Application.dataPath, file.ToString());
        address = address.Remove(0, 10);
        string[] addr = address.Split(".");
        Texture2D tex = Resources.Load<Texture2D>(addr[0]);
        return tex;
    }

    private static void Bake(List<Texture2D> texArray, string path, string fileName, bool isLinear)
    {
        Texture2DArray texture2DArray = new Texture2DArray(texArray[0].width, texArray[0].height, texArray.Count,
            texArray[0].format, false, isLinear);
        for (int i = 0; i < texArray.Count; i++)
        {
            Graphics.CopyTexture(texArray[i],0,0,texture2DArray, i,0);
        }
        texture2DArray.wrapMode = TextureWrapMode.Repeat;
        texture2DArray.filterMode = FilterMode.Bilinear;
        AssetDatabase.CreateAsset(texture2DArray, Path.Combine(path, fileName));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
    [MenuItem("Assets/Textures/Modify TextureArray Format(切到iOS平台有效)")]
    public static void ModifyTextureArrayFormat()
    {
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
        {
            EditorUtility.DisplayDialog("请先切到iOS平台", "再执行该功能", "确定");
            return;
        }
        
        var selectedObjs = Selection.objects;

        foreach (var obj in selectedObjs)
        {
            
            string address = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(address))
            {
                Texture2DArray texture2DArray = AssetDatabase.LoadAssetAtPath<Texture2DArray>(address);

                if (texture2DArray == null)
                {
                    EditorUtility.DisplayDialog("选择的文件中有不是TextureArray格式的文件", "该文件后缀名为.asset", "确定");
                    return;
                }

                Texture2DArray newTextureArray = new Texture2DArray(texture2DArray.width, texture2DArray.height,
                    texture2DArray.depth, TextureFormat.ASTC_6x6, false);

                for (int i = 0; i < texture2DArray.depth; i++)
                {
                    RenderTexture rt = new RenderTexture(texture2DArray.width, texture2DArray.height, 0,
                        RenderTextureFormat.ARGB32);
                    rt.Create();
                    
                    Graphics.SetRenderTarget(rt);
                    Graphics.Blit(texture2DArray, rt, i, 0);

                    Texture2D uncompressedTex = new Texture2D(texture2DArray.width, texture2DArray.height,
                        TextureFormat.ARGB32, false);
                    RenderTexture.active = rt;
                    uncompressedTex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
                    uncompressedTex.Apply();
                    RenderTexture.active = null;
                    
                    rt.Release();

                    string tempPath = Path.GetDirectoryName(address) + "temp_" + i + ".png";
                    File.WriteAllBytes(tempPath, uncompressedTex.EncodeToPNG());
                    AssetDatabase.ImportAsset(tempPath, ImportAssetOptions.ForceUpdate);
                    
                    TextureImporter textureImporter = AssetImporter.GetAtPath(tempPath) as TextureImporter;
                    if (textureImporter != null)
                    {
                        textureImporter.textureCompression = TextureImporterCompression.CompressedHQ;
                        textureImporter.SetPlatformTextureSettings(new TextureImporterPlatformSettings
                        {
                            name = "iPhone",
                            overridden = true,
                            format = TextureImporterFormat.ASTC_6x6,
                            maxTextureSize = 2048,
                            compressionQuality = 100
                        });
                        
                        textureImporter.SaveAndReimport();
                    }

                    Texture2D compressedTex = AssetDatabase.LoadAssetAtPath<Texture2D>(tempPath);
                    if (compressedTex != null)
                    {
                        Graphics.CopyTexture(compressedTex, 0, 0, newTextureArray, i, 0);
                    }
                    
                    DestroyImmediate(uncompressedTex);
                }
                
                AssetDatabase.CreateAsset(newTextureArray, address);
                AssetDatabase.SaveAssets();

                for (int i = 0; i < texture2DArray.depth; i++)
                {
                    string tempPath = Path.GetDirectoryName(address) + "temp_" + i + ".png";
                    AssetDatabase.DeleteAsset(tempPath);

                }
                
                AssetDatabase.Refresh();

                EditorUtility.DisplayDialog("成功修改格式", "选中的texture array已经修改为astc格式", "确定");
            }
        }
    
    }
}
