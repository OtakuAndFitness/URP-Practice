using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable,VolumeComponentMenu("Custom-post-processing/Glitch/RGBSplit")]
    public class RGBSplit : VolumeComponent, IPostProcessComponent
    {
        public GlitchRGBSplitDirectionParameter SplitDirection = new GlitchRGBSplitDirectionParameter(DirectionEX.Horizontal);
        
        public GlitchRGBSplitFilerModeParameter filterMode = new GlitchRGBSplitFilerModeParameter(FilterMode.Bilinear);

        public ClampedFloatParameter Fading = new ClampedFloatParameter(1, 0, 1);

        public ClampedFloatParameter Amount = new ClampedFloatParameter(0, 0, 5);

        public ClampedFloatParameter Speed = new ClampedFloatParameter(1, 0, 10);

        public ClampedFloatParameter CenterFading = new ClampedFloatParameter(1, 0, 1);

        public ClampedFloatParameter AmountR = new ClampedFloatParameter(1, 0, 5);

        public ClampedFloatParameter AmountB = new ClampedFloatParameter(1, 0, 5);
    
        public bool IsActive()
        {
            return active && Amount.value != 0;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }

    [Serializable]
    public sealed class GlitchRGBSplitFilerModeParameter : VolumeParameter<FilterMode> { public GlitchRGBSplitFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

    
    [Serializable]
    public sealed class GlitchRGBSplitDirectionParameter : VolumeParameter<DirectionEX> { public GlitchRGBSplitDirectionParameter(DirectionEX value, bool overrideState = false) : base(value, overrideState) { } }

}

