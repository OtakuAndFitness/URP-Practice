using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Blur/IrisBlur")]
    public class IrisBlur : CustomPostProcessingBase
    {
        // public IrisFilerModeParameter filterMode = new IrisFilerModeParameter(FilterMode.Bilinear);
        public ClampedFloatParameter blurSize = new ClampedFloatParameter(1f, 0f, 3f);
        public ClampedIntParameter iteration = new ClampedIntParameter(0, 0, 128);
        public ClampedFloatParameter centerOffsetX = new ClampedFloatParameter (0f,-1f,1f);
        public ClampedFloatParameter centerOffsetY = new ClampedFloatParameter (0f,-1f,1f);
        public ClampedFloatParameter areaSize = new ClampedFloatParameter (8f,0f,50f);
        
        private const string _shaderName = "Custom/PostProcessing/Blur/IrisBlur";
        private RTHandle _tempRT0;
        private string _tempRT0Name => "_TemporaryRenderTexture0";

        private int _gradientKeyword = Shader.PropertyToID("_IrisGradient");
        private int parametersKeyword = Shader.PropertyToID("_IrisParameters");
        
        public override bool IsActive() =>  _material != null && iteration.value > 0;
        

        public override CustomPostProcessingInjectionPoint InjectionPoint =>
            CustomPostProcessingInjectionPoint.AfterPostProcess;
        public override int OrderInInjectionPoint => 7;

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
            cmd.SetGlobalVector(_gradientKeyword, new Vector3(centerOffsetX.value, centerOffsetY.value, areaSize.value * 0.1f));
            cmd.SetGlobalVector(parametersKeyword, new Vector2(iteration.value, blurSize.value));
            Draw(cmd, _tempRT0, destination, 0);
        }
        
        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            
            CoreUtils.Destroy(_material);
            
            _tempRT0?.Release();
        }
    }
    
    // [Serializable]
    // public sealed class IrisFilerModeParameter : VolumeParameter<FilterMode> { public IrisFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

}

