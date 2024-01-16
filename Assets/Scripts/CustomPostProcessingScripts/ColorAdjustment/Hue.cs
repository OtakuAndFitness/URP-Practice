using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/ColorAdjustment/Hue")]
    public class Hue : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter HueDegree = new ClampedFloatParameter(0, -180, 180);
        
        public bool IsActive() =>  HueDegree.value != 0;
        public bool IsTileCompatible()
        {
            return false;
        }
        
    }
}

