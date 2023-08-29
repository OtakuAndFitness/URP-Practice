using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TextureArrayBake
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
}
