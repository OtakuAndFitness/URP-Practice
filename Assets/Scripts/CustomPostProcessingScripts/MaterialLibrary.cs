using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace PostProcessingExtends
{
    public class MaterialLibrary
    {
        //blur
        public readonly Material gaussianBlur;
        public readonly Material boxBlur;
        public readonly Material kawaseBlur;
        public readonly Material bokehBlur;
        public readonly Material tiltShiftBlur;
        public readonly Material irisBlur;
        public readonly Material grainyBlur;
        public readonly Material radialBlur;
        public readonly Material directionalBlur;

        //glitch
        public readonly Material rgbSplit;
        public readonly Material imageBlock;
        public readonly Material lineBlock;
        public readonly Material tileJitter;
        public readonly Material scanLineJitter;
        public readonly Material digitalStripe;
        public readonly Material analogNoise;
        public readonly Material screenJump;
        public readonly Material screenShake;
        public readonly Material waveJitter;

        //Edge Detection
        public readonly Material roberts;
        public readonly Material robertsNeon;
        public readonly Material scharr;
        public readonly Material scharrNeon;
        public readonly Material sobel;
        public readonly Material sobelNeon;

        //Pixelise
        public readonly Material circle;
        public readonly Material diamond;
        public readonly Material hexagon;
        public readonly Material hexagonGrid;
        public readonly Material leaf;
        public readonly Material led;
        public readonly Material quad;
        public readonly Material sector;
        public readonly Material triangle;

        //Vignette
        public readonly Material aurora;
        public readonly Material rapidOldTV;
        public readonly Material rapidOldTVV2;
        public readonly Material rapid;
        public readonly Material rapidV2;

        //Sharpen
        public readonly Material sharpenV1;
        public readonly Material sharpenV2;
        public readonly Material sharpenV3;

        //ColorAdjustment
        public readonly Material bleachBypass;
        public readonly Material brightness;
        public readonly Material hue;
        public readonly Material tint;
        public readonly Material whiteBalance;
        public readonly Material lensFilter;
        public readonly Material saturation;
        public readonly Material technicolor;
        public readonly Material colorReplace;
        public readonly Material colorReplaceV2;
        public readonly Material contrast;
        public readonly Material contrastV2;
        public readonly Material contrastV3;
        
        public readonly Material copyMaterial = null;


        public MaterialLibrary(CustomPostProcessingData data)
        {
            //blur
            gaussianBlur = Load(data.customShaders.gaussianBlur);
            boxBlur = Load(data.customShaders.boxBlur);
            kawaseBlur = Load(data.customShaders.kawaseBlur);
            bokehBlur = Load(data.customShaders.bokehBlur);
            tiltShiftBlur = Load(data.customShaders.tiltShfitBlur);
            irisBlur = Load(data.customShaders.irisBlur);
            grainyBlur = Load(data.customShaders.grainyBlur);
            radialBlur = Load(data.customShaders.radialBlur);
            directionalBlur = Load(data.customShaders.directionalBlur);

            //glitch
            rgbSplit = Load(data.customShaders.rgbSplit);
            imageBlock = Load(data.customShaders.imageBlock);
            lineBlock = Load(data.customShaders.lineBlock);
            tileJitter = Load(data.customShaders.tileJitter);
            scanLineJitter = Load(data.customShaders.scanLineJitter);
            digitalStripe = Load(data.customShaders.digitalStripe);
            analogNoise = Load(data.customShaders.analogNoise);
            screenJump = Load(data.customShaders.screenJump);
            screenShake = Load(data.customShaders.screenShake);
            waveJitter = Load(data.customShaders.waveJitter);

            //Edge Detection
            roberts = Load(data.customShaders.roberts);
            robertsNeon = Load(data.customShaders.robertsNeon);
            scharr = Load(data.customShaders.scharr);
            scharrNeon = Load(data.customShaders.scharrNeon);
            sobel = Load(data.customShaders.sobel);
            sobelNeon = Load(data.customShaders.sobelNeon);

            //Pixelise
            circle = Load(data.customShaders.circle);
            diamond = Load(data.customShaders.diamond);
            hexagon = Load(data.customShaders.hexagon);
            hexagonGrid = Load(data.customShaders.hexagonGrid);
            leaf = Load(data.customShaders.leaf);
            led = Load(data.customShaders.led);
            quad = Load(data.customShaders.quad);
            sector = Load(data.customShaders.sector);
            triangle = Load(data.customShaders.triangle);

            //Vignette
            aurora = Load(data.customShaders.aurora);
            rapidOldTV = Load(data.customShaders.rapidOldTV);
            rapidOldTVV2 = Load(data.customShaders.rapidOldTVV2);
            rapid = Load(data.customShaders.rapid);
            rapidV2 = Load(data.customShaders.rapidV2);

            //Sharpen
            sharpenV1 = Load(data.customShaders.sharpenV1);
            sharpenV2 = Load(data.customShaders.sharpenV2);
            sharpenV3 = Load(data.customShaders.sharpenV3);

            //ColorAdjustment
            bleachBypass = Load(data.customShaders.bleachBypass);
            brightness = Load(data.customShaders.brightness);
            hue = Load(data.customShaders.hue);
            tint = Load(data.customShaders.tint);
            whiteBalance = Load(data.customShaders.whiteBalance);
            lensFilter = Load(data.customShaders.lensFilter);
            saturation = Load(data.customShaders.saturation);
            technicolor = Load(data.customShaders.technicolor);
            colorReplace = Load(data.customShaders.colorReplace);
            colorReplaceV2 = Load(data.customShaders.colorReplaceV2);
            contrast = Load(data.customShaders.contrast);
            contrastV2 = Load(data.customShaders.contrastV2);
            contrastV3 = Load(data.customShaders.contrastV3);
            
            copyMaterial = Load(data.customShaders.copyMaterial);
        }

        Material Load(Shader shader)
        {
            if (shader == null)
            {
                Debug.LogErrorFormat($"Missing shader. {GetType().DeclaringType.Name} render pass will not execute. Check for missing reference in the renderer resources.");
                return null;
            }
            else if (!shader.isSupported)
            {
                return null;
            }

            return CoreUtils.CreateEngineMaterial(shader);
        }

        internal void Cleanup()
        {
            //blur
            CoreUtils.Destroy(gaussianBlur);
            CoreUtils.Destroy(boxBlur);
            CoreUtils.Destroy(kawaseBlur);
            CoreUtils.Destroy(bokehBlur);
            CoreUtils.Destroy(tiltShiftBlur);
            CoreUtils.Destroy(irisBlur);
            CoreUtils.Destroy(grainyBlur);
            CoreUtils.Destroy(radialBlur);
            CoreUtils.Destroy(directionalBlur);

            //glitch
            CoreUtils.Destroy(rgbSplit);
            CoreUtils.Destroy(imageBlock);
            CoreUtils.Destroy(lineBlock);
            CoreUtils.Destroy(tileJitter);
            CoreUtils.Destroy(scanLineJitter);
            CoreUtils.Destroy(digitalStripe);
            CoreUtils.Destroy(screenJump);
            CoreUtils.Destroy(screenShake);
            CoreUtils.Destroy(waveJitter);

            //Edge Detection
            CoreUtils.Destroy(roberts);
            CoreUtils.Destroy(robertsNeon);
            CoreUtils.Destroy(scharr);
            CoreUtils.Destroy(scharrNeon);
            CoreUtils.Destroy(sobel);
            CoreUtils.Destroy(sobelNeon);

            //Pixelise
            CoreUtils.Destroy(circle);
            CoreUtils.Destroy(diamond);
            CoreUtils.Destroy(hexagon);
            CoreUtils.Destroy(hexagonGrid);
            CoreUtils.Destroy(leaf);
            CoreUtils.Destroy(led);
            CoreUtils.Destroy(quad);
            CoreUtils.Destroy(sector);
            CoreUtils.Destroy(triangle);

            //Vignette
            CoreUtils.Destroy(aurora);
            CoreUtils.Destroy(rapidOldTV);
            CoreUtils.Destroy(rapidOldTVV2);
            CoreUtils.Destroy(rapid);
            CoreUtils.Destroy(rapidV2);

            //Sharpen
            CoreUtils.Destroy(sharpenV1);
            CoreUtils.Destroy(sharpenV2);
            CoreUtils.Destroy(sharpenV3);

            //ColorAdjustment
            CoreUtils.Destroy(bleachBypass);
            CoreUtils.Destroy(brightness);
            CoreUtils.Destroy(hue);
            CoreUtils.Destroy(tint);
            CoreUtils.Destroy(whiteBalance);
            CoreUtils.Destroy(lensFilter);
            CoreUtils.Destroy(saturation);
            CoreUtils.Destroy(technicolor);
            CoreUtils.Destroy(colorReplace);
            CoreUtils.Destroy(colorReplaceV2);
            CoreUtils.Destroy(contrast);
            CoreUtils.Destroy(contrastV2);
            CoreUtils.Destroy(contrastV3);
            
            CoreUtils.Destroy(copyMaterial);

        }
    }
}