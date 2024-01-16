using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/ColorAdjustment/Brightness")]
    public class Brightness : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter brightness = new ClampedFloatParameter(0, 0, 10);
        
        public bool IsActive() =>  brightness.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }
        
    }
}

