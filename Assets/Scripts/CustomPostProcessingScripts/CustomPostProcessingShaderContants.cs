using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostProcessingExtends
{
    public static class CustomPostProcessingShaderConstants
    {
        public static readonly int _Offset = Shader.PropertyToID("_Offset");
        public static readonly int _GoldenRot = Shader.PropertyToID("_GoldenRot");
        public static readonly int _Params = Shader.PropertyToID("_Params");
        public static readonly int _Params2 = Shader.PropertyToID("_Params2");
        public static readonly int _Params3 = Shader.PropertyToID("_Params3");
        public static readonly int _Gradient = Shader.PropertyToID("_Gradient");
        public static readonly int _StripColorAdjustIndensity = Shader.PropertyToID("_StripColorAdjustIndensity");
        public static readonly int _StripColorAdjustColor = Shader.PropertyToID("_StripColorAdjustColor");
        public static readonly int _NoiseTex = Shader.PropertyToID("_NoiseTex");
        public static readonly int _Indensity = Shader.PropertyToID("_Indensity");
        public static readonly int _ScreenShake = Shader.PropertyToID("_ScreenShake");
        public static readonly int _Resolution = Shader.PropertyToID("_Resolution");
        public static readonly int _BackgroundColor = Shader.PropertyToID("_BackgroundColor");
        public static readonly int _EdgeColor = Shader.PropertyToID("_EdgeColor");
        public static readonly int _PixelSize = Shader.PropertyToID("_PixelSize");
        public static readonly int _VignetteColor = Shader.PropertyToID("_VignetteColor");
        public static readonly int _Sharpness = Shader.PropertyToID("_Sharpness");
        public static readonly int _Brightness = Shader.PropertyToID("_Brightness");
        public static readonly int _HueDegree = Shader.PropertyToID("_HueDegree");
        public static readonly int _ColorTint = Shader.PropertyToID("_ColorTint");
        public static readonly int _Tint = Shader.PropertyToID("_Tint");
        public static readonly int _Temperature = Shader.PropertyToID("_Temperature");
        public static readonly int _LensColor = Shader.PropertyToID("_LensColor");
        public static readonly int _Saturation = Shader.PropertyToID("_Saturation");
        public static readonly int _ColorBalance = Shader.PropertyToID("_ColorBalance");
        public static readonly int _Exposure = Shader.PropertyToID("_Exposure");
        public static readonly int _ToColor = Shader.PropertyToID("_ToColor");
        public static readonly int _FromColor = Shader.PropertyToID("_FromColor");
        public static readonly int _Fuzziness = Shader.PropertyToID("_Fuzziness");
        public static readonly int _Range = Shader.PropertyToID("_Range");
        public static readonly int _Contrast = Shader.PropertyToID("_Contrast");
        public static readonly int _ContrastFactorRGB = Shader.PropertyToID("_ContrastFactorRGB");


    }

    internal enum CustomPostProcessingProfileId
    {
        //Blur
        BokehBlur,
        BoxBlur,
        DirectionalBlur,
        DualKawaseBlur,
        GaussianBlur,
        GrainyBlur,
        IrisBlur,
        KawaseBlur,
        RadialBlur,
        TiltShiftBlur,
        //ColorAdjustment
        BleachBypass,
        Brightness,
        ColorReplace,
        ColorReplaceV2,
        Contrast,
        ContrastV2,
        ContrastV3,
        Hue,
        LensFilter,
        Saturation,
        Technicolor,
        Tint,
        WhiteBalance,
        //EdgeDetection
        Roberts,
        RobertsNeon,
        Scharr,
        ScharrNeon,
        Sobel,
        SobelNeon,
        //Glitch
        AnalogNoise,
        DigitalStripe,
        ImageBlock,
        LineBlock,
        RGBSplit,
        ScanLineJitter,
        ScreenJump,
        ScreenShake,
        TileJitter,
        WaveJitter,
        //Pixelise
        Circle,
        Diamond,
        Hexagon,
        HexagonGrid,
        Leaf,
        Led,
        Quad,
        Sector,
        Triangle,
        //Sharpen
        SharpenV1,
        SharpenV2,
        SharpenV3,
        //Vignette
        Aurora,
        Rapid,
        RapidOldTV,
        RapidOldTVV2,
        RapidV2,
    }
}

	