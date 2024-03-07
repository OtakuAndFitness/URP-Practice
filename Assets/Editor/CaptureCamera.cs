using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CaptureCamera : EditorWindow
{
    public static int textureWidth = 3300;
    public static int textureHeight = 2048;
    public static int GameViewWidth;
    public static int GameViewHeight;
    public static string textureName = "zhucheng";
    public static IsUseAlpha isUseAlpha = IsUseAlpha.None;
    public static PictureFormat texFormat = PictureFormat.TGA;
    private static string path;
    private static int width;
    private static int height;

    
    public enum IsUseAlpha
    {
        None,
        Alpha
    }
    
    public enum PictureFormat
    {
        PNG,
        JPG,
        TGA
    }
    
    [MenuItem("Tools/CaptureCamera")]
    public static void ShowWindow()
    {
        //Open Window
        path = PlayerPrefs.GetString("filePath") == String.Empty ? Application.dataPath : PlayerPrefs.GetString("filePath");
        textureWidth = 3300;
        textureHeight = 2048;
        texFormat = PictureFormat.TGA;
        isUseAlpha = IsUseAlpha.None;
        GetWindow(typeof(CaptureCamera));
        
    }
    

    private void OnGUI()
    {
        GUILayout.Label("Capture Camera", EditorStyles.boldLabel);
        string[] resolution = UnityStats.screenRes.Split('x');
        width = int.Parse(resolution[0]);
        height = int.Parse(resolution[1]);
        GUILayout.Label("GameView Resolution(改变游戏视窗才可以改变):",EditorStyles.miniBoldLabel);
        GUI.enabled = false;
        GameViewWidth = EditorGUILayout.IntField("GameView Width", width);
        GameViewHeight = EditorGUILayout.IntField("GameView Height", height);
        GUI.enabled = true;
        GUILayout.Label("Texture Resolution（使用者自由设置）:",EditorStyles.miniBoldLabel);
        textureWidth = EditorGUILayout.IntField("Texture Width", textureWidth);
        textureHeight = EditorGUILayout.IntField("Texture Height", textureHeight);
        if (textureHeight > height || textureHeight == 0)
            textureHeight = height;
        if (textureWidth > width || textureWidth == 0)
            textureWidth = width;
        textureWidth = Math.Abs(textureWidth);
        textureHeight = Math.Abs(textureHeight);
        textureName = EditorGUILayout.TextField("Texture Name", textureName);
        if (String.IsNullOrEmpty(textureName))
            textureName = "zhucheng";
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Texture Format");
            texFormat = (PictureFormat)EditorGUILayout.EnumPopup(texFormat);
        }
        GUILayout.EndHorizontal();
        if (texFormat == PictureFormat.JPG)
        {
            GUI.enabled = false;
        }
        else 
        {
            GUI.enabled = true;
        }
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Use Alpha?");
            isUseAlpha = (IsUseAlpha)EditorGUILayout.EnumPopup(isUseAlpha);
        }
        GUILayout.EndHorizontal();
        GUI.enabled = true;
        GUILayout.Label("Address:");
        GUILayout.BeginHorizontal();
        {
            path = GUILayout.TextField(path, GUILayout.MaxWidth(240f));
            if (GUILayout.Button("Browse", GUILayout.Width(60f)))
            {
                path = EditorUtility.OpenFolderPanel("Address", Application.dataPath, "");
            }
            
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Capture"))
        {
            CaptureCameraCo();
        }
    }

    static void CaptureCameraCo()
    {
        Camera camera = Camera.main;
        Texture2D screenShot = new Texture2D(textureWidth, textureHeight, isUseAlpha == IsUseAlpha.Alpha ? TextureFormat.ARGB32 : TextureFormat.RGB24, false);
        Rect rect = new Rect(0, 0, textureWidth, textureHeight);
        // 创建一个RenderTexture对象
        RenderTexture rt = new RenderTexture(width, height, 0);
        // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机
        camera.targetTexture = rt;
        camera.Render();
        //ps: --- 如果这样加上第二个相机，可以实现只截图某几个指定的相机一起看到的图像。
        //ps: camera2.targetTexture = rt;
        //ps: camera2.Render();
        //ps: -------------------------------------------------------------------
        // 激活这个rt, 并从中中读取像素。
        RenderTexture.active = rt;
        //Texture2D screenShot = new Texture2D((int) rect.width, (int) rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0); // 注：这个时候，它是从RenderTexture.active中读取像素
        screenShot.Apply();
        
        // 最后将这些纹理数据，成一个png图片文件
        // string path = "Assets/zhucheng.jpg";
        if (String.IsNullOrEmpty(path))
            path = Application.dataPath;
        bool isValidate = Directory.Exists(path);
        if (!isValidate)
        {
            EditorUtility.DisplayDialog("路径错误:", "请重新选择路径", "确定");
            return;
        }

        byte[] bytes = null;
        string filePath = null;
        switch (texFormat)
        {
            case PictureFormat.TGA:
                filePath = Path.Combine(path, textureName + ".tga");
                bytes = screenShot.EncodeToTGA();
                break;
            case PictureFormat.JPG:
                filePath = Path.Combine(path, textureName + ".jpg");
                bytes = screenShot.EncodeToJPG();
                break;
            case PictureFormat.PNG:
                filePath = Path.Combine(path, textureName + ".png");
                bytes = screenShot.EncodeToPNG();
                break;
        }

        if (bytes != null && !string.IsNullOrEmpty(filePath))
        {
            PlayerPrefs.SetString("filePath", path);
            File.WriteAllBytes(filePath, bytes);
        }
        else
        {
            EditorUtility.DisplayDialog("截图失败:", "截图格式有问题", "确定");
            return;
        }

        // 重置相关参数，以使用camera继续在屏幕上显示
        camera.targetTexture = null;
        //ps: camera2.targetTexture = ;
        RenderTexture.active = null; // JC: added to avoid errors
        // GameObject.Destroy(rt);
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("截图完成:", "已保存在"+ path, "确定");
    }
}