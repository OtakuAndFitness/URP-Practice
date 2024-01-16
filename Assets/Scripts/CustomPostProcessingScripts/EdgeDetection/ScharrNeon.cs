using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/EdgeDetection/ScharrNeon")]
    public class ScharrNeon : VolumeComponent, IPostProcessComponent
    {
        // public ScharrNeonFilterModeParameter FilterMode = new ScharrNeonFilterModeParameter(UnityEngine.FilterMode.Bilinear);
        
        public ClampedFloatParameter edgeWidth = new ClampedFloatParameter(0, 0, 5);
        
        public ClampedFloatParameter edgeNeonFade = new ClampedFloatParameter(1, 0.1f, 1);

        public ClampedFloatParameter backgroundFade = new ClampedFloatParameter(0,0,1);
        
        public ClampedFloatParameter brightness = new ClampedFloatParameter(1, 0.2f, 2);
        
        public ColorParameter backgroundColor = new ColorParameter(Color.white, true, true, true);
        

        public bool IsActive() =>  edgeWidth.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }
        

        // [Serializable]
        // public sealed class ScharrNeonFilterModeParameter : VolumeParameter<FilterMode> { public ScharrNeonFilterModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

    }
}