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
        
        
        // private const string _shaderName = "Custom/PostProcessing/Glitch/ScreenShake";
        // private RTHandle _tempRT0;
        // private RTHandle _tempRT1;
        // private string _tempRT0Name => "_TemporaryRenderTexture0";
        // private string _tempRT1Name => "_TemporaryRenderTexture1";

        // private int _paramsKeyword = Shader.PropertyToID("_ScreenShakeParams");

        public bool IsActive() =>  ScreenShakeIndensity.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }


        // public override CustomPostProcessingInjectionPoint InjectionPoint =>
        //     CustomPostProcessingInjectionPoint.AfterPostProcess;
        // public override int OrderInInjectionPoint => 38;
        //
        // public override void Setup()
        // {
        //     if (_material == null)
        //     {
        //         _material = CoreUtils.CreateEngineMaterial(_shaderName);
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
        //     
        //     Draw(cmd, source, _tempRT0);
        //     
        //     cmd.SetGlobalFloat(_paramsKeyword, ScreenShakeIndensity.value);
        //     
        //     Draw(cmd, _tempRT0, destination, (int)ScreenShakeDirection.value);
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
        public sealed class ScreenShakeDirectionParameter : VolumeParameter<Direction> { public ScreenShakeDirectionParameter(Direction value, bool overrideState = false) : base(value, overrideState) { } }

        // [Serializable]
        // public sealed class ScreenShakeFilerModeParameter : VolumeParameter<FilterMode> { public ScreenShakeFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

    }
}