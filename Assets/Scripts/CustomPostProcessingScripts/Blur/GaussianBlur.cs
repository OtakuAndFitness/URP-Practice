using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Blur/GaussianBlur")]
    public class GaussianBlur : VolumeComponent, IPostProcessComponent
    {
        // public GaussianFilerModeParameter filterMode = new GaussianFilerModeParameter(FilterMode.Bilinear);
        public ClampedFloatParameter blurRadius = new ClampedFloatParameter(3f, 0f, 5f);
        public ClampedIntParameter iteration = new ClampedIntParameter(0, 0, 15);
        public ClampedFloatParameter downScaling = new ClampedFloatParameter(2f, 1f, 8f);
        
        
        public bool IsActive() =>  iteration.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }
        
    }
    
    // [Serializable]
    // public sealed class GaussianFilerModeParameter : VolumeParameter<FilterMode> { public GaussianFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

}

