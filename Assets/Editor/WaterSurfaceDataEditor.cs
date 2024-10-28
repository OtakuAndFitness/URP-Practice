using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaterSurfaceData))]
public class WaterSurfaceDataEditor : Editor
{
    private string mvTip = "This controls the max depth of the waters transparency/visiblility, the absorption and scattering gradients map to this depth. Units:Meters";
    private string waveCountTip = "Number of waves the automatic setup creates, if aiming for mobile set to 6 or less";
    private string amplitudeTip = "The average height of the waves. Units:Meters";
    private string waveLengthTip = "The average wavelength of the waves. Units:Meters";
    private string windDirTip = "The general wind direction, this is in degrees from Z+";
    private string alignButtonTip = "This aligns the wave direction to the current scene view camera facing direction";
    private string randomSeedTip = "This seed controls the automatic wave generation";
    private string absorpRampTip = "This gradient controls the color of the water as it gets deeper, darkening the surfaces under the water as they get deeper.";
    private string scatterRampTip = "This gradient controls the 'scattering' of the water from shallow to deep, lighting the water as there becomes more of it.";
    private string foamCurveTip = "This curve control the foam propagation. X is wave height and Y is foam opacity";

    
    static string[] foamTypeOptions = new string[2] { "Automatic", "Simple Curve" };

    // private bool isClickedBakeButton = false;

    private void OnValidate()
    {
        SerializedProperty isInit = serializedObject.FindProperty("isInit");
        if (isInit?.boolValue == false)
        {
            SetUp();
        }
    }

    private void SetUp()
    {
        WaterSurfaceData wsd = (WaterSurfaceData)target;
        wsd.isInit = true;
        wsd.absorptionRamp = DefaultAbsorptionGrad();
        wsd.scatterRamp = DefaultScatterGrad();
        EditorUtility.SetDirty(wsd);
    }
    
    private Gradient DefaultAbsorptionGrad() // Preset for absorption
    {
        Gradient g = new Gradient();
        GradientColorKey[] gck = new GradientColorKey[5];
        GradientAlphaKey[] gak = new GradientAlphaKey[1];
        gak[0].alpha = 1;
        gak[0].time = 0;
        gck[0].color = Color.white;
        gck[0].time = 0f;
        gck[1].color = new Color(0.22f, 0.87f, 0.87f);
        gck[1].time = 0.082f;
        gck[2].color = new Color(0f, 0.47f, 0.49f);
        gck[2].time = 0.318f;
        gck[3].color = new Color(0f, 0.275f, 0.44f);
        gck[3].time = 0.665f;
        gck[4].color = Color.black;
        gck[4].time = 1f;
        g.SetKeys(gck, gak);
        return g;
    }

