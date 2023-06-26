using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateURPUnlitShader
{
    private static string templatePath = "Assets/Editor/UnlitShaderTemplate.txt";

    [MenuItem("Assets/Create/Shader/CustomUnlit")]
    static void CreateCustomShader()
    {
        StreamReader inp_stm = new StreamReader(templatePath);
        string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
        if (!string.IsNullOrEmpty(Path.GetExtension(path)))//选中了某个文件，有了文件扩展名
        {
            string fileName = Path.GetFileName(path);
            path = path.Substring(0, path.Length - fileName.Length);
        }
        string outPutPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(path,
            "NewUnlitShader.shader"));
        
        File.WriteAllText(outPutPath,inp_stm.ReadToEnd());
        
        inp_stm.Close( );
        AssetDatabase.Refresh();
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(outPutPath);
        
    }

}
