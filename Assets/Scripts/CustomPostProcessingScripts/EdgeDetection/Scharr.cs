using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/EdgeDetection/Scharr")]
    public class Scharr : CustomPostProcessingBase
    {
        // public ScharrFilterModeParameter FilterMode = new ScharrFilterModeParameter(UnityEngine.FilterMode.Bilinear);
        
        public ClampedFloatParameter edgeWidth = new ClampedFloatParameter(0, 0, 5);
        
        public ColorParameter edgeColor = new ColorParameter(Color.black, true, true, true);
        
        public ClampedFloatParameter backgroundFade = new ClampedFloatParameter(0, 0, 1);
        
        public ColorParameter backgroundColor = new ColorParameter(Color.white, true, true, true);

        private const string _shaderName = "Custom/PostProcessing/EdgeDetection/Scharr";
        private RTHandle _tempRT0;
        // private RTHandle _tempRT1;
        private string _tempRT0Name => "_TemporaryRenderTexture0";
        // private string _tempRT1Name => "_TemporaryRenderTexture1";

        private int _paramsKeyword = Shader.PropertyToID("_ScharrParams");
        private int _edgeColorKeyword = Shader.PropertyToID("_ScharrEdgeColor");
        private int _backgroundKeyword = Shader.PropertyToID("_ScharrBackgroundColor");

        public override bool IsActive() =>  _material != null && edgeWidth.value > 0;
        

        public override CustomPostProcessingInjectionPoint InjectionPoint =>
            CustomPostProcessingInjectionPoint.AfterPostProcess;
        public override int OrderInInjectionPoint => 27;
        
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
            cmd.SetGlobalVector(_paramsKeyword, new Vector2(edgeWidth.value, backgroundFade.value));
            cmd.SetGlobalColor(_edgeColorKeyword, edgeColor.value);
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
        // public sealed class ScharrFilterModeParameter : VolumeParameter<FilterMode> { public ScharrFilterModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

    }
}