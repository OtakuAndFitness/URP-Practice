using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Glitch/TileJitter")]
    public class TileJitter : VolumeComponent, IPostProcessComponent
    {
        // public TileJitterFilerModeParameter FilterMode = new TileJitterFilerModeParameter(UnityEngine.FilterMode.Bilinear);

        public DirectionParameter JitterDirection = new DirectionParameter(Direction.Horizontal);

        public IntervalTypeParameter IntervalType = new IntervalTypeParameter(PostProcessingExtends.IntervalType.Random);
        
        public ClampedFloatParameter Frequency = new ClampedFloatParameter(1, 0, 25);
        
        public DirectionParameter SplittingDirection = new DirectionParameter(Direction.Vertical);
        
        public ClampedFloatParameter SplittingNumber = new ClampedFloatParameter(5, 0, 50);
        
        public ClampedFloatParameter Amount = new ClampedFloatParameter(0, 0, 100);

        public ClampedFloatParameter Speed = new ClampedFloatParameter(0.35f, 0, 1);
        
        public bool IsActive() =>  Amount.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }
        
        [Serializable]
        public sealed class DirectionParameter : VolumeParameter<Direction> { public DirectionParameter(Direction value, bool overrideState = false) : base(value, overrideState) { } }

        // [Serializable]
        // public sealed class TileJitterFilerModeParameter : VolumeParameter<FilterMode> { public TileJitterFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

        [Serializable]
        public sealed class IntervalTypeParameter : VolumeParameter<IntervalType>{ public IntervalTypeParameter(IntervalType value, bool overrideState = false) : base(value, overrideState) { } }
    }
}