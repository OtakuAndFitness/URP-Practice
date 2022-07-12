using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable,VolumeComponentMenu("Custom-post-processing/Pixelise/Led")]
    public class Led : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter pixelSize = new ClampedFloatParameter(0, 0, 1);
        public ClampedFloatParameter ledRadius = new ClampedFloatParameter(1, 0.01f, 1);
        public ColorParameter backgroundColor = new ColorParameter(Color.black, true, true, true);
        public BoolParameter useAutoScreenRatio = new BoolParameter(false);
        public ClampedFloatParameter pixelRatio = new ClampedFloatParameter(1, 0.2f,5);
        
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

