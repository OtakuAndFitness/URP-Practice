using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Rendering.Universal
{
    [Serializable,VolumeComponentMenu("Custom-post-processing/Glitch/ScreenJump")]
    public class ScreenJump : VolumeComponent, IPostProcessComponent
    {
        // public ScreenJumpFilerModeParameter FilterMode = new ScreenJumpFilerModeParameter(UnityEngine.FilterMode.Bilinear);

        public ScreenJumpDirectionParameter ScreenJumpDirection = new ScreenJumpDirectionParameter(Direction.Horizontal);

        public BoolParameter isHorizontalReverse = new BoolParameter(true);
        
        public ClampedFloatParameter ScreenJumpIndensity = new ClampedFloatParameter(0, 0, 1);
        
        
        public bool IsActive()
        {
            return active && ScreenJumpIndensity.value != 0;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
        
        [Serializable]
        public sealed class ScreenJumpDirectionParameter : VolumeParameter<Direction> { public ScreenJumpDirectionParameter(Direction value, bool overrideState = false) : base(value, overrideState) { } }

        // [Serializable]
        // public sealed class ScreenJumpFilerModeParameter : VolumeParameter<FilterMode> { public ScreenJumpFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

    }
}