using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/ColorAdjustment/Tint")]
    public class Tint : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter indensity = new ClampedFloatParameter(0, 0, 1);
        public ColorParameter colorTint = new ColorParameter(new Color(0.9f, 1.0f, 0.0f, 1), true, true, true);

        public bool IsActive() =>  indensity.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }
        
    }
}

