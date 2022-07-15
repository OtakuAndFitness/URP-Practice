using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable,VolumeComponentMenu("Custom-post-processing/Vignette/RapidOldTVV2")]
    public class RapidOldTVV2 : VolumeComponent, IPostProcessComponent
    {
        public RapidOldTVTypeParameter vignetteType = new RapidOldTVTypeParameter(VignetteType.ClassicMode);
        public ClampedFloatParameter vignetteSize = new ClampedFloatParameter(0, 0, 200);
        public ClampedFloatParameter sizeOffset = new ClampedFloatParameter(0.2f,0,1);
        public ColorParameter vignetteColor = new ColorParameter(new Color(0.1f, 0.8f, 1.0f) , true,true,true);
        

        public bool IsActive()
        {
            return active && vignetteSize.value != 0;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }

}

