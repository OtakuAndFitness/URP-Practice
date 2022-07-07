using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Rendering.Universal
{
    [Serializable,VolumeComponentMenu("Custom-post-processing/EdgeDetection/RobertsNeon")]
    public class RobertsNeon : VolumeComponent, IPostProcessComponent
    {
        public RobertsNeonFilterModeParameter FilterMode = new RobertsNeonFilterModeParameter(UnityEngine.FilterMode.Bilinear);
        
        public ClampedFloatParameter edgeWidth = new ClampedFloatParameter(0, 0, 5);
        
        public ClampedFloatParameter edgeNeonFade = new ClampedFloatParameter(1,0.1f,1);
        
        public ClampedFloatParameter backgroundFade = new ClampedFloatParameter(0, 0, 1);

        public ClampedFloatParameter brigtness = new ClampedFloatParameter(1,0.2f,2);

        public ColorParameter backgroundColor = new ColorParameter(Color.white, true, true, true);


        public bool IsActive()
        {
            return active && edgeWidth.value != 0;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
        

        [Serializable]
        public sealed class RobertsNeonFilterModeParameter : VolumeParameter<FilterMode> { public RobertsNeonFilterModeParameter(FilterMode value, bool overrideState = false) : base(value, overrideState) { } }

    }
}