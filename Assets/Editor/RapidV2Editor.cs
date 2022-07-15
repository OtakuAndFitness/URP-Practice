using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace UnityEditor.Rendering.Universal
{
    [VolumeComponentEditor(typeof(RapidV2))]
    public class RapidV2Editor : VolumeComponentEditor
    {
        // private SerializedDataParameter m_FilterMode;
        private SerializedDataParameter m_VignetteType;
        private SerializedDataParameter m_VignetteIndensity;
        private SerializedDataParameter m_VignetteSharpness;
        private SerializedDataParameter m_VignetteCenter;
        private SerializedDataParameter m_VignetteColor;


        public override void OnEnable()
        {
            var o = new PropertyFetcher<RapidV2>(serializedObject);
            
            // m_FilterMode = Unpack(o.Find(x => x.FilterMode));
            m_VignetteType = Unpack(o.Find(x => x.vignetteType));
            m_VignetteIndensity = Unpack(o.Find(x => x.vignetteIndensity));
            m_VignetteSharpness = Unpack(o.Find(x => x.vignetteSharpness));
            m_VignetteCenter = Unpack(o.Find(x => x.vignetteCenter));
            m_VignetteColor = Unpack(o.Find(x => x.vignetteColor));

        }

        public override void OnInspectorGUI()
        {            
            
            // PropertyField(m_FilterMode);
            // DrawHeader("Jitter Direction");
            PropertyField(m_VignetteType);
            // DrawHeader("Interval Frequency");
            PropertyField(m_VignetteIndensity);
            PropertyField(m_VignetteSharpness);
            PropertyField(m_VignetteCenter);
            if (m_VignetteType.value.intValue == (int)VignetteType.ColorMode)
            {
                PropertyField(m_VignetteColor);
            }
            // DrawHeader("Core Property");
            // PropertyField(m_Speed);
            // PropertyField(m_Amount);
            // DrawHeader("Custom Jitter Resolution");
            // PropertyField(m_CustomResolution);
            // if (m_CustomResolution.value.boolValue)
            // {
            //     PropertyField(m_Resolution);
            // }

        }
    }
}

