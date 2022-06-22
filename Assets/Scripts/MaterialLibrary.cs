using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEngine.Rendering.Universal
{
    public class MaterialLibrary
    {
        public readonly Material gaussianBlur;
        public readonly Material boxBlur;
        public readonly Material kawaseBlur;
        public readonly Material dualKawaseBlur;
        public readonly Material bokehBlur;
        public readonly Material tiltShiftBlur;
        public readonly Material irisBlur;
        public readonly Material grainyBlur;
        public readonly Material radialBlur;
        
        public MaterialLibrary(CustomPostProcessingData data)
        {
            gaussianBlur = Load(data.customShaders.gaussianBlur);
            boxBlur = Load(data.customShaders.boxBlur);
            kawaseBlur = Load(data.customShaders.kawaseBlur);
            dualKawaseBlur = Load(data.customShaders.dualKawaseBlur);
            bokehBlur = Load(data.customShaders.bokehBlur);
            tiltShiftBlur = Load(data.customShaders.tiltShfitBlur);
            irisBlur = Load(data.customShaders.irisBlur);
            grainyBlur = Load(data.customShaders.grainyBlur);
            radialBlur = Load(data.customShaders.radialBlur);
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
            CoreUtils.Destroy(gaussianBlur);
            CoreUtils.Destroy(boxBlur);
            CoreUtils.Destroy(kawaseBlur);
            CoreUtils.Destroy(dualKawaseBlur);
            CoreUtils.Destroy(bokehBlur);
            CoreUtils.Destroy(tiltShiftBlur);
            CoreUtils.Destroy(irisBlur);
            CoreUtils.Destroy(grainyBlur);
            CoreUtils.Destroy(radialBlur);
        }
    }
}

