using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/EdgeDetection/SobelNeon")]
    public class SobelNeon : CustomPostProcessingBase
    {
        // public SobelNeonFilterModeParameter FilterMode = new SobelNeonFilterModeParameter(UnityEngine.FilterMode.Bilinear);
        
        public ClampedFloatParameter edgeWidth = new ClampedFloatParameter(0, 0, 5);
        
        public ClampedFloatParameter edgeNeonFade = new ClampedFloatParameter(1, 0.1f, 1);

        public ClampedFloatParameter backgroundFade = new ClampedFloatParameter(0,0,1);
        
        public ClampedFloatParameter brightness = new ClampedFloatParameter(1, 0.2f, 2);
        
        public ColorParameter backgroundColor = new ColorParameter(Color.white, true, true, true);


        private const string _shaderName = "Custom/PostProcessing/EdgeDetection/SobelNeon";
        private RTHandle _tempRT0;
        // private RTHandle _tempRT1;
        private string _tempRT0Name => "_TemporaryRenderTexture0";
        // private string _tempRT1Name => "_TemporaryRenderTexture1";

        private int _paramsKeyword = Shader.PropertyToID("_SobelNeonParams");
        private int _backgroundKeyword = Shader.PropertyToID("_SobelNeonBackgroundColor");

        public override bool IsActive() =>  _material != null && edgeWidth.value > 0;
        

        public override CustomPostProcessingInjectionPoint InjectionPoint =>
            CustomPostProcessingInjectionPoint.AfterPostProcess;
        public override int OrderInInjectionPoint => 30;
        
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
            cmd.SetGlobalVector(_paramsKeyword, new Vector4(edgeWidth.value, edgeNeonFade.value, brightness.value, backgroundFade.value));
            cmd.SetGlobalColor(_backgroundKeyword, backgroundColor.value);
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
        // public sealed class SobelNeonFilterModeParameter : VolumeParameter<FilterMode> { public SobelNeonFilterModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

    }
}