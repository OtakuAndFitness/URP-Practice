using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityEngine.Rendering.Universal
{
    [Serializable]
    public class CustomPostProcessingData : ScriptableObject
    {
#if UNITY_EDITOR
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812")]

        [MenuItem("Assets/Create/Rendering/Universal Render Pipeline/Additional Post-process Data", priority = CoreUtils.assetCreateMenuPriority3 + 1)]
        static void CreateAdditionalPostProcessData()
        {
            //ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, CreateInstance<CreatePostProcessDataAsset>(), "CustomPostProcessData.asset", null, null);
            var instance = CreateInstance<CustomPostProcessingData>();
            string assetPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            assetPath = Path.Combine(assetPath, nameof(CustomPostProcessingData) + ".asset");
            if (File.Exists(assetPath))
            {
                File.Delete(assetPath);
                // AssetDatabase.CreateAsset(instance, assetPath);
                // Selection.activeObject = instance;
                // AssetDatabase.Refresh();
            }
            // else
            // {
                AssetDatabase.CreateAsset(instance, assetPath);
                Selection.activeObject = instance;
                AssetDatabase.Refresh();
            // }
            
        }
#endif

        [Serializable]
        public sealed class CustomShaders
        {
            public Shader gaussianBlur;
            public Shader boxBlur;
            public Shader kawaseBlur;
            public Shader dualKawaseBlur;
            public Shader bokehBlur;
        }

        public CustomShaders customShaders;
    }
}

