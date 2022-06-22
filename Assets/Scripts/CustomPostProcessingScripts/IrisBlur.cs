using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable,VolumeComponentMenu("Custom-post-processing/Blur/IrisBlur")]
    public class IrisBlur : VolumeComponent, IPostProcessComponent
    {
        public IrisFilerModeParameter filterMode = new IrisFilerModeParameter(FilterMode.Bilinear);
        public ClampedIntParameter blurCount = new ClampedIntParameter(8, 8, 128);
        public ClampedFloatParameter indensity = new ClampedFloatParameter(0f, 0, 3);
        public ClampedFloatParameter centerOffsetX = new ClampedFloatParameter (0f,-1f,1f);
        public ClampedFloatParameter centerOffsetY = new ClampedFloatParameter (0f,-1f,1f);
        public ClampedFloatParameter areaSize = new ClampedFloatParameter (0f,0f,50f);
        
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
    public sealed class IrisFilerModeParameter : VolumeParameter<FilterMode> { public IrisFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

}

