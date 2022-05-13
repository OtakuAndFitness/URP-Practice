using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateURPUnlitShader
{
    private static string templatePath = "Assets/Editor/UnlitShader.txt";

    [MenuItem("Assets/Create/Shader/CustomUnlit")]
    static void CreateCustomShader()
    {
        StreamReader inp_stm = new StreamReader(templatePath);
        string outPutPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(AssetDatabase.GetAssetPath(Selection.activeInstanceID),
            "NewUnlitShader.shader"));
        
        File.WriteAllText(outPutPath,inp_stm.ReadToEnd());
        
        inp_stm.Close( );
        AssetDatabase.Refresh();
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(outPutPath);
        
    }

}
