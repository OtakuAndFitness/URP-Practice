using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/ColorAdjustment/ColorReplace")]
    public class ColorReplace : VolumeComponent, IPostProcessComponent
    {
        public ColorParameter FromColor = new ColorParameter(new Color(0.8f, 0.0f, 0.0f, 1), true, true, true);
        public ColorParameter ToColor = new ColorParameter(new Color(0.0f, 0.8f, 0.0f, 1), true, true, true);
        // public GradientParameter FromGradientColor = new GradientParameter(null);
        // public GradientParameter ToGradientColor = new GradientParameter(null);
        // public ClampedFloatParameter gridentSpeed = new ClampedFloatParameter(0.5f, 0, 100);
        public ClampedFloatParameter Range = new ClampedFloatParameter(0, 0, 1);
        public ClampedFloatParameter Fuzziness = new ClampedFloatParameter(0.5f,0f,1f);
        
       // private const string _shaderName = "Custom/PostProcessing/ColorAdjustment/ColorReplace";
        // private RTHandle _tempRT0;
        // private RTHandle _tempRT1;
        // private string _tempRT0Name => "_TemporaryRenderTexture0";
        // private string _tempRT1Name => "_TemporaryRenderTexture1";

        // private int _rangeKeyword = Shader.PropertyToID("_ColorReplaceRange");
        // private int _fuzzinessKeyword = Shader.PropertyToID("_ColorReplaceFuzziness");
        // private int _fromColorKeyword = Shader.PropertyToID("_ColorReplaceFromColor");
        // private int _toColorKeyword = Shader.PropertyToID("_ColorReplaceToColor");

        public bool IsActive() =>  Range.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }


        // public override CustomPostProcessingInjectionPoint InjectionPoint =>
        //     CustomPostProcessingInjectionPoint.AfterPostProcess;
        // public override int OrderInInjectionPoint => 13;
        //
        // private float _TimeX = 1.0f;
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
        //     
        //     cmd.SetGlobalFloat(_rangeKeyword, Range.value);
        //     cmd.SetGlobalFloat(_fuzzinessKeyword, Fuzziness.value);
        //     cmd.SetGlobalColor(_fromColorKeyword, FromColor.value);
        //     cmd.SetGlobalColor(_toColorKeyword, ToColor.value);
        //     
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

