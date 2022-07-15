using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Custom-post-processing/ColorAdjustment/Saturation")]
    public class Saturation : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter saturation = new ClampedFloatParameter(1, 0, 2);
        
        public bool IsActive()
        {
            return active && saturation.value != 1.0f;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
}

