using System;
using System.Collections;
using System.Collections.Generic;
using PostProcessingExtends;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable, VolumeComponentMenu("Custom-Post-Processing/ColorAdjustment/ContrastV3")]
    public class ContrastV3 : VolumeComponent, IPostProcessComponent
    {
        public Vector4Parameter contrast = new Vector4Parameter(new Vector4(1f, 1f, 1f, -0.1f));
        
        public bool IsActive() => contrast.value != new Vector4(1f,1f,1f,-0.1f);
        public bool IsTileCompatible()
        {
            return false;
        }
        
    }
}

