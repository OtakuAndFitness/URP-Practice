using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Custom-post-processing/ColorAdjustment/Brightness")]
    public class Brightness : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter brightness = new ClampedFloatParameter(0, -1, 10);
        
        public bool IsActive()
        {
            return active && brightness.value != 0;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
}

