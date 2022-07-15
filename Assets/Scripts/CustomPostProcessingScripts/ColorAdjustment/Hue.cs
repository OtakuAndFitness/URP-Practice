using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Custom-post-processing/ColorAdjustment/Hue")]
    public class Hue : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter HueDegree = new ClampedFloatParameter(0, -180, 180);
        
        public bool IsActive()
        {
            return active && HueDegree.value != 0;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
}

