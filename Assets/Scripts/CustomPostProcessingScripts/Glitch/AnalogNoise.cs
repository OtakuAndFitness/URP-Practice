using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Rendering.Universal
{
    [Serializable,VolumeComponentMenu("Custom-post-processing/Glitch/AnalogNoise")]
    public class AnalogNoise : VolumeComponent, IPostProcessComponent
    {
        public AnalogNoiseModeParameter FilterMode = new AnalogNoiseModeParameter(UnityEngine.FilterMode.Bilinear);
        
        public ClampedFloatParameter NoiseSpeed = new ClampedFloatParameter(0.5f, 0, 1);
        
        public ClampedFloatParameter NoiseFading = new ClampedFloatParameter(0, 0, 1);
        
        public ClampedFloatParameter LuminanceJitterThreshold = new ClampedFloatParameter(0.8f, 0, 1);


        public bool IsActive()
        {
            return active && NoiseFading.value != 0;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
        

        [Serializable]
        public sealed class AnalogNoiseModeParameter : VolumeParameter<FilterMode> { public AnalogNoiseModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

    }
}