using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Vignette/Aurora")]
    public class Aurora : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter vignetteArea = new ClampedFloatParameter(0.8f, 0, 1);
        public ClampedFloatParameter vignetteSmothness = new ClampedFloatParameter(0.5f, 0, 1);
        public ClampedFloatParameter vignetteFading = new ClampedFloatParameter(0, 0, 1);
        public ClampedFloatParameter colorChange = new ClampedFloatParameter(0.1f, 0.1f, 1);
        public ClampedFloatParameter colorFactorR = new ClampedFloatParameter(1, 0, 2);
        public ClampedFloatParameter colorFactorG = new ClampedFloatParameter(1, 0, 2);
        public ClampedFloatParameter colorFactorB = new ClampedFloatParameter(1, 0, 2);
        public ClampedFloatParameter flowSpeed = new ClampedFloatParameter(1, 0, 2);

        public bool IsActive() => vignetteFading.value > 0;

        public bool IsTileCompatible()
        {
            return false;
        }
        
    }
}

