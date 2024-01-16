using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Glitch/DigitalStripe")]
    public class DigitalStripe : VolumeComponent, IPostProcessComponent
    {
        // public DigitalStripeFilerModeParameter FilterMode = new DigitalStripeFilerModeParameter(UnityEngine.FilterMode.Bilinear);

        public ClampedFloatParameter indensity = new ClampedFloatParameter(0, 0, 1);

        public ClampedIntParameter frequency = new ClampedIntParameter(3, 1, 10);
        
        public ClampedFloatParameter stripeLength = new ClampedFloatParameter(0.89f, 0, 0.99f);

        public ClampedIntParameter noiseTextureWidth = new ClampedIntParameter(20, 8, 256);

        public ClampedIntParameter noiseTextureHeight = new ClampedIntParameter(20, 8, 256);

        public BoolParameter needStripColorAdjust = new BoolParameter(false);

        public ColorParameter stripColorAdjustColor = new ColorParameter(new Color(0.1f, 0.1f, 0.1f), true,true,true);

        public ClampedFloatParameter stripColorAdjustIndensity = new ClampedFloatParameter(2, 0, 10);
        
        public bool IsActive() =>  indensity.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }
        

        // [Serializable]
        // public sealed class DigitalStripeFilerModeParameter : VolumeParameter<FilterMode> { public DigitalStripeFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

    }
}