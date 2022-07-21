using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Custom-post-processing/ColorAdjustment/ContrastV2")]
    public class ContrastV2 : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter contrast = new ClampedFloatParameter(0, -1, 5);
        public ClampedFloatParameter contrastFactorR = new ClampedFloatParameter(0,-1,1);
        public ClampedFloatParameter contrastFactorG = new ClampedFloatParameter(0,-1,1);
        public ClampedFloatParameter contrastFactorB = new ClampedFloatParameter(0,-1,1);

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

