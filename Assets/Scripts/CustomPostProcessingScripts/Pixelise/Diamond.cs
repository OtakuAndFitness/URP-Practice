using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Pixelise/Diamond")]
    public class Diamond : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter pixelSize = new ClampedFloatParameter(0, 0, 1);
        

        public bool IsActive() =>  pixelSize.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }
    }
}

