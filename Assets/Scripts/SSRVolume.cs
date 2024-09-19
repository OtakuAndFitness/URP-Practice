using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SSRVolume : VolumeComponent
{
    public BoolParameter isActive = new BoolParameter(true,true);

    [Header("产生反射最小平滑度")] 
    public ClampedFloatParameter minimumSmoothness = new ClampedFloatParameter(0.8f, 0.0f, 1f);
    
    [Header("像素抖动")] 
    public ClampedFloatParameter dithering = new ClampedFloatParameter(0.2f, 0.0f, 1f);

    [Header("物体厚度")] 
    public ClampedFloatParameter objectThickness = new ClampedFloatParameter(0.75f, 0.1f, 1f);
    
    [Header("最小射线步进距离")] 
    public ClampedIntParameter stride = new ClampedIntParameter(4, 1, 8);

    [Header("射线与物体相交之前的最大迭代次数")] 
    public ClampedIntParameter maxRaySteps = new ClampedIntParameter(64, 16, 128);

    [Header("模糊半径")] 
    public ClampedFloatParameter blurRadius = new ClampedFloatParameter(0.0008f, 0.0f, 0.001f);
}
