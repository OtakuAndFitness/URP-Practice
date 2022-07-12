using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable,VolumeComponentMenu("Custom-post-processing/Blur/BoxBlur")]
    public class BoxBlur : VolumeComponent,IPostProcessComponent
    {
        // public BoxFilerModeParameter filterMode = new BoxFilerModeParameter(FilterMode.Bilinear);
        public ClampedIntParameter blurCount = new ClampedIntParameter(1, 1, 10);
        public ClampedIntParameter downSample = new ClampedIntParameter(1, 1, 6);
        public ClampedFloatParameter indensity = new ClampedFloatParameter(0f, 0, 3);
        
        public bool IsActive()
        {
            return active && indensity.value != 0;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
    
    // [Serializable]
    // public sealed class BoxFilerModeParameter : VolumeParameter<FilterMode> { public BoxFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }
}

