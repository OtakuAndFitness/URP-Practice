using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable,VolumeComponentMenu("Custom-post-processing/Blur/RadialBlur")]
    public class RadialBlur : VolumeComponent, IPostProcessComponent
    {
        public RadialBlurQualityParameter qualityLevel = new RadialBlurQualityParameter(RadialBlurQuality.RadialBlur_8Tap_Balance);
        public RadialFilerModeParameter filterMode = new RadialFilerModeParameter(FilterMode.Bilinear);
        public ClampedFloatParameter RadialCenterX = new ClampedFloatParameter(0f, 0, 1);
        public ClampedFloatParameter RadialCenterY = new ClampedFloatParameter(0f, 0, 1);
        public ClampedFloatParameter indensity = new ClampedFloatParameter(0f, 0, 3);
        
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
    public sealed class RadialFilerModeParameter : VolumeParameter<FilterMode> { public RadialFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }
    
    [Serializable]
    public sealed class RadialBlurQualityParameter : VolumeParameter<RadialBlurQuality> { public RadialBlurQualityParameter(RadialBlurQuality value, bool overrideState = false) : base(value, overrideState) { } }

    
    public enum RadialBlurQuality
    {
        RadialBlur_4Tap_Fatest = 0,
        RadialBlur_6Tap = 1,
        RadialBlur_8Tap_Balance = 2,
        RadialBlur_10Tap = 3,
        RadialBlur_12Tap = 4,
        RadialBlur_20Tap_Quality = 5,
        RadialBlur_30Tap_Extreme = 6,
    }

}

