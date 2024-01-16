using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [Serializable,VolumeComponentMenu("Custom-Post-Processing/ColorAdjustment/ColorReplace")]
    public class ColorReplace : VolumeComponent, IPostProcessComponent
    {
        public ColorParameter FromColor = new ColorParameter(new Color(0.8f, 0.0f, 0.0f, 1), true, true, true);
        public ColorParameter ToColor = new ColorParameter(new Color(0.0f, 0.8f, 0.0f, 1), true, true, true);
        public ClampedFloatParameter Range = new ClampedFloatParameter(0, 0, 1);
        public ClampedFloatParameter Fuzziness = new ClampedFloatParameter(0.5f,0f,1f);
        

        public bool IsActive() =>  Range.value > 0;
        public bool IsTileCompatible()
        {
            return false;
        }
        
    }
}

