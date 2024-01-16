using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/ColorAdjustment/Saturation")]
    public class Saturation : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter saturation = new ClampedFloatParameter(1, 0, 2);
        
        public bool IsActive() =>  active && Math.Abs(saturation.value - 1) > 0.001;
        public bool IsTileCompatible()
        {
            return false;
        }
        
    }
}

