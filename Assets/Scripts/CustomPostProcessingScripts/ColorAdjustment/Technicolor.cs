using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Custom-post-processing/ColorAdjustment/Technicolor")]
    public class Technicolor : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter exposure = new ClampedFloatParameter(4, 0, 8);
        public ClampedFloatParameter indensity = new ClampedFloatParameter(0,0,1);
        public ClampedFloatParameter colorBalanceR = new ClampedFloatParameter(0.2f,0,1);
        public ClampedFloatParameter colorBalanceG = new ClampedFloatParameter(0.2f,0,1);
        public ClampedFloatParameter colorBalanceB = new ClampedFloatParameter(0.2f,0,1);

        
        public bool IsActive()
        {
            return active && indensity.value != 0;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
}

