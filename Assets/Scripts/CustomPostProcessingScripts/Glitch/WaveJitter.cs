using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Glitch/WaveJitter")]
    public class WaveJitter : VolumeComponent, IPostProcessComponent
    {
        // public WaveJitterFilerModeParameter FilterMode = new WaveJitterFilerModeParameter(UnityEngine.FilterMode.Bilinear);
        
        public DirectionParameter JitterDirection = new DirectionParameter(Direction.Horizontal);

        public IntervalTypeParameter IntervalType = new IntervalTypeParameter(PostProcessingExtends.IntervalType.Random);
        
        public ClampedFloatParameter Frequency = new ClampedFloatParameter(5, 0, 50);
        
        public ClampedFloatParameter RGBSplit = new ClampedFloatParameter(20, 0, 50);
        
        public ClampedFloatParameter Speed = new ClampedFloatParameter(0.25f, 0, 1);
        
        public ClampedFloatParameter Amount = new ClampedFloatParameter(0, 0, 2);

        public BoolParameter CustomResolution = new BoolParameter(false);

        public Vector2Parameter Resolution = new Vector2Parameter(new Vector2(640,480));
        
        public bool IsActive() =>  Amount.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }
        
        [Serializable]
        public sealed class DirectionParameter : VolumeParameter<Direction> { public DirectionParameter(Direction value, bool overrideState = false) : base(value, overrideState) { } }

        // [Serializable]
        // public sealed class WaveJitterFilerModeParameter : VolumeParameter<FilterMode> { public WaveJitterFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

        [Serializable]
        public sealed class IntervalTypeParameter : VolumeParameter<IntervalType>{ public IntervalTypeParameter(IntervalType value, bool overrideState = false) : base(value, overrideState) { } }
    }
}