using System;
using System.Collections;
using System.Collections.Generic;
using PostProcessingExtends;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Blur/DualKawaseBlur")]
    public class DualKawaseBlur : VolumeComponent, IPostProcessComponent
    {
        // private const int MaxIteration = 10;
        // public DualKawaseFilerModeParameter filterMode = new DualKawaseFilerModeParameter(FilterMode.Bilinear);
        public ClampedFloatParameter blurSize = new ClampedFloatParameter(0.6f, 0.0f, 3.0f);
        public ClampedFloatParameter downScaling = new ClampedFloatParameter(2f, 1f, 10f);
        public ClampedIntParameter iteration = new ClampedIntParameter(0, 1, 10);

        // private const string _shaderName = "Custom/PostProcessing/Blur/DualKawaseBlur";
        // private RTHandle[] _tempRT = new RTHandle[MaxIteration + 1];
        // private string _tempRTName => "_TemporaryRenderTexture0";

        // private int _blurSizeKeyword = Shader.PropertyToID("_DualKawaseBlurSize");
        public bool IsActive() =>  iteration.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }


        // public override CustomPostProcessingInjectionPoint InjectionPoint =>
        //     CustomPostProcessingInjectionPoint.AfterPostProcess;
        // public override int OrderInInjectionPoint => 4;
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
        //     RenderingUtils.ReAllocateIfNeeded(ref _tempRT[0], descriptor, name: _tempRTName + "0",
        //         wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
        //
        //     for (int i = 1; i <= iteration.value; i++)
        //     {
        //         descriptor.width = Math.Max(descriptor.width / 2, 1);
        //         descriptor.height = Math.Max(descriptor.height / 2, 1);
        //         
        //         RenderingUtils.ReAllocateIfNeeded(ref _tempRT[i], descriptor, name: _tempRTName + "i",
        //             wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
        //         
        //     }
        //     
        // }
        //
        // public override void Render(CommandBuffer cmd, ref RenderingData renderingData, in RTHandle source, in RTHandle destination)
        // {
        //     if (_material == null)
        //     {
        //         return;
        //     }
        //     
        //     Draw(cmd, source, _tempRT[0]);
        //     for (int i = 0; i < iteration.value; i++)
        //     {
        //         cmd.SetGlobalFloat(_blurSizeKeyword, 1.0f + i * blurSize.value);
        //         Draw(cmd, _tempRT[i], _tempRT[i+1],0);
        //     }
        //     for (int i = iteration.value; i > 1; i--)
        //     {
        //         cmd.SetGlobalFloat(_blurSizeKeyword, 1.0f + i * blurSize.value);
        //         Draw(cmd, _tempRT[i], _tempRT[i-1],1);
        //     }
        //     Draw(cmd, _tempRT[1], destination,1);
        // }
        //
        // public override void Dispose(bool disposing)
        // {
        //     base.Dispose(disposing);
        //     
        //     CoreUtils.Destroy(_material);
        //
        //     for (int i = 0; i < MaxIteration; i++)
        //     {
        //         _tempRT[i]?.Release();
        //     }
        // }
    }
    
    // [Serializable]
    // public sealed class DualKawaseFilerModeParameter : VolumeParameter<FilterMode> { public DualKawaseFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

}

