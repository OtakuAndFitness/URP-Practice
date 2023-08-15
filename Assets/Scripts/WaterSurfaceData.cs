using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable][CreateAssetMenu(fileName = "WaterSurfaceData", menuName = "WaterSystem/SurfaceData", order = 0)]
public class WaterSurfaceData : ScriptableObject
{
    public float maximumVisibility = 40.0f;
    public BasicWaves basicWaves = new BasicWaves(6, 1.5f, 45.0f, 5.0f);
    public int randomSeed = 3234;
    // public FoamSettings foamSettings = new FoamSettings();
    public WaveType waveType = WaveType.Gerstner;

    public Gradient absorptionRamp;
    public Gradient scatterRamp;
    public FoamSettings foamSettings = new FoamSettings();

    public bool isInit = false;
    // public bool isOffline = false;

    [Serializable]
    public enum WaveType
    {
        Sine,
        Gerstner,
        FFT
    }


    [Serializable]
    public struct BasicWaves
    {
        public int waveNums;
        public float amplitdue;
        public float waveLength;
        public float direction;

        public BasicWaves(int nums, float amp, float len, float dir)
        {
            waveNums = nums;
            amplitdue = amp;
            waveLength = len;
            direction = dir;

        }
        
    }

    [Serializable]
    public class FoamSettings
    {
        public int foamType;//0=default, 1=simple,2=custom
        public AnimationCurve basicFoam;
        public AnimationCurve liteFoam;
        public AnimationCurve mediumFoam;
        public AnimationCurve denseFoam;
    
        public FoamSettings()
        {
            foamType = 0;
            basicFoam = new AnimationCurve(new Keyframe[2] { new Keyframe(0.25f, 0f), new Keyframe(1f, 1f) });
            liteFoam = new AnimationCurve(new Keyframe[3] { new Keyframe(0.2f, 0f), new Keyframe(0.4f, 1f), new Keyframe(0.7f, 0f)});
            mediumFoam = new AnimationCurve(new Keyframe[3] { new Keyframe(0.4f, 0f), new Keyframe(0.7f, 1f), new Keyframe(1f, 0f) });
            denseFoam = new AnimationCurve(new Keyframe[2] { new Keyframe(0.7f, 0f), new Keyframe(1f, 1f) });
        }
    } 
}
