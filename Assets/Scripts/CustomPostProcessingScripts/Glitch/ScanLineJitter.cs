using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Glitch/ScanLineJitter")]
    public class ScanLineJitter : VolumeComponent, IPostProcessComponent
    {
        // public ScanLineJitterFilerModeParameter FilterMode = new ScanLineJitterFilerModeParameter(UnityEngine.FilterMode.Bilinear);

        public DirectionParameter JitterDirection = new DirectionParameter(Direction.Horizontal);

        public IntervalTypeParameter IntervalType = new IntervalTypeParameter(PostProcessingExtends.IntervalType.Random);
        
        public ClampedFloatParameter Frequency = new ClampedFloatParameter(1, 0, 25);
        
        public ClampedFloatParameter JitterIndensity = new ClampedFloatParameter(0, 0, 1);
        
        
        public bool IsActive() =>  JitterIndensity.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }
        
        [Serializable]
        public sealed class DirectionParameter : VolumeParameter<Direction> { public DirectionParameter(Direction value, bool overrideState = false) : base(value, overrideState) { } }

        // [Serializable]
        // public sealed class ScanLineJitterFilerModeParameter : VolumeParameter<FilterMode> { public ScanLineJitterFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

        [Serializable]
        public sealed class IntervalTypeParameter : VolumeParameter<IntervalType>{ public IntervalTypeParameter(IntervalType value, bool overrideState = false) : base(value, overrideState) { } }
    }
}