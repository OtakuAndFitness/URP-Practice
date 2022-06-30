using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{    
    [Serializable,VolumeComponentMenu("Custom-post-processing/Blur/DirectionalBlur")]
    public class DirectionalBlur : VolumeComponent, IPostProcessComponent
    {
        public DirectionalFilerModeParameter filterMode = new DirectionalFilerModeParameter(FilterMode.Bilinear);
        public ClampedFloatParameter angle = new ClampedFloatParameter(0.5f, 0, 6);
        public ClampedIntParameter blurCount = new ClampedIntParameter(1, 1, 30);
        public ClampedIntParameter downSample = new ClampedIntParameter(1, 1, 4);
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
    
    [Serializable]
    public sealed class DirectionalFilerModeParameter : VolumeParameter<FilterMode> { public DirectionalFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }


}
