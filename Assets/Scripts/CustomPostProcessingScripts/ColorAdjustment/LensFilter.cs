using System;
using System.Collections;
using System.Collections.Generic;
using PostProcessingExtends;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/ColorAdjustment/LensFilter")]
    public class LensFilter : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter Indensity = new ClampedFloatParameter(0, 0, 1);
        public ColorParameter LensColor = new ColorParameter(new Color(1.0f, 1.0f, 0.1f, 1), true,true,true);

        public bool IsActive() =>  Indensity.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }
        
    }
}

