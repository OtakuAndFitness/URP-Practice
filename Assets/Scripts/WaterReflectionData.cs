using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable][CreateAssetMenu(fileName = "WaterReflectionData", menuName = "WaterSystem/ReflectionData", order = 0)]
public class WaterReflectionData : ScriptableObject
{
    [Serializable]
    public enum ReflectionType
    {
        Cubemap,
        ReflectionProbe,
        PlanarReflection
    }

    public ReflectionType reflectionType = ReflectionType.ReflectionProbe;

    public PlanarReflection.PlanarReflectionSettings planarReflectionSettings;

    public Cubemap cubemapType;

}
