using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Glitch/ScreenShake")]
    public class ScreenShake : VolumeComponent, IPostProcessComponent
    {
        // public ScreenShakeFilerModeParameter FilterMode = new ScreenShakeFilerModeParameter(UnityEngine.FilterMode.Bilinear);

        public ScreenShakeDirectionParameter ScreenShakeDirection = new ScreenShakeDirectionParameter(Direction.Horizontal);

        // public BoolParameter isHorizontalReverse = new BoolParameter(true);
        
        public ClampedFloatParameter ScreenShakeIndensity = new ClampedFloatParameter(0, 0, 1);

        public bool IsActive() =>  ScreenShakeIndensity.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }
        
        [Serializable]
        public sealed class ScreenShakeDirectionParameter : VolumeParameter<Direction> { public ScreenShakeDirectionParameter(Direction value, bool overrideState = false) : base(value, overrideState) { } }

        // [Serializable]
        // public sealed class ScreenShakeFilerModeParameter : VolumeParameter<FilterMode> { public ScreenShakeFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

    }
}