using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable, VolumeComponentMenu("Custom-Post-Processing/ColorAdjustment/ContrastV2")]
    public class ContrastV2 : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter contrast = new ClampedFloatParameter(0, 0, 5);
        public ClampedFloatParameter contrastFactorR = new ClampedFloatParameter(0,-1,1);
        public ClampedFloatParameter contrastFactorG = new ClampedFloatParameter(0,-1,1);
        public ClampedFloatParameter contrastFactorB = new ClampedFloatParameter(0,-1,1);

        // private const string _shaderName = "Custom/PostProcessing/ColorAdjustment/ContrastV2";
        // private RTHandle _tempRT0;
        // private RTHandle _tempRT1;
        // private string _tempRT0Name => "_TemporaryRenderTexture0";
        // private string _tempRT1Name => "_TemporaryRenderTexture1";

        // private int _contrastV2Keyword = Shader.PropertyToID("_ContrastV2");
        // private int _contrastV2ColorKeyword = Shader.PropertyToID("_ContrastV2FactorRGB");
        
        public bool IsActive() =>  contrast.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }


        // public override CustomPostProcessingInjectionPoint InjectionPoint =>
        //     CustomPostProcessingInjectionPoint.AfterPostProcess;
        // public override int OrderInInjectionPoint => 17;
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
        //     cmd.SetGlobalFloat(_contrastV2Keyword, contrast.value + 1);
        //     cmd.SetGlobalColor(_contrastV2ColorKeyword, new Color(contrastFactorR.value, contrastFactorG.value,contrastFactorB.value));
        //     Draw(cmd, _tempRT0, destination, 0);
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
    }
}

