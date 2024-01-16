using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{    
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Blur/DirectionalBlur")]
    public class DirectionalBlur : VolumeComponent, IPostProcessComponent
    {
        // public DirectionalFilerModeParameter filterMode = new DirectionalFilerModeParameter(FilterMode.Bilinear);
        public ClampedFloatParameter angle = new ClampedFloatParameter(0f, 0f, 6.0f);
        public ClampedFloatParameter blurSize = new ClampedFloatParameter(0.6f, 0.0f, 50.0f);
        public ClampedFloatParameter downScaling = new ClampedFloatParameter(1f, 1f, 10f);
        public ClampedIntParameter iteration = new ClampedIntParameter(0, 0, 30);
        
        public bool IsActive() => iteration.value > 0;

        public bool IsTileCompatible()
        {
            return false;
        }
    }
    
    // [Serializable]
    // public sealed class DirectionalFilerModeParameter : VolumeParameter<FilterMode> { public DirectionalFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }


}
