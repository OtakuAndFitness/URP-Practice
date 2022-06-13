using System;
using System.Collections;
using System.Collections.Generic;
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
            AssetDatabase.CreateAsset(instance, string.Format("Assets/Settings/{0}.asset", typeof(CustomPostProcessingData).Name));
            Selection.activeObject = instance;
        }
#endif

        [Serializable, ReloadGroup]
        public sealed class CustomShaders
        {
            [Reload("Custom/PostProcessing/GaussianBlur")]
            public Shader gaussianBlur;
        }

        public CustomShaders customShaders;
    }
}

