using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Blur/BoxBlur")]
    public class BoxBlur : VolumeComponent, IPostProcessComponent
    {
        // public BoxFilerModeParameter filterMode = new BoxFilerModeParameter(FilterMode.Bilinear);
        public ClampedFloatParameter blurRadius = new ClampedFloatParameter(0.6f, 0.0f, 3.0f);
        public ClampedIntParameter iteration = new ClampedIntParameter(0, 0, 20);
        public ClampedFloatParameter downScaling = new ClampedFloatParameter(2f, 1f, 8f);
        
        public bool IsActive() =>  iteration.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }
        
    }
    
    // [Serializable]
    // public sealed class BoxFilerModeParameter : VolumeParameter<FilterMode> { public BoxFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }
}

