using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Custom-post-processing/ColorAdjustment/Contrast")]
    public class Contrast : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter contrast = new ClampedFloatParameter(0, -1, 2);
        
        public bool IsActive()
        {
            return active && contrast.value != 0;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
}

