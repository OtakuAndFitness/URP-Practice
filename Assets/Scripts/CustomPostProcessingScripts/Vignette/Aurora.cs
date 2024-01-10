using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Vignette/Aurora")]
    public class Aurora : CustomPostProcessingBase
    {
        public ClampedFloatParameter vignetteArea = new ClampedFloatParameter(0.8f, 0, 1);
        public ClampedFloatParameter vignetteSmothness = new ClampedFloatParameter(0.5f, 0, 1);
        public ClampedFloatParameter vignetteFading = new ClampedFloatParameter(0, 0, 1);
        public ClampedFloatParameter colorChange = new ClampedFloatParameter(0.1f, 0.1f, 1);
        public ClampedFloatParameter colorFactorR = new ClampedFloatParameter(1, 0, 2);
        public ClampedFloatParameter colorFactorG = new ClampedFloatParameter(1, 0, 2);
        public ClampedFloatParameter colorFactorB = new ClampedFloatParameter(1, 0, 2);
        public ClampedFloatParameter flowSpeed = new ClampedFloatParameter(1, 0, 2);


        private const string _shaderName = "Custom/PostProcessing/Vignette/Aurora";

        private RTHandle _tempRT0;
        private string _tempRT0Name => "_TemporaryRenderTexture0";

        private int _parametersKeyword = Shader.PropertyToID("_AuroraParameters");
        private int _parameters2Keyword = Shader.PropertyToID("_AuroraParameters2");

        public override bool IsActive() => _material != null && vignetteFading.value > 0;
        public override CustomPostProcessingInjectionPoint InjectionPoint =>
            CustomPostProcessingInjectionPoint.AfterPostProcess;
        public override int OrderInInjectionPoint => 55;
        
        private float _TimeX = 1.0f;


        public override void Setup()
        {
            if (_material == null)
            {
                _material = CoreUtils.CreateEngineMaterial(_shaderName);
            }
        }
        
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var descriptor = GetCameraRenderTextureDescriptor(renderingData);
            
            RenderingUtils.ReAllocateIfNeeded(ref _tempRT0, descriptor, name: _tempRT0Name,
                wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
            
        }
        
        public override void Render(CommandBuffer cmd, ref RenderingData renderingData, in RTHandle source, in RTHandle destination)
        {
            if (_material == null)
            {
                return;
            }
            
            Draw(cmd, source, _tempRT0);
            
            
            _TimeX += Time.deltaTime;
            if (_TimeX > 100)
            {
                _TimeX = 0;
            }
            cmd.SetGlobalVector(_parametersKeyword, new Vector4(vignetteArea.value, vignetteSmothness.value, colorChange.value, _TimeX * flowSpeed.value));
            cmd.SetGlobalVector(_parameters2Keyword, new Vector4(colorFactorR.value, colorFactorG.value,colorFactorB.value, vignetteFading.value));
            
            Draw(cmd, _tempRT0, destination, 0);
            
        }
        
        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            
            CoreUtils.Destroy(_material);
            
            _tempRT0?.Release();
        }
    }
}

