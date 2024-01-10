using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/ColorAdjustment/ColorReplaceV2")]
    public class ColorReplaceV2 : CustomPostProcessingBase
    {
        // public ColorParameter FromColor = new ColorParameter(new Color(0.8f, 0.0f, 0.0f, 1), true, true, true);
        // public ColorParameter ToColor = new ColorParameter(new Color(0.0f, 0.8f, 0.0f, 1), true, true, true);
        public GradientParameter FromGradientColor = new GradientParameter(null);
        public GradientParameter ToGradientColor = new GradientParameter(null);
        public ClampedFloatParameter gridentSpeed = new ClampedFloatParameter(0.5f, 0, 100);
        public ClampedFloatParameter Range = new ClampedFloatParameter(0, 0, 1);
        public ClampedFloatParameter Fuzziness = new ClampedFloatParameter(0.5f,0f,1f);
        
       private const string _shaderName = "Custom/PostProcessing/ColorAdjustment/ColorReplaceV2";
        private RTHandle _tempRT0;
        // private RTHandle _tempRT1;
        private string _tempRT0Name => "_TemporaryRenderTexture0";
        // private string _tempRT1Name => "_TemporaryRenderTexture1";

        private int _rangeKeyword = Shader.PropertyToID("_ColorReplaceV2Range");
        private int _fuzzinessKeyword = Shader.PropertyToID("_ColorReplaceV2Fuzziness");
        private int _fromColorKeyword = Shader.PropertyToID("_ColorReplaceV2FromColor");
        private int _toColorKeyword = Shader.PropertyToID("_ColorReplaceV2ToColor");

        public override bool IsActive() =>  _material != null && Range.value > 0;
        

        public override CustomPostProcessingInjectionPoint InjectionPoint =>
            CustomPostProcessingInjectionPoint.AfterPostProcess;
        public override int OrderInInjectionPoint => 14;

        private float _TimeX = 1.0f;
        
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
            
            cmd.SetGlobalFloat(_rangeKeyword, Range.value);
            cmd.SetGlobalFloat(_fuzzinessKeyword, Fuzziness.value);
            
            _TimeX += (Time.deltaTime * gridentSpeed.value);
            if (_TimeX > 100)
            {
                _TimeX = 0;
            }

            if (FromGradientColor.value != null)
            {
                cmd.SetGlobalColor(_fromColorKeyword, FromGradientColor.value.Evaluate(_TimeX * 0.01f));
            }

            if (ToGradientColor.value != null)
            {
                cmd.SetGlobalColor(_toColorKeyword, ToGradientColor.value.Evaluate(_TimeX * 0.01f));
            }
            
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
    // public sealed class ColorTypeParameter : VolumeParameter<ColorType>{
    //     public ColorTypeParameter(ColorType value, bool overrideState = false) : base(value, overrideState)
    //     {
    //         
    //     }
    // }
    
    [Serializable]
    public sealed class GradientParameter : VolumeParameter<Gradient>{
        public GradientParameter(Gradient value, bool overrideState = false) : base(value, overrideState)
        {
            
        }
    }
}

