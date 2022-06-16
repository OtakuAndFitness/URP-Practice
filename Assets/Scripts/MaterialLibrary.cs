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
        
        public MaterialLibrary(CustomPostProcessingData data)
        {
            gaussianBlur = Load(data.customShaders.gaussianBlur);
            boxBlur = Load(data.customShaders.boxBlur);

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
        }
    }
}

