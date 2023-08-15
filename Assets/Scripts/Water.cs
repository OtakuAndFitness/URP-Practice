using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class Water : MonoBehaviour
{
    [SerializeField]
    public WaterReflectionData waterReflectionData;
    
    [SerializeField]
    public WaterSurfaceData waterSurfaceData;
    
    [SerializeField]
    public WaterResources waterResources;
    
    private Texture2D _rampTexture;

    private static readonly int AbsorptionScatteringRamp = Shader.PropertyToID("_AbsorptionScatteringRamp");

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init()
    {
        GenerateColorRamp();
    }
    
    public void GenerateColorRamp()
    {
        if(_rampTexture == null)
            _rampTexture = new Texture2D(128, 4, GraphicsFormat.R8G8B8A8_SRGB, TextureCreationFlags.None);
        _rampTexture.wrapMode = TextureWrapMode.Clamp;
        
        var defaultFoamRamp = waterResources.defaultFoamRamp;
        
        var cols = new Color[512];
        for (var i = 0; i < 128; i++)
        {
            cols[i] = waterSurfaceData.absorptionRamp.Evaluate(i / 128f);
        }
        for (var i = 0; i < 128; i++)
        {
            cols[i + 128] = waterSurfaceData.scatterRamp.Evaluate(i / 128f);
        }
        for (var i = 0; i < 128; i++)
        {
            switch(waterSurfaceData.foamSettings.foamType)
            {
                case 0: // default
                    cols[i + 256] = defaultFoamRamp.GetPixelBilinear(i / 128f , 0.5f);
                    break;
                case 1: // simple
                    cols[i + 256] = defaultFoamRamp.GetPixelBilinear(waterSurfaceData.foamSettings.basicFoam.Evaluate(i / 128f) , 0.5f);
                    break;
            }
        }
        _rampTexture.SetPixels(cols);
        _rampTexture.Apply();
        // if (!waterSurfaceData.isOffline)
            Shader.SetGlobalTexture(AbsorptionScatteringRamp, _rampTexture);
    }
}
