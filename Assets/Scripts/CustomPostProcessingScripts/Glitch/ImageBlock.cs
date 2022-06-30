using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable,VolumeComponentMenu("Custom-post-processing/Glitch/ImageBlock")]
    public class ImageBlock : VolumeComponent, IPostProcessComponent
    {
        public GlitchImageBlockFilerModeParameter filterMode = new GlitchImageBlockFilerModeParameter(FilterMode.Bilinear);

        public ClampedFloatParameter Fade = new ClampedFloatParameter(1, 0, 1);

        public ClampedFloatParameter Speed = new ClampedFloatParameter(0.5f, 0, 1);

        public ClampedFloatParameter Amount = new ClampedFloatParameter(0, 0, 10);

        public ClampedFloatParameter BlockLayer1_U = new ClampedFloatParameter(9, 0, 50);

        public ClampedFloatParameter BlockLayer1_V = new ClampedFloatParameter(9, 0, 50);

        public ClampedFloatParameter BlockLayer2_U = new ClampedFloatParameter(5, 0, 50);

        public ClampedFloatParameter BlockLayer2_V = new ClampedFloatParameter(5, 0, 50);

        public ClampedFloatParameter BlockLayer1_Indensity = new ClampedFloatParameter(8, 0, 50);

        public ClampedFloatParameter BlockLayer2_Indensity = new ClampedFloatParameter(4, 0, 50);

        public ClampedFloatParameter RGBSplitIndensity = new ClampedFloatParameter(0.5f, 0, 50);
        
        
        public bool IsActive()
        {
            return active && Amount.value != 0;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
        
        [Serializable]
        public sealed class GlitchImageBlockFilerModeParameter : VolumeParameter<FilterMode> { public GlitchImageBlockFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

    }
}

