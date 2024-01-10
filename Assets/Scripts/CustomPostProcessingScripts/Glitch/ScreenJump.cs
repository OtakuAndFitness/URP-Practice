using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Glitch/ScreenJump")]
    public class ScreenJump : CustomPostProcessingBase
    {
        // public ScreenJumpFilerModeParameter FilterMode = new ScreenJumpFilerModeParameter(UnityEngine.FilterMode.Bilinear);

        public ScreenJumpDirectionParameter ScreenJumpDirection = new ScreenJumpDirectionParameter(Direction.Horizontal);

        public BoolParameter isHorizontalReverse = new BoolParameter(true);
        
        public ClampedFloatParameter ScreenJumpIndensity = new ClampedFloatParameter(0, 0, 1);
        
        
         private const string _shaderName = "Custom/PostProcessing/Glitch/ScreenJump";
        private RTHandle _tempRT0;
        // private RTHandle _tempRT1;
        private string _tempRT0Name => "_TemporaryRenderTexture0";
        // private string _tempRT1Name => "_TemporaryRenderTexture1";

        private int _paramsKeyword = Shader.PropertyToID("_ScanLineJitterParams");
        private float _screenJumpTime;

        public override bool IsActive() =>  _material != null && ScreenJumpIndensity.value > 0;
        

        public override CustomPostProcessingInjectionPoint InjectionPoint =>
            CustomPostProcessingInjectionPoint.AfterPostProcess;
        public override int OrderInInjectionPoint => 37;
        
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
            
            _screenJumpTime += Time.deltaTime * ScreenJumpIndensity.value * 9.8f;
            cmd.SetGlobalVector(_paramsKeyword, new Vector2(ScreenJumpIndensity.value, isHorizontalReverse.value ? -_screenJumpTime : _screenJumpTime));
            
            Draw(cmd, _tempRT0, destination, (int)ScreenJumpDirection.value);
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
        
        [Serializable]
        public sealed class ScreenJumpDirectionParameter : VolumeParameter<Direction> { public ScreenJumpDirectionParameter(Direction value, bool overrideState = false) : base(value, overrideState) { } }

        // [Serializable]
        // public sealed class ScreenJumpFilerModeParameter : VolumeParameter<FilterMode> { public ScreenJumpFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

    }
}