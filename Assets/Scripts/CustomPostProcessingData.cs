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

        [MenuItem("Assets/Create/Rendering/Universal Render Pipeline/Custom Post-process Data", priority = CoreUtils.assetCreateMenuPriority3 + 1)]
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
                AssignShaders(instance.customShaders);
                EditorUtility.SetDirty(instance);
                Selection.activeObject = instance;
                AssetDatabase.SaveAssets();
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
            public Shader sharpenV1;
            public Shader sharpenV2;
            public Shader sharpenV3;
            
            //ColorAdjustment
            public Shader bleachBypass;
            public Shader brightness;
            public Shader hue;
            public Shader tint;
            public Shader whiteBalance;
            public Shader lensFilter;
            public Shader saturation;
            public Shader technicolor;
            public Shader colorReplace;
            public Shader contrast;
            public Shader contrastV2;
            public Shader contrastV3;
        }
        
        public CustomShaders customShaders;

        private static void AssignShaders(CustomShaders customShaders)
        {
            if (customShaders == null)
            {
                Debug.LogError("Custom Shaders is null");
                return;
            }
            
            customShaders.gaussianBlur = Shader.Find("Custom/PostProcessing/Blur/GaussianBlur");
            customShaders.boxBlur = Shader.Find("Custom/PostProcessing/Blur/BoxBlur");
            customShaders.kawaseBlur = Shader.Find("Custom/PostProcessing/Blur/KawaseBlur");
            customShaders.dualKawaseBlur = Shader.Find("Custom/PostProcessing/Blur/DualKawaseBlur");
            customShaders.bokehBlur = Shader.Find("Custom/PostProcessing/Blur/BokehBlur");
            customShaders.tiltShfitBlur = Shader.Find("Custom/PostProcessing/Blur/TiltShiftBlur");
            customShaders.irisBlur = Shader.Find("Custom/PostProcessing/Blur/IrisBlur");
            customShaders.grainyBlur = Shader.Find("Custom/PostProcessing/Blur/GrainyBlur");
            customShaders.radialBlur = Shader.Find("Custom/PostProcessing/Blur/RadialBlur");
            customShaders.directionalBlur = Shader.Find("Custom/PostProcessing/Blur/DirectionalBlur");
            customShaders.rgbSplit = Shader.Find("Custom/PostProcessing/Glitch/RGBSplit");
            customShaders.imageBlock = Shader.Find("Custom/PostProcessing/Glitch/ImageBlock");
            customShaders.lineBlock = Shader.Find("Custom/PostProcessing/Glitch/LineBlock");
            customShaders.tileJitter = Shader.Find("Custom/PostProcessing/Glitch/TileJitter");
            customShaders.scanLineJitter = Shader.Find("Custom/PostProcessing/Glitch/ScanLineJitter");
            customShaders.digitalStripe = Shader.Find("Custom/PostProcessing/Glitch/DigitalStripe");
            customShaders.analogNoise = Shader.Find("Custom/PostProcessing/Glitch/AnalogNoise");
            customShaders.screenJump = Shader.Find("Custom/PostProcessing/Glitch/ScreenJump");
            customShaders.screenShake = Shader.Find("Custom/PostProcessing/Glitch/ScreenShake");
            customShaders.waveJitter = Shader.Find("Custom/PostProcessing/Glitch/WaveJitter");
            customShaders.roberts = Shader.Find("Custom/PostProcessing/EdgeDetection/Roberts");
            customShaders.robertsNeon = Shader.Find("Custom/PostProcessing/EdgeDetection/RobertsNeon");
            customShaders.scharr = Shader.Find("Custom/PostProcessing/EdgeDetection/Scharr");
            customShaders.scharrNeon = Shader.Find("Custom/PostProcessing/EdgeDetection/ScharrNeon");
            customShaders.sobel = Shader.Find("Custom/PostProcessing/EdgeDetection/Sobel");
            customShaders.sobelNeon = Shader.Find("Custom/PostProcessing/EdgeDetection/SobelNeon");
            customShaders.circle = Shader.Find("Custom/PostProcessing/Pixelise/Circle");
            customShaders.diamond = Shader.Find("Custom/PostProcessing/Pixelise/Diamond");
            customShaders.hexagon = Shader.Find("Custom/PostProcessing/Pixelise/Hexagon");
            customShaders.hexagonGrid = Shader.Find("Custom/PostProcessing/Pixelise/HexagonGrid");
            customShaders.leaf = Shader.Find("Custom/PostProcessing/Pixelise/Leaf");
            customShaders.led = Shader.Find("Custom/PostProcessing/Pixelise/Led");
            customShaders.quad = Shader.Find("Custom/PostProcessing/Pixelise/Quad");
            customShaders.sector = Shader.Find("Custom/PostProcessing/Pixelise/Sector");
            customShaders.triangle = Shader.Find("Custom/PostProcessing/Pixelise/Triangle");
            customShaders.aurora = Shader.Find("Custom/PostProcessing/Vignette/Aurora");
            customShaders.rapidOldTV = Shader.Find("Custom/PostProcessing/Vignette/RapidOldTV");
            customShaders.rapidOldTVV2 = Shader.Find("Custom/PostProcessing/Vignette/RapidOldTVV2");
            customShaders.rapid = Shader.Find("Custom/PostProcessing/Vignette/Rapid");
            customShaders.rapidV2 = Shader.Find("Custom/PostProcessing/Vignette/RapidV2");
            customShaders.sharpenV1 = Shader.Find("Custom/PostProcessing/Sharpen/SharpenV1");
            customShaders.sharpenV2 = Shader.Find("Custom/PostProcessing/Sharpen/SharpenV2");
            customShaders.sharpenV3 = Shader.Find("Custom/PostProcessing/Sharpen/SharpenV3");
            customShaders.bleachBypass = Shader.Find("Custom/PostProcessing/ColorAdjustment/BleachBypass");
            customShaders.brightness = Shader.Find("Custom/PostProcessing/ColorAdjustment/Brightness");
            customShaders.hue = Shader.Find("Custom/PostProcessing/ColorAdjustment/Hue");
            customShaders.tint = Shader.Find("Custom/PostProcessing/ColorAdjustment/Tint");
            customShaders.whiteBalance = Shader.Find("Custom/PostProcessing/ColorAdjustment/WhiteBalance");
            customShaders.lensFilter = Shader.Find("Custom/PostProcessing/ColorAdjustment/LensFilter");
            customShaders.saturation = Shader.Find("Custom/PostProcessing/ColorAdjustment/Saturation");
            customShaders.technicolor = Shader.Find("Custom/PostProcessing/ColorAdjustment/Technicolor");
            customShaders.colorReplace = Shader.Find("Custom/PostProcessing/ColorAdjustment/ColorReplace");
            customShaders.contrast = Shader.Find("Custom/PostProcessing/ColorAdjustment/Contrast");
            customShaders.contrastV2 = Shader.Find("Custom/PostProcessing/ColorAdjustment/ContrastV2");
            customShaders.contrastV3 = Shader.Find("Custom/PostProcessing/ColorAdjustment/ContrastV3");
            
        }
    }
    
    

}

