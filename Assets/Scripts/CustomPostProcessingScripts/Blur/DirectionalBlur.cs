using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{    
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Blur/DirectionalBlur")]
    public class DirectionalBlur : VolumeComponent, IPostProcessComponent
    {
        // public DirectionalFilerModeParameter filterMode = new DirectionalFilerModeParameter(FilterMode.Bilinear);
        public ClampedFloatParameter angle = new ClampedFloatParameter(0f, 0f, 6.0f);
        public ClampedFloatParameter blurSize = new ClampedFloatParameter(0.6f, 0.0f, 50.0f);
        public ClampedFloatParameter downScaling = new ClampedFloatParameter(1f, 1f, 10f);
        public ClampedIntParameter iteration = new ClampedIntParameter(0, 0, 30);
        
        // private const string _shaderName = "Custom/PostProcessing/Blur/DirectionalBlur";

        // private RTHandle _tempRT0;
        // private string _tempRT0Name => "_TemporaryRenderTexture0";

        // private int blurSizeKeyword = Shader.PropertyToID("_DirectionalBlurSize");
        
        public bool IsActive() => iteration.value > 0;

        public bool IsTileCompatible()
        {
            return false;
        }
        // public override CustomPostProcessingInjectionPoint InjectionPoint =>
        //     CustomPostProcessingInjectionPoint.AfterPostProcess;
        // public override int OrderInInjectionPoint => 3;
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
        //     descriptor.width = (int)(descriptor.width / downScaling.value);
        //     descriptor.height = (int)(descriptor.height / downScaling.value);
        //
        //     
        //     RenderingUtils.ReAllocateIfNeeded(ref _tempRT0, descriptor, name: _tempRT0Name,
        //         wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
        //     
        // }
        
        // public override void Render(CommandBuffer cmd, ref RenderingData renderingData, in RTHandle source, in RTHandle destination)
        // {
        //     if (_material == null)
        //     {
        //         return;
        //     }
        //     
        //     float sinVal = (Mathf.Sin(angle.value) * blurSize.value * 0.05f) / iteration.value;
        //     float cosVal = (Mathf.Cos(angle.value) * blurSize.value * 0.05f) / iteration.value;
        //     cmd.SetGlobalVector(blurSizeKeyword, new Vector3(iteration.value, sinVal, cosVal));
        //
        //     if (downScaling.value > 1.0f)
        //     {
        //         Draw(cmd, source, _tempRT0);
        //         Draw(cmd, _tempRT0, destination, 0);
        //
        //     }
        //     else
        //     {
        //         Draw(cmd, source, destination,0);
        //     }
        //     
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
    
    // [Serializable]
    // public sealed class DirectionalFilerModeParameter : VolumeParameter<FilterMode> { public DirectionalFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }


}
