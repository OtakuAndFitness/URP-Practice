using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable,VolumeComponentMenu("Custom-post-processing/Blur/TiltShiftBlur")]
    public class TiltShiftBlur : VolumeComponent, IPostProcessComponent
    {
        public TiltShiftFilerModeParameter filterMode = new TiltShiftFilerModeParameter(FilterMode.Bilinear);
        public ClampedIntParameter blurCount = new ClampedIntParameter(8, 8, 128);
        public ClampedFloatParameter indensity = new ClampedFloatParameter(0f, 0, 3);
        public ClampedFloatParameter centerOffset = new ClampedFloatParameter (0f,-1f,1f);
        public ClampedFloatParameter areaSize = new ClampedFloatParameter (0f,0f,20f);
        public ClampedFloatParameter areaSmooth = new ClampedFloatParameter(1.2f,1f,20f);
        
        public bool IsActive()
        {
            return active && indensity != 0;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
    
    [Serializable]
    public sealed class TiltShiftFilerModeParameter : VolumeParameter<FilterMode> { public TiltShiftFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

}

