using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Custom-post-processing/ColorAdjustment/WhiteBalance")]
    public class WhiteBalance : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter temperature = new ClampedFloatParameter(0, -1, 1);
        public ClampedFloatParameter tint = new ClampedFloatParameter(0,-1,1);
        
        public bool IsActive()
        {
            return active && tint.value != 0;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
}

