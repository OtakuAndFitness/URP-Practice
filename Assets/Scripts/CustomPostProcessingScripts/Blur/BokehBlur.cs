using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Blur/BokehBlur")]
    public class BokehBlur : CustomPostProcessingBase
    {
        // public BokehFilerModeParameter filterMode = new BokehFilerModeParameter(FilterMode.Bilinear);
        public ClampedFloatParameter blurRadius = new ClampedFloatParameter(0.6f, 0.0f, 10.0f);
        public ClampedIntParameter iteration = new ClampedIntParameter(0, 0, 128);
        public ClampedFloatParameter downScaling = new ClampedFloatParameter(2f, 1f, 8f);
    
        private const string _shaderName = "Custom/PostProcessing/Blur/BokehBlur";

        private RTHandle _tempRT0;
        private string _tempRT0Name => "_TemporaryRenderTexture0";

        private int _blurSizeKeyword = Shader.PropertyToID("_BokehBlurSize");
        private int _iterationKeyword = Shader.PropertyToID("_BokehIteration");

        public override bool IsActive() => _material != null && iteration.value > 0;
        public override CustomPostProcessingInjectionPoint InjectionPoint =>
            CustomPostProcessingInjectionPoint.AfterPostProcess;
        public override int OrderInInjectionPoint => 1;

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

            // _width = descriptor.width;
            // _height = descriptor.height;
            
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
            
            cmd.SetGlobalFloat(_blurSizeKeyword, blurRadius.value);
            cmd.SetGlobalFloat(_iterationKeyword, iteration.value);
            // cmd.SetGlobalVector(CustomPostProcessingShaderConstants._GoldenRot, _GoldenRot);
            // cmd.SetGlobalVector(CustomPostProcessingShaderConstants._Offset, new Vector4(iteration.value, blurRadius.value, 1f / _width, 1f / _height));
            
            Draw(cmd, _tempRT0, destination, 0);
        }
        
        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            
            CoreUtils.Destroy(_material);
            
            _tempRT0?.Release();
        }

        // [Serializable]
        // public sealed class BokehFilerModeParameter : VolumeParameter<FilterMode> { public BokehFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

    }
}

