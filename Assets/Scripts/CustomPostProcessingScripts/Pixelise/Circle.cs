using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable, VolumeComponentMenu("Custom-Post-Processing/Pixelise/Circle")]
    public class Circle : CustomPostProcessingBase
    {
        public ClampedFloatParameter pixelSize = new ClampedFloatParameter(0, 0, 1);
        public ClampedFloatParameter circleRadius = new ClampedFloatParameter(0.45f, 0.01f, 1);
        public ClampedFloatParameter pixelIntervalX = new ClampedFloatParameter(1, 0.2f, 5);
        public ClampedFloatParameter pixelIntervalY = new ClampedFloatParameter(1, 0.2f, 5);
        public ColorParameter backgroundColor = new ColorParameter(Color.black, true, true, true);

        
        private const string _shaderName = "Custom/PostProcessing/Pixelise/Circle";
        private RTHandle _tempRT0;
        // private RTHandle _tempRT1;
        private string _tempRT0Name => "_TemporaryRenderTexture0";
        // private string _tempRT1Name => "_TemporaryRenderTexture1";

        private int _paramsKeyword = Shader.PropertyToID("_CircleParams");
        private int _params2Keyword = Shader.PropertyToID("_CircleParams2");
        private int _backgroundKeyword = Shader.PropertyToID("_CircleBackground");

        private int _width;
        private int _height;

        public override bool IsActive() =>  _material != null && pixelSize.value > 0;
        

        public override CustomPostProcessingInjectionPoint InjectionPoint =>
            CustomPostProcessingInjectionPoint.AfterPostProcess;
        public override int OrderInInjectionPoint => 41;
        
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
            
           
            float size = (1.01f - pixelSize.value) * 300f;
            Vector4 parameters = new Vector4(size, ((_width * 2 / (float)_height) * size / Mathf.Sqrt(3f)), circleRadius.value, 0f);
            cmd.SetGlobalVector(_paramsKeyword, parameters);
            cmd.SetGlobalVector(_params2Keyword, new Vector2(pixelIntervalX.value, pixelIntervalY.value));
            cmd.SetGlobalColor(_backgroundKeyword, backgroundColor.value);
            
            Draw(cmd, _tempRT0, destination, 0);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var descriptor = GetCameraRenderTextureDescriptor(renderingData);
            _width = descriptor.width;
            _height = descriptor.height;
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

