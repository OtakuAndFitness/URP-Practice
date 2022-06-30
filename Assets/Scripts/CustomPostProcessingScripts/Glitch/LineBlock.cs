using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Rendering.Universal
{
    [Serializable,VolumeComponentMenu("Custom-post-processing/Glitch/LineBlock")]
    public class LineBlock : VolumeComponent, IPostProcessComponent
    {
        public LineBlockFilerModeParameter FilterMode = new LineBlockFilerModeParameter(UnityEngine.FilterMode.Bilinear);

        public DirectionParameter BlockDirection = new DirectionParameter(Direction.Horizontal);

        public IntervalTypeParameter IntervalType = new IntervalTypeParameter(Universal.IntervalType.Random);
        
        public ClampedFloatParameter Frequency = new ClampedFloatParameter(1, 0, 25);
        
        public ClampedFloatParameter Amount = new ClampedFloatParameter(0, 0, 1);

        public ClampedFloatParameter LinesWidth = new ClampedFloatParameter(1, 0.1f, 10);

        public ClampedFloatParameter Speed = new ClampedFloatParameter(0.8f, 0, 1);

        public ClampedFloatParameter Offset = new ClampedFloatParameter(1, 0, 13);

        public ClampedFloatParameter Alpha = new ClampedFloatParameter(1, 0, 1);


        public bool IsActive()
        {
            return active && Amount.value != 0;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
        
        [Serializable]
        public sealed class DirectionParameter : VolumeParameter<Direction> { public DirectionParameter(Direction value, bool overrideState = false) : base(value, overrideState) { } }

        [Serializable]
        public sealed class LineBlockFilerModeParameter : VolumeParameter<FilterMode> { public LineBlockFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

        [Serializable]
        public sealed class IntervalTypeParameter : VolumeParameter<IntervalType>{ public IntervalTypeParameter(IntervalType value, bool overrideState = false) : base(value, overrideState) { } }
    }
}