using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Glitch/RGBSplit")]
    public class RGBSplit : CustomPostProcessingBase
    {
        public GlitchRGBSplitDirectionParameter SplitDirection = new GlitchRGBSplitDirectionParameter(DirectionEX.Horizontal);
        
        // public GlitchRGBSplitFilerModeParameter filterMode = new GlitchRGBSplitFilerModeParameter(FilterMode.Bilinear);

        public ClampedFloatParameter Fading = new ClampedFloatParameter(1, 0, 1);

        public ClampedFloatParameter Amount = new ClampedFloatParameter(0, 0, 5);

        public ClampedFloatParameter Speed = new ClampedFloatParameter(1, 0, 10);

        public ClampedFloatParameter CenterFading = new ClampedFloatParameter(1, 0, 1);

        public ClampedFloatParameter AmountR = new ClampedFloatParameter(1, 0, 5);

        public ClampedFloatParameter AmountB = new ClampedFloatParameter(1, 0, 5);
    
        private const string _shaderName = "Custom/PostProcessing/Glitch/RGBSplit";
        private RTHandle _tempRT0;
        // private RTHandle _tempRT1;
        private string _tempRT0Name => "_TemporaryRenderTexture0";
        // private string _tempRT1Name => "_TemporaryRenderTexture1";

        private int _paramsKeyword = Shader.PropertyToID("_RGBSplitParams");
        private int _params2Keyword = Shader.PropertyToID("_RGBSplitParams2");

        private float _TimeX = 1.0f;
        public override bool IsActive() =>  _material != null && Amount.value > 0;
        

        public override CustomPostProcessingInjectionPoint InjectionPoint =>
            CustomPostProcessingInjectionPoint.AfterPostProcess;
        public override int OrderInInjectionPoint => 35;
        
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
            cmd.SetGlobalVector(_paramsKeyword, new Vector4(Fading.value, Amount.value, Speed.value, CenterFading.value));
            cmd.SetGlobalVector(_params2Keyword, new Vector3(_TimeX, AmountR.value, AmountB.value));
            int pass = (int)SplitDirection.value;
            
            Draw(cmd, _tempRT0, destination, pass);
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
    // public sealed class GlitchRGBSplitFilerModeParameter : VolumeParameter<FilterMode> { public GlitchRGBSplitFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

    
    [Serializable]
    public sealed class GlitchRGBSplitDirectionParameter : VolumeParameter<DirectionEX> { public GlitchRGBSplitDirectionParameter(DirectionEX value, bool overrideState = false) : base(value, overrideState) { } }

}

