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
        }
    }
}

