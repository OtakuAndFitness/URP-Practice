using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable,VolumeComponentMenu("Custom-post-processing/Blur/BokehBlur")]
    public class BokehBlur : VolumeComponent, IPostProcessComponent
    {
        // public BokehFilerModeParameter filterMode = new BokehFilerModeParameter(FilterMode.Bilinear);
        public ClampedIntParameter blurCount = new ClampedIntParameter(8, 8, 128);
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
    
        // [Serializable]
        // public sealed class BokehFilerModeParameter : VolumeParameter<FilterMode> { public BokehFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

    }
}

