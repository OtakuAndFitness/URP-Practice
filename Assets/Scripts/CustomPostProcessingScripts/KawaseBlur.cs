using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable,VolumeComponentMenu("Custom-post-processing/KawaseBlur")]
    public class KawaseBlur : VolumeComponent, IPostProcessComponent
    {
        public KawaseFilerModeParameter filterMode = new KawaseFilerModeParameter(FilterMode.Bilinear);
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
    
    [Serializable]
    public sealed class KawaseFilerModeParameter : VolumeParameter<FilterMode> { public KawaseFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

}

