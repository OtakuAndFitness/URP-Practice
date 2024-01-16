using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Sharpen/V3")]
    public class SharpenV3 : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter Sharpness = new ClampedFloatParameter(0, 0,1);

        public bool IsActive() => Sharpness.value > 0;

        public bool IsTileCompatible()
        {
            return false;
        }
        
    }

}
