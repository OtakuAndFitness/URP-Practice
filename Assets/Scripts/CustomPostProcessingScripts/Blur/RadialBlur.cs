using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Blur/RadialBlur")]
    public class RadialBlur : VolumeComponent, IPostProcessComponent
    {
        // public RadialBlurQualityParameter qualityLevel = new RadialBlurQualityParameter(RadialBlurQuality.RadialBlur_8Tap_Balance);
        // public RadialFilerModeParameter filterMode = new RadialFilerModeParameter(FilterMode.Bilinear);
        public ClampedFloatParameter RadialCenterX = new ClampedFloatParameter(0.5f, 0.0f, 1.0f);
        public ClampedFloatParameter RadialCenterY = new ClampedFloatParameter(0.5f, 0.0f, 1.0f);
        public ClampedFloatParameter blurSize = new ClampedFloatParameter(0.6f, 0.0f, 1.0f);
        public ClampedIntParameter iteration = new ClampedIntParameter(0, 0, 30);

        // private const string _shaderName = "Custom/PostProcessing/Blur/RadialBlur";
        // private RTHandle _tempRT0;
        // private RTHandle _tempRT1;
        // private string _tempRT0Name => "_TemporaryRenderTexture0";
        // private string _tempRT1Name => "_TemporaryRenderTexture1";

        // private int _blurParametersKeyword = Shader.PropertyToID("_RadialBlurParameters");
        
        public bool IsActive() =>  iteration.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }


        // public override CustomPostProcessingInjectionPoint InjectionPoint =>
        //     CustomPostProcessingInjectionPoint.AfterPostProcess;
        // public override int OrderInInjectionPoint => 9;
        // public override void Setup()
        // {
        //     if (_material == null)
        //     {
        //         _material = CoreUtils.CreateEngineMaterial(_shaderName);
        //     }
        // }
        //
        // public override void Render(CommandBuffer cmd, ref RenderingData renderingData, in RTHandle source, in RTHandle destination)
        // {
        //     if (_material == null)
        //     {
        //         return;
        //     }
        //     
        //     cmd.SetGlobalVector(_blurParametersKeyword, new Vector4(iteration.value, blurSize.value * 0.02f,RadialCenterX.value, RadialCenterY.value));
        //     Draw(cmd, source, destination, 0);
        // }
        //
        // public override void Dispose(bool disposing)
        // {
        //     base.Dispose(disposing);
        //     
        //     CoreUtils.Destroy(_material);
        //     
        //     // _tempRT0?.Release();
        //     // _tempRT1?.Release();
        // }
    }
    
    // [Serializable]
    // public sealed class RadialFilerModeParameter : VolumeParameter<FilterMode> { public RadialFilerModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }
    
    // [Serializable]
    // public sealed class RadialBlurQualityParameter : VolumeParameter<RadialBlurQuality> { public RadialBlurQualityParameter(RadialBlurQuality value, bool overrideState = false) : base(value, overrideState) { } }

    
    // public enum RadialBlurQuality
    // {
    //     RadialBlur_4Tap_Fatest = 0,
    //     RadialBlur_6Tap = 1,
    //     RadialBlur_8Tap_Balance = 2,
    //     RadialBlur_10Tap = 3,
    //     RadialBlur_12Tap = 4,
    //     RadialBlur_20Tap_Quality = 5,
    //     RadialBlur_30Tap_Extreme = 6,
    // }

}

