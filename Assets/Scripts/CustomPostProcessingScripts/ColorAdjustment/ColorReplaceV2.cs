using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/ColorAdjustment/ColorReplaceV2")]
    public class ColorReplaceV2 : VolumeComponent, IPostProcessComponent
    {
        public GradientParameter FromGradientColor = new GradientParameter(null);
        public GradientParameter ToGradientColor = new GradientParameter(null);
        public ClampedFloatParameter gridentSpeed = new ClampedFloatParameter(0.5f, 0, 100);
        public ClampedFloatParameter Range = new ClampedFloatParameter(0, 0, 1);
        public ClampedFloatParameter Fuzziness = new ClampedFloatParameter(0.5f,0f,1f);

        public bool IsActive() =>  Range.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }
    }
    
    // [Serializable]
    // public sealed class ColorTypeParameter : VolumeParameter<ColorType>{
    //     public ColorTypeParameter(ColorType value, bool overrideState = false) : base(value, overrideState)
    //     {
    //         
    //     }
    // }
    
    [Serializable]
    public sealed class GradientParameter : VolumeParameter<Gradient>{
        public GradientParameter(Gradient value, bool overrideState = false) : base(value, overrideState)
        {
            
        }
    }
}

