using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Custom-post-processing/ColorAdjustment/LensFilter")]
    public class LensFilter : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter Indensity = new ClampedFloatParameter(0, 0, 1);
        public ColorParameter LensColor = new ColorParameter(new Color(1.0f, 1.0f, 0.1f, 1), true,true,true);
        
        public bool IsActive()
        {
            return active && Indensity.value != 0;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
}

