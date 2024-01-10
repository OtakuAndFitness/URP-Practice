using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/ColorAdjustment/Saturation")]
    public class Saturation : CustomPostProcessingBase
    {
        public ClampedFloatParameter saturation = new ClampedFloatParameter(1, 0, 2);
        
        private const string _shaderName = "Custom/PostProcessing/ColorAdjustment/Saturation";
        private RTHandle _tempRT0;
        // private RTHandle _tempRT1;
        private string _tempRT0Name => "_TemporaryRenderTexture0";
        // private string _tempRT1Name => "_TemporaryRenderTexture1";

        private int _saturationKeyword = Shader.PropertyToID("_Saturation");
        
        public override bool IsActive() =>  _material != null && saturation.value != 1;
        

        public override CustomPostProcessingInjectionPoint InjectionPoint =>
            CustomPostProcessingInjectionPoint.AfterPostProcess;
        public override int OrderInInjectionPoint => 21;
        
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
            cmd.SetGlobalFloat(_saturationKeyword, saturation.value);
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
}