    private Gradient DefaultScatterGrad() // Preset for scattering
    {
        Gradient g = new Gradient();
        GradientColorKey[] gck = new GradientColorKey[4];
        GradientAlphaKey[] gak = new GradientAlphaKey[1];
        gak[0].alpha = 1;
        gak[0].time = 0;
        gck[0].color = Color.black;
        gck[0].time = 0f;
        gck[1].color = new Color(0.08f, 0.41f, 0.34f);
        gck[1].time = 0.15f;
        gck[2].color = new Color(0.13f, 0.55f, 0.45f);
        gck[2].time = 0.42f;
        gck[3].color = new Color(0.21f, 0.62f, 0.6f);
        gck[3].time = 1f;
        g.SetKeys(gck, gak);
        return g;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Maximum Water Depth", EditorStyles.boldLabel);
        EditorGUI.indentLevel += 1;
        SerializedProperty maximumVisibility = serializedObject.FindProperty("maximumVisibility");
        EditorGUILayout.Slider(maximumVisibility, 3, 300, new GUIContent("Water Depth", mvTip));
        
        SmallHeader("Color Controls");
        SerializedProperty absorptionRamp = serializedObject.FindProperty("absorptionRamp");
        EditorGUILayout.PropertyField(absorptionRamp, new GUIContent("Absorption Color",absorpRampTip), true, null);
        SerializedProperty scatterRamp = serializedObject.FindProperty("scatterRamp");
        EditorGUILayout.PropertyField(scatterRamp, new GUIContent("Scattering Color",scatterRampTip), true, null);
        
        SmallHeader("Foam Controls");
        SerializedProperty foamSettings = serializedObject.FindProperty("foamSettings");
        SerializedProperty foamType = foamSettings.FindPropertyRelative("foamType");
        foamType.intValue = GUILayout.Toolbar(foamType.intValue, foamTypeOptions);

        switch (foamType.intValue)
        {
            case 0:
                EditorGUILayout.HelpBox("Automatic will distribute the foam suitable for an average swell", MessageType.Info);
                break;
            case 1:
                EditorGUILayout.BeginHorizontal();
                // float preWidth = EditorGUIUtility.labelWidth;
                // EditorGUIUtility.labelWidth = 50f;
                EditorGUILayout.LabelField(new GUIContent("Foam Profile", foamCurveTip));
                // EditorGUIUtility.labelWidth = preWidth;
                SerializedProperty basicFoam = foamSettings.FindPropertyRelative("basicFoam");
                basicFoam.animationCurveValue = EditorGUILayout.CurveField(basicFoam.animationCurveValue, Color.white,
                    new Rect(Vector2.zero, Vector2.one));
                EditorGUILayout.EndHorizontal();
                break;
        }

        // GUI.enabled = false;
        // SerializedProperty isOffline = serializedObject.FindProperty("isOffline");
        // EditorGUILayout.Toggle(new GUIContent("Ramp is Offline?"), isOffline.boolValue);
        // GUI.enabled = true;
        
        // if (GUILayout.Button("Bake"))
        // {
            // isClickedBakeButton = true;
            // BakeOfflineTexture(isClickedBakeButton);
        // }
        
        // if (GUILayout.Button("UnBake"))
        // {
            // isClickedBakeButton = false;
            // BakeOfflineTexture(isClickedBakeButton);
        // }
        
        EditorGUI.indentLevel -= 1;
        EditorGUILayout.LabelField("Water Wave Type", EditorStyles.boldLabel);
        SerializedProperty waveType = serializedObject.FindProperty("waveType");
        waveType.enumValueIndex = GUILayout.Toolbar(waveType.enumValueIndex, waveType.enumDisplayNames);
        EditorGUI.indentLevel += 1;

        switch (waveType.enumValueIndex)
        {
            case 0:
                //Sine
                
                break;
            case 1:
                //Gerstner
                SerializedProperty basicWaves = serializedObject.FindProperty("basicWaves");
                SerializedProperty waveCount = basicWaves.FindPropertyRelative("waveNums");
                EditorGUILayout.IntSlider(waveCount, 1, 10, new GUIContent("Wave Count", waveCountTip));
                SerializedProperty amplitude = basicWaves.FindPropertyRelative("amplitdue");
                EditorGUILayout.Slider(amplitude, 0.1f, 30.0f, new GUIContent("Amplitude", amplitudeTip));
                SerializedProperty waveLength = basicWaves.FindPropertyRelative("waveLength");
                EditorGUILayout.Slider(waveLength, 1.0f, 200.0f, new GUIContent("Wave Length", waveLengthTip));
                EditorGUILayout.BeginHorizontal();
                SerializedProperty direction = basicWaves.FindPropertyRelative("direction");
                EditorGUILayout.Slider(direction, -180.0f, 180.0f, new GUIContent("Wind Direction", windDirTip));
                if (GUILayout.Button(new GUIContent("Align to scene camera", alignButtonTip)))
                {
                    direction.floatValue = CameraRelativeDirection();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                SerializedProperty randomSeed = serializedObject.FindProperty("randomSeed");
                randomSeed.intValue =
                    EditorGUILayout.IntField(new GUIContent("Random Seed", randomSeedTip), randomSeed.intValue);
                if (GUILayout.Button("Randomise Waves"))
                {
                    randomSeed.intValue = DateTime.Now.Millisecond * 100 - DateTime.Now.Millisecond;
                }
                EditorGUILayout.EndHorizontal();
                break;
            case 2:
                //FFT
                
                break;
        }
        
        EditorGUI.indentLevel -= 1;

        serializedObject.ApplyModifiedProperties();
    }

    // private void BakeOfflineTexture(bool isOffline)
    // {
    //     WaterSurfaceData wsd = (WaterSurfaceData)target;
    //     wsd.isOffline = isOffline;
    //     EditorUtility.SetDirty(wsd);
    // }

    private float CameraRelativeDirection()
    {
        float degrees = 0;

        Vector3 camFwd = SceneView.lastActiveSceneView.camera.transform.forward;
        camFwd.y = 0f;
        camFwd.Normalize();
        float dot = Vector3.Dot(-Vector3.forward, camFwd);
        degrees = Mathf.LerpUnclamped(90.0f, 180.0f, dot);
        if(camFwd.x < 0)
            degrees *= -1f;

        return Mathf.RoundToInt(degrees * 1000) / 1000;
    }

    void SmallHeader(string header)
    {
        EditorGUI.indentLevel -= 1;
        EditorGUILayout.LabelField(header, EditorStyles.miniBoldLabel);
        EditorGUI.indentLevel += 1;
    }
}
