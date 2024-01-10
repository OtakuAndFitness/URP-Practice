using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/ColorAdjustment/WhiteBalance")]
    public class WhiteBalance : CustomPostProcessingBase
    {
        public ClampedFloatParameter temperature = new ClampedFloatParameter(0, -1, 1);
        public ClampedFloatParameter tint = new ClampedFloatParameter(0,-1,1);
        
        private const string _shaderName = "Custom/PostProcessing/ColorAdjustment/WhiteBalance";
        private RTHandle _tempRT0;
        // private RTHandle _tempRT1;
        private string _tempRT0Name => "_TemporaryRenderTexture0";
        // private string _tempRT1Name => "_TemporaryRenderTexture1";

        private int _temperatureKeyword = Shader.PropertyToID("_WhiteBalanceTemperature");
        private int _tintKeyword = Shader.PropertyToID("_WhiteBalanceTint");

        public override bool IsActive() =>  _material != null && temperature.value != 0;
        

        public override CustomPostProcessingInjectionPoint InjectionPoint =>
            CustomPostProcessingInjectionPoint.AfterPostProcess;
        public override int OrderInInjectionPoint => 24;
        
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
            cmd.SetGlobalFloat(_temperatureKeyword, temperature.value);
            cmd.SetGlobalFloat(_tintKeyword, tint.value);
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

