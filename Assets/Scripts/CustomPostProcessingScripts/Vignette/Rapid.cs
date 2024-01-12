using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Vignette/Rapid")]
    public class Rapid : VolumeComponent, IPostProcessComponent
    {
        public RapidOldTVTypeParameter vignetteType = new RapidOldTVTypeParameter(VignetteType.ClassicMode);
        public ClampedFloatParameter vignetteIndensity = new ClampedFloatParameter(0, 0, 5);
        public Vector2Parameter vignetteCenter = new Vector2Parameter(new Vector2(0.5f,0.5f));
        public ColorParameter vignetteColor = new ColorParameter(new Color(0.1f, 0.8f, 1.0f) , true,true,true);
        

        // private const string _shaderName = "Custom/PostProcessing/Vignette/Rapid";

        // private RTHandle _tempRT0;
        // private string _tempRT0Name => "_TemporaryRenderTexture0";

        // private int _parametersKeyword = Shader.PropertyToID("_RapidParameters");
        // private int _colorKeyword = Shader.PropertyToID("_RapidColor");

        public bool IsActive() => vignetteIndensity.value > 0;

        public bool IsTileCompatible()
        {
            return false;
        }
        // public override CustomPostProcessingInjectionPoint InjectionPoint =>
        //     CustomPostProcessingInjectionPoint.AfterPostProcess;
        // public override int OrderInInjectionPoint => 56;
        //
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
        //     cmd.SetGlobalVector(_parametersKeyword, new Vector3(vignetteIndensity.value, vignetteCenter.value.x, vignetteCenter.value.y));
        //     if (vignetteType.value == VignetteType.ColorMode)
        //     {
        //         cmd.SetGlobalColor(_colorKeyword, vignetteColor.value);
        //     }
        //     
        //     Draw(cmd, _tempRT0, destination, (int)vignetteType.value);
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
    }
    
}

