using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Sharpen/V2")]
    public class SharpenV2 : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter Sharpness = new ClampedFloatParameter(0, 0,5);

        // private const string _shaderName = "Custom/PostProcessing/Sharpen/SharpenV2";

        // private RTHandle _tempRT0;
        // private string _tempRT0Name => "_TemporaryRenderTexture0";

        // private int _parametersKeyword = Shader.PropertyToID("_SharpenV2");

        public bool IsActive() => Sharpness.value > 0;

        public bool IsTileCompatible()
        {
            return false;
        }
        // public override CustomPostProcessingInjectionPoint InjectionPoint =>
        //     CustomPostProcessingInjectionPoint.AfterPostProcess;
        // public override int OrderInInjectionPoint => 53;
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
        //     Draw(cmd, source, _tempRT0);
        //     cmd.SetGlobalFloat(_parametersKeyword, Sharpness.value);
        //     Draw(cmd, _tempRT0, destination, 0);
        // }
        //
        // public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        // {
        //     var descriptor = GetCameraRenderTextureDescriptor(renderingData);
        //     
        //     RenderingUtils.ReAllocateIfNeeded(ref _tempRT0, descriptor, name: _tempRT0Name,
        //         wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
        //     
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

}
