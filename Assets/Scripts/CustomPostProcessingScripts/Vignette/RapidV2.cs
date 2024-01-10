using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Vignette/RapidV2")]
    public class RapidV2 : CustomPostProcessingBase
    {
        public RapidOldTVTypeParameter vignetteType = new RapidOldTVTypeParameter(VignetteType.ClassicMode);
        public ClampedFloatParameter vignetteIndensity = new ClampedFloatParameter(0, 0, 5);
        public ClampedFloatParameter vignetteSharpness = new ClampedFloatParameter(0.1f, -1, 1);
        public Vector2Parameter vignetteCenter = new Vector2Parameter(new Vector2(0.5f,0.5f));
        public ColorParameter vignetteColor = new ColorParameter(new Color(0.1f, 0.8f, 1.0f) , true,true,true);
        

        private const string _shaderName = "Custom/PostProcessing/Vignette/RapidV2";

        private RTHandle _tempRT0;
        private string _tempRT0Name => "_TemporaryRenderTexture0";

        private int _parametersKeyword = Shader.PropertyToID("_RapidV2Parameters");
        private int _colorKeyword = Shader.PropertyToID("_RapidV2Color");

        public override bool IsActive() => _material != null && vignetteIndensity.value > 0;
        public override CustomPostProcessingInjectionPoint InjectionPoint =>
            CustomPostProcessingInjectionPoint.AfterPostProcess;
        public override int OrderInInjectionPoint => 59;
        

        public override void Setup()
        {
            if (_material == null)
            {
                _material = CoreUtils.CreateEngineMaterial(_shaderName);
            }
        }

        public override void Render(CommandBuffer cmd, ref RenderingData renderingData, in RTHandle source, in RTHandle destination)
        {
            if (_material == null)
            {
                return;
            }
            
            Draw(cmd, source, _tempRT0);
            
            cmd.SetGlobalVector(_parametersKeyword, new Vector4(vignetteIndensity.value, vignetteSharpness.value, vignetteCenter.value.x, vignetteCenter.value.y));
            if (vignetteType.value == VignetteType.ColorMode)
            {
                cmd.SetGlobalColor(_colorKeyword, vignetteColor.value);
            }
            
            Draw(cmd, _tempRT0, destination, (int)vignetteType.value);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var descriptor = GetCameraRenderTextureDescriptor(renderingData);
            
            RenderingUtils.ReAllocateIfNeeded(ref _tempRT0, descriptor, name: _tempRT0Name,
                wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
            
        }
    }
    
}

