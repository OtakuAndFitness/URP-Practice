using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Glitch/AnalogNoise")]
    public class AnalogNoise : CustomPostProcessingBase
    {
        // public AnalogNoiseModeParameter FilterMode = new AnalogNoiseModeParameter(UnityEngine.FilterMode.Bilinear);
        
        public ClampedFloatParameter NoiseSpeed = new ClampedFloatParameter(0.5f, 0, 1);
        
        public ClampedFloatParameter NoiseFading = new ClampedFloatParameter(0, 0, 1);
        
        public ClampedFloatParameter LuminanceJitterThreshold = new ClampedFloatParameter(0.8f, 0, 1);


        private const string _shaderName = "Custom/PostProcessing/Glitch/AnalogNoise";
        private RTHandle _tempRT0;
        // private RTHandle _tempRT1;
        private string _tempRT0Name => "_TemporaryRenderTexture0";
        // private string _tempRT1Name => "_TemporaryRenderTexture1";

        private int _paramsKeyword = Shader.PropertyToID("_AnalogNoiseParams");

        private float _TimeX = 1.0f;
        public override bool IsActive() =>  _material != null && NoiseFading.value > 0;
        

        public override CustomPostProcessingInjectionPoint InjectionPoint =>
            CustomPostProcessingInjectionPoint.AfterPostProcess;
        public override int OrderInInjectionPoint => 31;
        
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
            
            _TimeX += Time.deltaTime;
            if (_TimeX > 100)
            {
                _TimeX = 0;
            }
            Draw(cmd, source, _tempRT0);
            
            cmd.SetGlobalVector(_paramsKeyword, new Vector4(NoiseSpeed.value, NoiseFading.value, LuminanceJitterThreshold.value, _TimeX));
            
            Draw(cmd, _tempRT0, destination, 0);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var descriptor = GetCameraRenderTextureDescriptor(renderingData);
            // descriptor.width = (int)(descriptor.width / downScaling.value);
            // descriptor.height = (int)(descriptor.height / downScaling.value);

            RenderingUtils.ReAllocateIfNeeded(ref _tempRT0, descriptor, name: _tempRT0Name,
                wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
            // RenderingUtils.ReAllocateIfNeeded(ref _tempRT1, descriptor, name: _tempRT1Name,
            //     wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
        }
        
        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            
            CoreUtils.Destroy(_material);
            
            _tempRT0?.Release();
            // _tempRT1?.Release();
        }
        

        // [Serializable]
        // public sealed class AnalogNoiseModeParameter : VolumeParameter<FilterMode> { public AnalogNoiseModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

    }
}