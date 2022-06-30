using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable,VolumeComponentMenu("Custom-post-processing/Blur/GrainyBlur")]
    public class GrainyBlur : VolumeComponent, IPostProcessComponent
    {
        public GrainyFilerModeParameter filterMode = new GrainyFilerModeParameter(FilterMode.Bilinear);
        public ClampedIntParameter blurCount = new ClampedIntParameter(1, 1, 4);
        public ClampedIntParameter downSample = new ClampedIntParameter(1, 1, 4);
        public ClampedFloatParameter indensity = new ClampedFloatParameter(0f, 0, 2);
        
        public bool IsActive()
        {
            return active && indensity.value != 0;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
    
    [Serializable]
    public sealed class GrainyFilerModeParameter : VolumeParameter<FilterMode> { public GrainyFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

}

