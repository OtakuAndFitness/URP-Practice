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


        // private const string _shaderName = "Custom/PostProcessing/Glitch/WaveJitter";
        // private RTHandle _tempRT0;
        // private RTHandle _tempRT1;
        // private string _tempRT0Name => "_TemporaryRenderTexture0";
        // private string _tempRT1Name => "_TemporaryRenderTexture1";

        // private int _paramsKeyword = Shader.PropertyToID("_WaveJitterParams");
        // private int _resolutionKeyword = Shader.PropertyToID("_WaveJitterResolution");

        // private float _TimeX = 1.0f;
        // private float _randomFrequency;
        // private int _width;
        // private int _height;
        public bool IsActive() =>  Amount.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }


        // public override CustomPostProcessingInjectionPoint InjectionPoint =>
        //     CustomPostProcessingInjectionPoint.AfterPostProcess;
        // public override int OrderInInjectionPoint => 40;
        //
        // public override void Setup()
        // {
        //     if (_material == null)
        //     {
        //         _material = CoreUtils.CreateEngineMaterial(_shaderName);
        //     }
        // }
        //
        // private void UpdateFrequencyWJ()
        // {
        //     if (IntervalType.value == PostProcessingExtends.IntervalType.Random)
        //     {
        //         _randomFrequency = UnityEngine.Random.Range(0, Frequency.value);
        //     }
        //
        //     if (IntervalType.value == PostProcessingExtends.IntervalType.Infinite)
        //     {
        //         SetKeyword("USING_FREQUENCY_INFINITE");
        //         // waveJitter.EnableKeyword("USING_FREQUENCY_INFINITE");
        //     }
        //     else
        //     {
        //         SetKeyword("USING_FREQUENCY_INFINITE", false);
        //         // waveJitter.DisableKeyword("USING_FREQUENCY_INFINITE");
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
        //     Draw(cmd, source, _tempRT0);
        //     
        //     UpdateFrequencyWJ();
        //     cmd.SetGlobalVector(_paramsKeyword, new Vector4(IntervalType.value == PostProcessingExtends.IntervalType.Random ? _randomFrequency : Frequency.value, RGBSplit.value , Speed.value, Amount.value));
        //     cmd.SetGlobalVector(_resolutionKeyword, CustomResolution.value ? Resolution.value : new Vector2(_width,_height));
        //     
        //     Draw(cmd, _tempRT0, destination, (int)JitterDirection.value);
        // }
        //
        // public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        // {
        //     var descriptor = GetCameraRenderTextureDescriptor(renderingData);
        //     _width = descriptor.width;
        //     _height = descriptor.height;
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
        // public sealed class WaveJitterFilerModeParameter : VolumeParameter<FilterMode> { public WaveJitterFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

        [Serializable]
        public sealed class IntervalTypeParameter : VolumeParameter<IntervalType>{ public IntervalTypeParameter(IntervalType value, bool overrideState = false) : base(value, overrideState) { } }
    }
}