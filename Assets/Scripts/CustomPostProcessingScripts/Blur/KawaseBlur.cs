using System;
using System.Collections;
using System.Collections.Generic;
using PostProcessingExtends;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UPostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Blur/KawaseBlur")]
    public class KawaseBlur : VolumeComponent, IPostProcessComponent
    {
        // public KawaseFilerModeParameter filterMode = new KawaseFilerModeParameter(FilterMode.Bilinear);
        public ClampedFloatParameter blurSize= new ClampedFloatParameter(0.6f, 0.0f, 3.0f);
        public ClampedFloatParameter downSample = new ClampedFloatParameter(2f, 1f, 8f);
        public ClampedIntParameter iteration = new ClampedIntParameter(0, 0, 20);

        // private const string _shaderName = "Custom/PostProcessing/Blur/KawaseBlur";
        // private RTHandle _tempRT0;
        // private RTHandle _tempRT1;
        // private string _tempRT0Name => "_TemporaryRenderTexture0";
        // private string _tempRT1Name => "_TemporaryRenderTexture1";
        //
        // private int _blurSizeKeyword = Shader.PropertyToID("_KawaseBlurSize");
        
        public bool IsActive() =>  iteration.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }


        // public override CustomPostProcessingInjectionPoint InjectionPoint =>
        //     CustomPostProcessingInjectionPoint.AfterPostProcess;
        // public override int OrderInInjectionPoint => 8;
        //
        // public override void Setup()
        // {
        //     if (_material == null)
        //     {
        //         _material = CoreUtils.CreateEngineMaterial(_shaderName);
        //     }
        // }
        //
        // public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        // {
        //     var descriptor = GetCameraRenderTextureDescriptor(renderingData);
        //     descriptor.width = (int)(descriptor.width / downSample.value);
        //     descriptor.height = (int)(descriptor.height / downSample.value);
        //
        //     RenderingUtils.ReAllocateIfNeeded(ref _tempRT0, descriptor, name: _tempRT0Name,
        //         wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
        //     RenderingUtils.ReAllocateIfNeeded(ref _tempRT1, descriptor, name: _tempRT1Name,
        //         wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
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
        //     for (int i = 0; i < iteration.value; i++)
        //     {
        //         cmd.SetGlobalFloat(_blurSizeKeyword, 1.0f + i * blurSize.value);
        //         Draw(cmd, _tempRT0, _tempRT1, 0);
        //         CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
        //     }
        //     Draw(cmd, _tempRT0, destination);
        // }
        //
        // public override void Dispose(bool disposing)
        // {
        //     base.Dispose(disposing);
        //     
        //     CoreUtils.Destroy(_material);
        //     
        //     _tempRT0?.Release();
        //     _tempRT1?.Release();
        // }
    }
    
    // [Serializable]
    // public sealed class KawaseFilerModeParameter : VolumeParameter<FilterMode> { public KawaseFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

}

