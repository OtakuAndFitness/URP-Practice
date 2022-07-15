using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Custom-post-processing/ColorAdjustment/ColorReplace")]
    public class ColorReplace : VolumeComponent, IPostProcessComponent
    {
        public ColorTypeParameter colorType = new ColorTypeParameter(ColorType.Original);
        public ColorParameter FromColor = new ColorParameter(new Color(0.8f, 0.0f, 0.0f, 1), true, true, true);
        public ColorParameter ToColor = new ColorParameter(new Color(0.0f, 0.8f, 0.0f, 1), true, true, true);
        public GradientParameter FromGradientColor = new GradientParameter(null);
        public GradientParameter ToGradientColor = new GradientParameter(null);
        public ClampedFloatParameter gridentSpeed = new ClampedFloatParameter(0.5f, 0, 100);
        public ClampedFloatParameter Range = new ClampedFloatParameter(0, 0, 1);
        public ClampedFloatParameter Fuzziness = new ClampedFloatParameter(0.5f,0,1);
        
        public bool IsActive()
        {
            return active && Range.value != 0;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }

    public enum ColorType
    {
        Original,
        Gradient
    }
    
    [Serializable]
    public sealed class ColorTypeParameter : VolumeParameter<ColorType>{
        public ColorTypeParameter(ColorType value, bool overrideState = false) : base(value, overrideState)
        {
            
        }
    }
    
    [Serializable]
    public sealed class GradientParameter : VolumeParameter<Gradient>{
        public GradientParameter(Gradient value, bool overrideState = false) : base(value, overrideState)
        {
            
        }
    }
}

