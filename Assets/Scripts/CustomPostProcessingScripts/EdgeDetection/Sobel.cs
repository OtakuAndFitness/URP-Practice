using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Rendering.Universal
{
    [Serializable,VolumeComponentMenu("Custom-post-processing/EdgeDetection/Sobel")]
    public class Sobel : VolumeComponent, IPostProcessComponent
    {
        // public SobelFilterModeParameter FilterMode = new SobelFilterModeParameter(UnityEngine.FilterMode.Bilinear);
        
        public ClampedFloatParameter edgeWidth = new ClampedFloatParameter(0, 0, 5);
        
        public ColorParameter edgeColor = new ColorParameter(Color.black, true, true, true);
        
        public ClampedFloatParameter backgroundFade = new ClampedFloatParameter(0, 0, 1);
        
        public ColorParameter backgroundColor = new ColorParameter(Color.white, true, true, true);


        public bool IsActive()
        {
            return active && edgeWidth.value != 0;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
        

        // [Serializable]
        // public sealed class SobelFilterModeParameter : VolumeParameter<FilterMode> { public SobelFilterModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

    }
}