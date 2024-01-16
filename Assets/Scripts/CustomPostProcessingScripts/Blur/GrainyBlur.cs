using System;
using System.Collections;
using System.Collections.Generic;
using PostProcessingExtends;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Blur/GrainyBlur")]
    public class GrainyBlur : VolumeComponent, IPostProcessComponent
    {
        // public GrainyFilerModeParameter filterMode = new GrainyFilerModeParameter(FilterMode.Bilinear);
        public ClampedFloatParameter blurRadius = new ClampedFloatParameter(0.6f, 0.0f, 30.0f);
        public ClampedFloatParameter downSample = new ClampedFloatParameter(2.0f, 1, 8);
        public ClampedIntParameter iteration = new ClampedIntParameter(0, 0, 8);

        public bool IsActive() =>  iteration.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }
        
    }
    
    // [Serializable]
    // public sealed class GrainyFilerModeParameter : VolumeParameter<FilterMode> { public GrainyFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

}

