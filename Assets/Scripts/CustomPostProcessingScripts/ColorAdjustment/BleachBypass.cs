using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Custom-post-processing/ColorAdjustment/BleachBypass")]
    public class BleachBypass : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter Indensity = new ClampedFloatParameter(0, 0, 1);
        
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

