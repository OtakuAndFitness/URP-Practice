using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable, VolumeComponentMenu("Custom-Post-Processing/Glitch/LineBlock")]
    public class LineBlock : CustomPostProcessingBase
    {
        // public LineBlockFilerModeParameter FilterMode = new LineBlockFilerModeParameter(UnityEngine.FilterMode.Bilinear);

        public DirectionParameter BlockDirection = new DirectionParameter(Direction.Horizontal);

        public IntervalTypeParameter IntervalType = new IntervalTypeParameter(PostProcessingExtends.IntervalType.Random);
        
        public ClampedFloatParameter Frequency = new ClampedFloatParameter(1, 0, 25);
        
        public ClampedFloatParameter Amount = new ClampedFloatParameter(0, 0, 1);

        public ClampedFloatParameter LinesWidth = new ClampedFloatParameter(1, 0.1f, 10);

        public ClampedFloatParameter Speed = new ClampedFloatParameter(0.8f, 0, 1);

        public ClampedFloatParameter Offset = new ClampedFloatParameter(1, 0, 13);

        public ClampedFloatParameter Alpha = new ClampedFloatParameter(1, 0, 1);


        private const string _shaderName = "Custom/PostProcessing/Glitch/LineBlock";
        private RTHandle _tempRT0;
        // private RTHandle _tempRT1;
        private string _tempRT0Name => "_TemporaryRenderTexture0";
        // private string _tempRT1Name => "_TemporaryRenderTexture1";

        private int _paramsKeyword = Shader.PropertyToID("_LineBlockParams");
        private int _params2Keyword = Shader.PropertyToID("_LineBlockParams2");

        private float _TimeX = 1.0f;
        public override bool IsActive() =>  _material != null && Amount.value > 0;
        
        private int _frameCount = 0;
        private float _randomFrequency;

        public override CustomPostProcessingInjectionPoint InjectionPoint =>
            CustomPostProcessingInjectionPoint.AfterPostProcess;
        public override int OrderInInjectionPoint => 34;
        
        public override void Setup()
        {
            if (_material == null)
            {
                _material = CoreUtils.CreateEngineMaterial(_shaderName);
            }
        }
        
        private void UpdateFrequency()
        {
            if (IntervalType.value == PostProcessingExtends.IntervalType.Random)
            {
                if (_frameCount > Frequency.value)
                {

                    _frameCount = 0;
                    _randomFrequency = UnityEngine.Random.Range(0, Frequency.value);
                }
                _frameCount++;
            }

            if (IntervalType.value == PostProcessingExtends.IntervalType.Infinite)
            {
                SetKeyword("USING_FREQUENCY_INFINITE");
                // _material.EnableKeyword("USING_FREQUENCY_INFINITE");
            }
            else
            {
                // _material.DisableKeyword("USING_FREQUENCY_INFINITE");
                SetKeyword("USING_FREQUENCY_INFINITE", false);
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
            
            UpdateFrequency();
            
            Draw(cmd, source, _tempRT0);
            cmd.SetGlobalVector(_paramsKeyword, new Vector3(
                IntervalType.value == PostProcessingExtends.IntervalType.Random ? _randomFrequency : Frequency.value,
                _TimeX * Speed.value * 0.2f , Amount.value));
            cmd.SetGlobalVector(_params2Keyword, new Vector3(Offset.value, 1 / LinesWidth.value, Alpha.value));
            int pass = (int)BlockDirection.value;
            
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
        
        [Serializable]
        public sealed class DirectionParameter : VolumeParameter<Direction> { public DirectionParameter(Direction value, bool overrideState = false) : base(value, overrideState) { } }

        // [Serializable]
        // public sealed class LineBlockFilerModeParameter : VolumeParameter<FilterMode> { public LineBlockFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

        [Serializable]
        public sealed class IntervalTypeParameter : VolumeParameter<IntervalType>{ public IntervalTypeParameter(IntervalType value, bool overrideState = false) : base(value, overrideState) { } }
    }
}