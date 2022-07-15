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
            //blur
            public Shader gaussianBlur;
            public Shader boxBlur;
            public Shader kawaseBlur;
            public Shader dualKawaseBlur;
            public Shader bokehBlur;
            public Shader tiltShfitBlur;
            public Shader irisBlur;
            public Shader grainyBlur;
            public Shader radialBlur;
            public Shader directionalBlur;
            
            //glitch
            public Shader rgbSplit;
            public Shader imageBlock;
            public Shader lineBlock;
            public Shader tileJitter;
            public Shader scanLineJitter;
            public Shader digitalStripe;
            public Shader analogNoise;
            public Shader screenJump;
            public Shader screenShake;
            public Shader waveJitter;
            
            //Edge Detection
            public Shader roberts;
            public Shader robertsNeon;
            public Shader scharr;
            public Shader scharrNeon;
            public Shader sobel;
            public Shader sobelNeon;
            
            //Pixelise
            public Shader circle;
            public Shader diamond;
            public Shader hexagon;
            public Shader hexagonGrid;
            public Shader leaf;
            public Shader led;
            public Shader quad;
            public Shader sector;
            public Shader triangle;
            
            //Vignette
            public Shader aurora;
            public Shader rapidOldTV;
            public Shader rapidOldTVV2;
            public Shader rapid;
            public Shader rapidV2;
            
            //Sharpen
            public Shader v1;
            public Shader v2;
            public Shader v3;
            
            //ColorAdjustment
            public Shader bleachBypass;
            public Shader brightness;
            public Shader hue;
            public Shader tint;
            public Shader whiteBalance;
        }

        public CustomShaders customShaders;
    }
}

