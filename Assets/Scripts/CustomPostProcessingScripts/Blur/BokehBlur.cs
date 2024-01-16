using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Blur/BokehBlur")]
    public class BokehBlur : VolumeComponent, IPostProcessComponent
    {
        // public BokehFilerModeParameter filterMode = new BokehFilerModeParameter(FilterMode.Bilinear);
        public ClampedFloatParameter blurRadius = new ClampedFloatParameter(0.6f, 0.0f, 10.0f);
        public ClampedIntParameter iteration = new ClampedIntParameter(0, 0, 128);
        public ClampedFloatParameter downScaling = new ClampedFloatParameter(2f, 1f, 8f);
        

        public bool IsActive() => iteration.value > 0;

        public bool IsTileCompatible()
        {
            return false;
        }

        // [Serializable]
        // public sealed class BokehFilerModeParameter : VolumeParameter<FilterMode> { public BokehFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

    }
}

