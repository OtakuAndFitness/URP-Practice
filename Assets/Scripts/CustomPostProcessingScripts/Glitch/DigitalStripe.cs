using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Glitch/DigitalStripe")]
    public class DigitalStripe : VolumeComponent, IPostProcessComponent
    {
        // public DigitalStripeFilerModeParameter FilterMode = new DigitalStripeFilerModeParameter(UnityEngine.FilterMode.Bilinear);

        public ClampedFloatParameter indensity = new ClampedFloatParameter(0, 0, 1);

        public ClampedIntParameter frequency = new ClampedIntParameter(3, 1, 10);
        
        public ClampedFloatParameter stripeLength = new ClampedFloatParameter(0.89f, 0, 0.99f);

        public ClampedIntParameter noiseTextureWidth = new ClampedIntParameter(20, 8, 256);

        public ClampedIntParameter noiseTextureHeight = new ClampedIntParameter(20, 8, 256);

        public BoolParameter needStripColorAdjust = new BoolParameter(false);

        public ColorParameter stripColorAdjustColor = new ColorParameter(new Color(0.1f, 0.1f, 0.1f), true,true,true);

        public ClampedFloatParameter stripColorAdjustIndensity = new ClampedFloatParameter(2, 0, 10);
        
        // private Texture2D _noiseTexture;
        // private RenderTexture _trashFrame1;
        // private RenderTexture _trashFrame2;

        
        // private const string _shaderName = "Custom/PostProcessing/Glitch/DigitalStripe";
        // private RTHandle _tempRT0;
        // private RTHandle _tempRT1;
        // private string _tempRT0Name => "_TemporaryRenderTexture0";
        // private string _tempRT1Name => "_TemporaryRenderTexture1";

        // private int _colorKeyword = Shader.PropertyToID("_StripColorAdjustColor");
        // private int _indensityKeyword = Shader.PropertyToID("_DigitalStripeIndensity");
        // private int _colorIndensityKeyword = Shader.PropertyToID("_StripColorAdjustIndensity");
        // private int _texKeyword = Shader.PropertyToID("_DigitalStripeNoiseTex");

        // private float _TimeX = 1.0f;
        public bool IsActive() =>  indensity.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }


        // public override CustomPostProcessingInjectionPoint InjectionPoint =>
        //     CustomPostProcessingInjectionPoint.AfterPostProcess;
        // public override int OrderInInjectionPoint => 32;
        //
        // public override void Setup()
        // {
        //     if (_material == null)
        //     {
        //         _material = CoreUtils.CreateEngineMaterial(_shaderName);
        //     }
        // }
        //
        // private void UpdateFrequencyDS(int frame, int noiseTextureWidth, int noiseTextureHeight, float stripLength)
        // {
        //     int frameCount = Time.frameCount;
        //     if (frameCount % frame != 0)
        //     {
        //         return;
        //     }
        //
        //     _noiseTexture = new Texture2D(noiseTextureWidth, noiseTextureHeight, TextureFormat.ARGB32, false);
        //     _noiseTexture.wrapMode = TextureWrapMode.Clamp;
        //     _noiseTexture.filterMode = FilterMode.Point;
        //
        //     // _trashFrame1 = new RenderTexture(Screen.width, Screen.height, 0);
        //     // _trashFrame2 = new RenderTexture(Screen.width, Screen.height, 0);
        //     // _trashFrame1.hideFlags = HideFlags.DontSave;
        //     // _trashFrame2.hideFlags = HideFlags.DontSave;
        //
        //     Color color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        //
        //     for (int y = 0; y < _noiseTexture.height; y++)
        //     {
        //         for (int x = 0; x < _noiseTexture.width; x++)
        //         {
        //             //随机值若大于给定strip随机阈值，重新随机颜色
        //             if (UnityEngine.Random.value > stripLength)
        //             {
        //                 color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        //             }
        //             //设置贴图像素值
        //             _noiseTexture.SetPixel(x, y, color);
        //         }
        //     }
        //
        //     _noiseTexture.Apply();
        //
        // }
        //
        // public override void Render(CommandBuffer cmd, ref RenderingData renderingData, in RTHandle source, in RTHandle destination)
        // {
        //     if (_material == null)
        //     {
        //         return;
        //     }
        //     
        //     UpdateFrequencyDS(frequency.value, noiseTextureWidth.value, noiseTextureHeight.value, stripeLength.value);
        //     Draw(cmd, source, _tempRT0);
        //     cmd.SetGlobalFloat(_indensityKeyword, indensity.value);
        //     if (_noiseTexture != null)
        //     {
        //         cmd.SetGlobalTexture(_texKeyword, _noiseTexture);
        //     }
        //     if (needStripColorAdjust.value)
        //     {
        //         SetKeyword("NEED_TRASH_FRAME");
        //         cmd.SetGlobalColor(_colorKeyword, stripColorAdjustColor.value);
        //         cmd.SetGlobalFloat(_colorIndensityKeyword, stripColorAdjustIndensity.value);
        //     }
        //     else
        //     {
        //         SetKeyword("NEED_TRASH_FRAME", false);
        //         // digitalStripe.DisableKeyword("NEED_TRASH_FRAME");
        //     }
        //     
        //     Draw(cmd, _tempRT0, destination, 0);
        // }
        //
        // public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        // {
        //     var descriptor = GetCameraRenderTextureDescriptor(renderingData);
        //     // descriptor.width = (int)(descriptor.width / downScaling.value);
        //     // descriptor.height = (int)(descriptor.height / downScaling.value);
        //
        //     RenderingUtils.ReAllocateIfNeeded(ref _tempRT0, descriptor, name: _tempRT0Name,
        //         wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
        //     // RenderingUtils.ReAllocateIfNeeded(ref _tempRT1, descriptor, name: _tempRT1Name,
        //     //     wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
        // }
        //
        // public override void Dispose(bool disposing)
        // {
        //     base.Dispose(disposing);
        //     
        //     CoreUtils.Destroy(_material);
        //     _tempRT0?.Release();
        //     
        //     DestroyImmediate(_noiseTexture);
        //     // _tempRT1?.Release();
        // }
        

        // [Serializable]
        // public sealed class DigitalStripeFilerModeParameter : VolumeParameter<FilterMode> { public DigitalStripeFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

    }
}