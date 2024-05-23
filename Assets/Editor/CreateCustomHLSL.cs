using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateCustomHLSL : EditorWindow
{
    [MenuItem("Assets/Create/Shader/CustomHLSL")]
    public static void CreateHLSLFile()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
        if (!string.IsNullOrEmpty(Path.GetExtension(path)))//选中了某个文件，有了文件扩展名
        {
            string fileName = Path.GetFileName(path);
            path = path.Substring(0, path.Length - fileName.Length);
        }
        string outPutPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(path,
            "NewCustomHLSL.hlsl"));
        
        File.WriteAllText(outPutPath,"");
        
        AssetDatabase.Refresh();
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(outPutPath);
    }
}
