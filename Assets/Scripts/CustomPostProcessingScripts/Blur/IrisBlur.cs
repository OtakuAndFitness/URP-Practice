using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Blur/IrisBlur")]
    public class IrisBlur : VolumeComponent, IPostProcessComponent
    {
        // public IrisFilerModeParameter filterMode = new IrisFilerModeParameter(FilterMode.Bilinear);
        public ClampedFloatParameter blurSize = new ClampedFloatParameter(1f, 0f, 3f);
        public ClampedIntParameter iteration = new ClampedIntParameter(0, 0, 128);
        public ClampedFloatParameter centerOffsetX = new ClampedFloatParameter (0f,-1f,1f);
        public ClampedFloatParameter centerOffsetY = new ClampedFloatParameter (0f,-1f,1f);
        public ClampedFloatParameter areaSize = new ClampedFloatParameter (8f,0f,50f);
        
        public bool IsActive() =>  iteration.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }
    }
    
    // [Serializable]
    // public sealed class IrisFilerModeParameter : VolumeParameter<FilterMode> { public IrisFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

}

