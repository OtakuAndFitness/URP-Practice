using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Custom-post-processing/Sharpen/V3")]
    public class V3 : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter Sharpness = new ClampedFloatParameter(0, 0,1);

        public bool IsActive()
        {
            return active && Sharpness.value != 0;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }

}
