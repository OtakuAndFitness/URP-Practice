using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable,VolumeComponentMenu("Custom-post-processing/Pixelise/Sector")]
    public class Sector : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter pixelSize = new ClampedFloatParameter(0, 0, 1);
        public ClampedFloatParameter circleRadius = new ClampedFloatParameter(0.8f, 0.01f, 1);
        public ClampedFloatParameter pixelIntervalX = new ClampedFloatParameter(1, 0.2f, 5);
        public ClampedFloatParameter pixelIntervalY = new ClampedFloatParameter(1, 0.2f, 5);
        public ColorParameter backgroundColor = new ColorParameter(Color.black, true, true, true);

        
        public bool IsActive()
        {
            return active && pixelSize.value != 0;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
}

