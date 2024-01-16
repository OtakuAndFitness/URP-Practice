using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/ColorAdjustment/Contrast")]
    public class Contrast : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter contrast = new ClampedFloatParameter(0, 0, 2);
        
        public bool IsActive() => contrast.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }
    }
}

