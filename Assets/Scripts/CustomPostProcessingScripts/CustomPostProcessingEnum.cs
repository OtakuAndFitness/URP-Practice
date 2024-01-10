using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostProcessingExtends
{
    public enum Direction
    {
        Horizontal = 0,
        Vertical = 1,
    }
        
    public enum IntervalType
    {
        Infinite,
        Periodic,
        Random
    }
    
    public enum DirectionEX
    {
        Horizontal = 0,
        Vertical = 1,
        Horizontal_Vertical =2,
    }

    public enum CustomPostProcessingInjectionPoint
    {
        AfterOpaqueAndSkybox,
        BeforePostProcess,
        AfterPostProcess
    }
}
