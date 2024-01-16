using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/Pixelise/Leaf")]
    public class Leaf : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter pixelSize = new ClampedFloatParameter(0, 0, 1);
        public BoolParameter useAutoScreenRatio = new BoolParameter(false);
        public ClampedFloatParameter pixelRatio = new ClampedFloatParameter(1, 0.2f,5);
        public ClampedFloatParameter pixelScaleX = new ClampedFloatParameter(1, 0.2f, 5);
        public ClampedFloatParameter pixelScaleY = new ClampedFloatParameter(1, 0.2f, 5);

        public bool IsActive() =>  pixelSize.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }
        
    }
}

