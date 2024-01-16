using System;
using System.Collections;
using System.Collections.Generic;
using PostProcessingExtends;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Blur/TiltShiftBlur")]
    public class TiltShiftBlur : VolumeComponent, IPostProcessComponent
    {
        // public TiltShiftFilerModeParameter filterMode = new TiltShiftFilerModeParameter(FilterMode.Bilinear);
        public ClampedFloatParameter blurSize = new ClampedFloatParameter(1f, 0f, 3f);
        public ClampedIntParameter iteration = new ClampedIntParameter(0, 0, 128);
        public ClampedFloatParameter centerOffset = new ClampedFloatParameter (0f,-1f,1f);
        public ClampedFloatParameter areaSize = new ClampedFloatParameter (0f,0f,20f);
        public ClampedFloatParameter areaSmooth = new ClampedFloatParameter(1.2f,1f,20f);
        
        public bool IsActive() =>  iteration.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }
        
    }
    
    // [Serializable]
    // public sealed class TiltShiftFilerModeParameter : VolumeParameter<FilterMode> { public TiltShiftFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

}

