using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEngine.Rendering.Universal
{
    public class MaterialLibrary
    {
        //blur
        public readonly Material gaussianBlur;
        public readonly Material boxBlur;
        public readonly Material kawaseBlur;
        public readonly Material dualKawaseBlur;
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
        
        public MaterialLibrary(CustomPostProcessingData data)
        {
            //blur
            gaussianBlur = Load(data.customShaders.gaussianBlur);
            boxBlur = Load(data.customShaders.boxBlur);
            kawaseBlur = Load(data.customShaders.kawaseBlur);
            dualKawaseBlur = Load(data.customShaders.dualKawaseBlur);
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
            CoreUtils.Destroy(dualKawaseBlur);
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

        }
    }
}

