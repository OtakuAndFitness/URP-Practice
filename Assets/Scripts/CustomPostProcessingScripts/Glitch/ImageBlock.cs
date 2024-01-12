using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Glitch/ImageBlock")]
    public class ImageBlock : VolumeComponent, IPostProcessComponent
    {
        // public GlitchImageBlockFilerModeParameter filterMode = new GlitchImageBlockFilerModeParameter(FilterMode.Bilinear);

        public ClampedFloatParameter Fade = new ClampedFloatParameter(1, 0, 1);

        public ClampedFloatParameter Speed = new ClampedFloatParameter(0.5f, 0, 1);

        public ClampedFloatParameter Amount = new ClampedFloatParameter(0, 0, 10);

        public ClampedFloatParameter BlockLayer1_U = new ClampedFloatParameter(9, 0, 50);

        public ClampedFloatParameter BlockLayer1_V = new ClampedFloatParameter(9, 0, 50);

        public ClampedFloatParameter BlockLayer2_U = new ClampedFloatParameter(5, 0, 50);

        public ClampedFloatParameter BlockLayer2_V = new ClampedFloatParameter(5, 0, 50);

        public ClampedFloatParameter BlockLayer1_Indensity = new ClampedFloatParameter(8, 0, 50);

        public ClampedFloatParameter BlockLayer2_Indensity = new ClampedFloatParameter(4, 0, 50);

        public ClampedFloatParameter RGBSplitIndensity = new ClampedFloatParameter(0.5f, 0, 50);
        
        
        // private const string _shaderName = "Custom/PostProcessing/Glitch/ImageBlock";
        // private RTHandle _tempRT0;
        // private RTHandle _tempRT1;
        // private string _tempRT0Name => "_TemporaryRenderTexture0";
        // private string _tempRT1Name => "_TemporaryRenderTexture1";

        // private int _paramsKeyword = Shader.PropertyToID("_ImageBlockParams");
        // private int _params2Keyword = Shader.PropertyToID("_ImageBlockParams2");
        // private int _params3Keyword = Shader.PropertyToID("_ImageBlockParams3");

        // private float _TimeX = 1.0f;
        public bool IsActive() =>  Amount.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }


        // public override CustomPostProcessingInjectionPoint InjectionPoint =>
        //     CustomPostProcessingInjectionPoint.AfterPostProcess;
        // public override int OrderInInjectionPoint => 33;
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
        //     _TimeX += Time.deltaTime;
        //     if (_TimeX > 100)
        //     {
        //         _TimeX = 0;
        //     }
        //     Draw(cmd, source, _tempRT0);
        //
        //     cmd.SetGlobalVector(_paramsKeyword, new Vector3(_TimeX * Speed.value, Amount.value, Fade.value));
        //     cmd.SetGlobalVector(_params2Keyword, new Vector4(BlockLayer1_U.value, BlockLayer1_V.value, BlockLayer2_U.value, BlockLayer2_V.value));
        //     cmd.SetGlobalVector(_params3Keyword, new Vector3(RGBSplitIndensity.value, BlockLayer1_Indensity.value, BlockLayer2_Indensity.value));
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
        
        // [Serializable]
        // public sealed class GlitchImageBlockFilerModeParameter : VolumeParameter<FilterMode> { public GlitchImageBlockFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

    }
}

