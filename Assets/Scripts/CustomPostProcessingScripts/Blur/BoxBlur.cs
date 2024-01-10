using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Blur/BoxBlur")]
    public class BoxBlur : CustomPostProcessingBase
    {
        // public BoxFilerModeParameter filterMode = new BoxFilerModeParameter(FilterMode.Bilinear);
        public ClampedFloatParameter blurRadius = new ClampedFloatParameter(0.6f, 0.0f, 3.0f);
        public ClampedIntParameter iteration = new ClampedIntParameter(0, 0, 20);
        public ClampedFloatParameter downScaling = new ClampedFloatParameter(2f, 1f, 8f);
        
        private const string _shaderName = "Custom/PostProcessing/Blur/BoxBlur";
        private RTHandle _tempRT0;
        private RTHandle _tempRT1;
        private string _tempRT0Name => "_TemporaryRenderTexture0";
        private string _tempRT1Name => "_TemporaryRenderTexture1";

        private int _blurSizeKeyword = Shader.PropertyToID("_BoxBlurSize");
        
        public override bool IsActive() =>  _material != null && iteration.value > 0;
        

        public override CustomPostProcessingInjectionPoint InjectionPoint =>
            CustomPostProcessingInjectionPoint.AfterPostProcess;
        public override int OrderInInjectionPoint => 2;

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
            descriptor.width = (int)(descriptor.width / downScaling.value);
            descriptor.height = (int)(descriptor.height / downScaling.value);

            RenderingUtils.ReAllocateIfNeeded(ref _tempRT0, descriptor, name: _tempRT0Name,
                wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
            RenderingUtils.ReAllocateIfNeeded(ref _tempRT1, descriptor, name: _tempRT1Name,
                wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
        }

        public override void Render(CommandBuffer cmd, ref RenderingData renderingData, in RTHandle source, in RTHandle destination)
        {
            if (_material == null)
            {
                return;
            }
            
            Draw(cmd, source, _tempRT0);

            for (int i = 0; i < iteration.value; i++) {
                cmd.SetGlobalVector(_blurSizeKeyword, new Vector4(1.0f + blurRadius.value, 0, 0, 0));
                Draw(cmd, _tempRT0, _tempRT1, 0);
                cmd.SetGlobalVector(_blurSizeKeyword, new Vector4(0, 1.0f + blurRadius.value, 0, 0));
                Draw(cmd, _tempRT1, _tempRT0, 0);
            }
            
            Draw(cmd, _tempRT0, destination);
            
        }
        
        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            
            CoreUtils.Destroy(_material);
            
            _tempRT0?.Release();
            _tempRT1?.Release();
        }
    }
    
    // [Serializable]
    // public sealed class BoxFilerModeParameter : VolumeParameter<FilterMode> { public BoxFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }
}

