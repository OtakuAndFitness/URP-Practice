using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/EdgeDetection/Scharr")]
    public class Scharr : VolumeComponent, IPostProcessComponent
    {
        // public ScharrFilterModeParameter FilterMode = new ScharrFilterModeParameter(UnityEngine.FilterMode.Bilinear);
        
        public ClampedFloatParameter edgeWidth = new ClampedFloatParameter(0, 0, 5);
        
        public ColorParameter edgeColor = new ColorParameter(Color.black, true, true, true);
        
        public ClampedFloatParameter backgroundFade = new ClampedFloatParameter(0, 0, 1);
        
        public ColorParameter backgroundColor = new ColorParameter(Color.white, true, true, true);
        
        //
        public bool IsActive() =>  edgeWidth.value > 0;

        public bool IsTileCompatible()
        {
            return false;
        }
        
        
        // [Serializable]
        // public sealed class ScharrFilterModeParameter : VolumeParameter<FilterMode> { public ScharrFilterModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

    }
}