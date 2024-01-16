using System;
using System.Collections;
using System.Collections.Generic;
using PostProcessingExtends;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UPostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Blur/KawaseBlur")]
    public class KawaseBlur : VolumeComponent, IPostProcessComponent
    {
        // public KawaseFilerModeParameter filterMode = new KawaseFilerModeParameter(FilterMode.Bilinear);
        public ClampedFloatParameter blurSize= new ClampedFloatParameter(0.6f, 0.0f, 3.0f);
        public ClampedFloatParameter downSample = new ClampedFloatParameter(2f, 1f, 8f);
        public ClampedIntParameter iteration = new ClampedIntParameter(0, 0, 20);
        
        public bool IsActive() =>  iteration.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }
        
    }
    
    // [Serializable]
    // public sealed class KawaseFilerModeParameter : VolumeParameter<FilterMode> { public KawaseFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

}

