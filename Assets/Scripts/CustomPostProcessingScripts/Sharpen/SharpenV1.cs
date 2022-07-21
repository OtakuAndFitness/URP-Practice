using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Custom-post-processing/Sharpen/V1")]
    public class SharpenV1 : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter Strength = new ClampedFloatParameter(0, 0,5);
        public ClampedFloatParameter Threshold = new ClampedFloatParameter(0.1f, 0,1);

        public bool IsActive()
        {
            return active && Strength.value != 0;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }

}
