using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Sharpen/V2")]
    public class SharpenV2 : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter Sharpness = new ClampedFloatParameter(0, 0,5);

        public bool IsActive() => Sharpness.value > 0;

        public bool IsTileCompatible()
        {
            return false;
        }
        
    }

}
