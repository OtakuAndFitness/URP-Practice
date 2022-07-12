using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Rendering.Universal
{
    [Serializable,VolumeComponentMenu("Custom-post-processing/Glitch/ScreenShake")]
    public class ScreenShake : VolumeComponent, IPostProcessComponent
    {
        // public ScreenShakeFilerModeParameter FilterMode = new ScreenShakeFilerModeParameter(UnityEngine.FilterMode.Bilinear);

        public ScreenShakeDirectionParameter ScreenShakeDirection = new ScreenShakeDirectionParameter(Direction.Horizontal);

        // public BoolParameter isHorizontalReverse = new BoolParameter(true);
        
        public ClampedFloatParameter ScreenShakeIndensity = new ClampedFloatParameter(0, 0, 1);
        
        
        public bool IsActive()
        {
            return active && ScreenShakeIndensity.value != 0;
        }

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