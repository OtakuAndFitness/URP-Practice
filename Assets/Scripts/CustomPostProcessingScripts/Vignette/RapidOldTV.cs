using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Vignette/RapidOldTV")]
    public class RapidOldTV : VolumeComponent, IPostProcessComponent
    {
        public RapidOldTVTypeParameter vignetteType = new RapidOldTVTypeParameter(VignetteType.ClassicMode);
        public ClampedFloatParameter vignetteIndensity = new ClampedFloatParameter(0, 0, 5);
        public Vector2Parameter vignetteCenter = new Vector2Parameter(new Vector2(0.5f,0.5f));
        public ColorParameter vignetteColor = new ColorParameter(new Color(0.1f, 0.8f, 1.0f) , true,true,true);
        

        public bool IsActive() => vignetteIndensity.value > 0;

        public bool IsTileCompatible()
        {
            return false;
        }
        
    }
    
    public enum VignetteType
    {
        ClassicMode = 0,
        ColorMode = 1,
    }
    
    [Serializable]
    public sealed class RapidOldTVTypeParameter : VolumeParameter<VignetteType> { public RapidOldTVTypeParameter(VignetteType value, bool overrideState = false) : base(value, overrideState) { } }

}

