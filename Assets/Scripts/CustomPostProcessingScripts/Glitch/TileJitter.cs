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

        // private const string _shaderName = "Custom/PostProcessing/Glitch/TileJitter";
        // private RTHandle _tempRT0;
        // private RTHandle _tempRT1;
        // private string _tempRT0Name => "_TemporaryRenderTexture0";
        // private string _tempRT1Name => "_TemporaryRenderTexture1";

        // private int _paramsKeyword = Shader.PropertyToID("_TileJitterParams");

        // private float _randomFrequency;
        
        public bool IsActive() =>  Amount.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }


        // public override CustomPostProcessingInjectionPoint InjectionPoint =>
        //     CustomPostProcessingInjectionPoint.AfterPostProcess;
        // public override int OrderInInjectionPoint => 39;
        //
        // public override void Setup()
        // {
        //     if (_material == null)
        //     {
        //         _material = CoreUtils.CreateEngineMaterial(_shaderName);
        //     }
        // }
        //
        // private void UpdateFrequencyTJ()
        // {
        //     if (IntervalType.value == PostProcessingExtends.IntervalType.Random)
        //     {
        //         _randomFrequency = UnityEngine.Random.Range(0, Frequency.value);
        //     }
        //
        //     if (IntervalType.value == PostProcessingExtends.IntervalType.Infinite)
        //     {
        //         SetKeyword("USING_FREQUENCY_INFINITE");
        //         // tileJitter.EnableKeyword("USING_FREQUENCY_INFINITE");
        //     }
        //     else
        //     {
        //         SetKeyword("USING_FREQUENCY_INFINITE", false);
        //         // tileJitter.DisableKeyword("USING_FREQUENCY_INFINITE");
        //     }
        // }
        //
        // public override void Render(CommandBuffer cmd, ref RenderingData renderingData, in RTHandle source, in RTHandle destination)
        // {
        //     if (_material == null)
        //     {
        //         return;
        //     }
        //     
        //     UpdateFrequencyTJ();
        //     if (JitterDirection.value == Direction.Horizontal)
        //     {
        //         SetKeyword("JITTER_DIRECTION_HORIZONTAL");
        //         // tileJitter.EnableKeyword("JITTER_DIRECTION_HORIZONTAL");
        //     }
        //     else
        //     {
        //         SetKeyword("JITTER_DIRECTION_HORIZONTAL", false);
        //         // tileJitter.DisableKeyword("JITTER_DIRECTION_HORIZONTAL");
        //     }
        //     Draw(cmd, source, _tempRT0);
        //     cmd.SetGlobalVector(_paramsKeyword, new Vector4(SplittingNumber.value, Amount.value , Speed.value * 100f, IntervalType.value == PostProcessingExtends.IntervalType.Random ? _randomFrequency : Frequency.value));
        //     
        //     Draw(cmd, _tempRT0, destination, SplittingDirection.value == Direction.Horizontal ? 0 : 1);
        // }
        //
        // public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        // {
        //     var descriptor = GetCameraRenderTextureDescriptor(renderingData);
        //     // descriptor.width = (int)(descriptor.width / downScaling.value);
        //     // descriptor.height = (int)(descriptor.height / downScaling.value);
        //
        //     RenderingUtils.ReAllocateIfNeeded(ref _tempRT0, descriptor, name: _tempRT0Name,
        //         wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
        //     // RenderingUtils.ReAllocateIfNeeded(ref _tempRT1, descriptor, name: _tempRT1Name,
        //     //     wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
        // }
        //
        // public override void Dispose(bool disposing)
        // {
        //     base.Dispose(disposing);
        //     
        //     CoreUtils.Destroy(_material);
        //     
        //     _tempRT0?.Release();
        //     // _tempRT1?.Release();
        // }
        
        [Serializable]
        public sealed class DirectionParameter : VolumeParameter<Direction> { public DirectionParameter(Direction value, bool overrideState = false) : base(value, overrideState) { } }

        // [Serializable]
        // public sealed class TileJitterFilerModeParameter : VolumeParameter<FilterMode> { public TileJitterFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

        [Serializable]
        public sealed class IntervalTypeParameter : VolumeParameter<IntervalType>{ public IntervalTypeParameter(IntervalType value, bool overrideState = false) : base(value, overrideState) { } }
    }
}