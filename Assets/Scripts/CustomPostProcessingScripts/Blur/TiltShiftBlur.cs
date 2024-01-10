using System;
using System.Collections;
using System.Collections.Generic;
using PostProcessingExtends;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Blur/TiltShiftBlur")]
    public class TiltShiftBlur : CustomPostProcessingBase
    {
        // public TiltShiftFilerModeParameter filterMode = new TiltShiftFilerModeParameter(FilterMode.Bilinear);
        public ClampedFloatParameter blurSize = new ClampedFloatParameter(1f, 0f, 3f);
        public ClampedIntParameter iteration = new ClampedIntParameter(0, 0, 128);
        public ClampedFloatParameter centerOffset = new ClampedFloatParameter (0f,-1f,1f);
        public ClampedFloatParameter areaSize = new ClampedFloatParameter (0f,0f,20f);
        public ClampedFloatParameter areaSmooth = new ClampedFloatParameter(1.2f,1f,20f);
        
        private const string _shaderName = "Custom/PostProcessing/Blur/TiltShiftBlur";
        private RTHandle _tempRT0;
        // private RTHandle _tempRT1;
        private string _tempRT0Name => "_TemporaryRenderTexture0";
        // private string _tempRT1Name => "_TemporaryRenderTexture1";

        private int _gradientKeyword = Shader.PropertyToID("_TiltShiftBlurGradient");
        private int _parametersKeyword = Shader.PropertyToID("_TiltShiftBlurParameters");
        
        public override bool IsActive() =>  _material != null && iteration.value > 0;
        

        public override CustomPostProcessingInjectionPoint InjectionPoint =>
            CustomPostProcessingInjectionPoint.AfterPostProcess;
        public override int OrderInInjectionPoint => 10;
        
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
            cmd.SetGlobalVector(_gradientKeyword, new Vector3(centerOffset.value, areaSize.value, areaSmooth.value));
            cmd.SetGlobalVector(_parametersKeyword, new Vector2(iteration.value, blurSize.value));
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
        
    }
    
    // [Serializable]
    // public sealed class TiltShiftFilerModeParameter : VolumeParameter<FilterMode> { public TiltShiftFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

}

