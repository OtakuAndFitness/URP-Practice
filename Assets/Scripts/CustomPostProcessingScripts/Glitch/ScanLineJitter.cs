using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Glitch/ScanLineJitter")]
    public class ScanLineJitter : CustomPostProcessingBase
    {
        // public ScanLineJitterFilerModeParameter FilterMode = new ScanLineJitterFilerModeParameter(UnityEngine.FilterMode.Bilinear);

        public DirectionParameter JitterDirection = new DirectionParameter(Direction.Horizontal);

        public IntervalTypeParameter IntervalType = new IntervalTypeParameter(PostProcessingExtends.IntervalType.Random);
        
        public ClampedFloatParameter Frequency = new ClampedFloatParameter(1, 0, 25);
        
        public ClampedFloatParameter JitterIndensity = new ClampedFloatParameter(0, 0, 1);
        
        
        private const string _shaderName = "Custom/PostProcessing/Glitch/ScanLineJitter";
        private RTHandle _tempRT0;
        // private RTHandle _tempRT1;
        private string _tempRT0Name => "_TemporaryRenderTexture0";
        // private string _tempRT1Name => "_TemporaryRenderTexture1";

        private int _paramsKeyword = Shader.PropertyToID("_ScanLineJitterParams");

        private float _randomFrequency;
        // private float _TimeX = 1.0f;
        public override bool IsActive() =>  _material != null && JitterIndensity.value > 0;
        

        public override CustomPostProcessingInjectionPoint InjectionPoint =>
            CustomPostProcessingInjectionPoint.AfterPostProcess;
        public override int OrderInInjectionPoint => 36;
        
        public override void Setup()
        {
            if (_material == null)
            {
                _material = CoreUtils.CreateEngineMaterial(_shaderName);
            }
        }
        
        private void UpdateFrequencySLJ()
        {
            if (IntervalType.value == PostProcessingExtends.IntervalType.Random)
            {
                _randomFrequency = UnityEngine.Random.Range(0, Frequency.value);
            }

            if (IntervalType.value == PostProcessingExtends.IntervalType.Infinite)
            {
                SetKeyword("USING_FREQUENCY_INFINITE");
                // scanLineJitter.EnableKeyword("USING_FREQUENCY_INFINITE");
            }
            else
            {
                SetKeyword("USING_FREQUENCY_INFINITE", false);
                // scanLineJitter.DisableKeyword("USING_FREQUENCY_INFINITE");
            }
        }

        public override void Render(CommandBuffer cmd, ref RenderingData renderingData, in RTHandle source, in RTHandle destination)
        {
            if (_material == null)
            {
                return;
            }
            
            
            Draw(cmd, source, _tempRT0);
            
            UpdateFrequencySLJ();
            float displacement = 0.005f + Mathf.Pow(JitterIndensity.value, 3) * 0.1f;
            float threshold = Mathf.Clamp01(1.0f - JitterIndensity.value * 1.2f);
            cmd.SetGlobalVector(_paramsKeyword, new Vector3(displacement, threshold, IntervalType.value == PostProcessingExtends.IntervalType.Random ? _randomFrequency : Frequency.value));
            
            Draw(cmd, _tempRT0, destination, (int)JitterDirection.value);
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
        public sealed class DirectionParameter : VolumeParameter<Direction> { public DirectionParameter(Direction value, bool overrideState = false) : base(value, overrideState) { } }

        // [Serializable]
        // public sealed class ScanLineJitterFilerModeParameter : VolumeParameter<FilterMode> { public ScanLineJitterFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

        [Serializable]
        public sealed class IntervalTypeParameter : VolumeParameter<IntervalType>{ public IntervalTypeParameter(IntervalType value, bool overrideState = false) : base(value, overrideState) { } }
    }
}