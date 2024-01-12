using System;
using System.Collections;
using System.Collections.Generic;
using PostProcessingExtends;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Blur/GrainyBlur")]
    public class GrainyBlur : VolumeComponent, IPostProcessComponent
    {
        // public GrainyFilerModeParameter filterMode = new GrainyFilerModeParameter(FilterMode.Bilinear);
        public ClampedFloatParameter blurRadius = new ClampedFloatParameter(0.6f, 0.0f, 30.0f);
        public ClampedFloatParameter downSample = new ClampedFloatParameter(2.0f, 1, 8);
        public ClampedIntParameter iteration = new ClampedIntParameter(0, 0, 8);
        
        // private const string _shaderName = "Custom/PostProcessing/Blur/GrainyBlur";
        // private RTHandle _tempRT0;
        // private string _tempRT0Name => "_TemporaryRenderTexture0";
       

        // private int _blurSizeKeyword = Shader.PropertyToID("_GrainyBlurSize");
        // private int _iterationKeyword = Shader.PropertyToID("_GrainyIteration");
        //
        // public override CustomPostProcessingInjectionPoint InjectionPoint =>
        //     CustomPostProcessingInjectionPoint.AfterPostProcess;
        // public override int OrderInInjectionPoint => 6;

        public bool IsActive() =>  iteration.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }


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
        //     cmd.SetGlobalFloat(_blurSizeKeyword, blurRadius.value);
        //     cmd.SetGlobalFloat(_iterationKeyword, iteration.value);
        //
        //     if (downSample.value > 1.0f)
        //     {
        //         Draw(cmd, source, _tempRT0);
        //         Draw(cmd, _tempRT0, destination, 0);
        //     }
        //     else
        //     {
        //         Draw(cmd, source, destination, 0);
        //
        //     }
        // }
        //
        // public override void Dispose(bool disposing)
        // {
        //     base.Dispose(disposing);
        //     
        //     CoreUtils.Destroy(_material);
        //     
        //     _tempRT0?.Release();
        // }
    }
    
    // [Serializable]
    // public sealed class GrainyFilerModeParameter : VolumeParameter<FilterMode> { public GrainyFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

}

