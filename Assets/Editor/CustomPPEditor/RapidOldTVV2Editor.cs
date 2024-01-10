using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [VolumeComponentEditor(typeof(RapidOldTVV2))]
    public class RapidOldTVV2Editor : VolumeComponentEditor
    {
        // private SerializedDataParameter m_FilterMode;
        private SerializedDataParameter m_VignetteType;
        private SerializedDataParameter m_VignetteSize;
        private SerializedDataParameter m_SizeOffset;
        private SerializedDataParameter m_VignetteColor;


        public override void OnEnable()
        {
            var o = new PropertyFetcher<RapidOldTVV2>(serializedObject);
            
            // m_FilterMode = Unpack(o.Find(x => x.FilterMode));
            m_VignetteType = Unpack(o.Find(x => x.vignetteType));
            m_VignetteSize = Unpack(o.Find(x => x.vignetteSize));
            m_SizeOffset = Unpack(o.Find(x => x.sizeOffset));
            m_VignetteColor = Unpack(o.Find(x => x.vignetteColor));

        }

        public override void OnInspectorGUI()
        {            
            
            // PropertyField(m_FilterMode);
            // DrawHeader("Jitter Direction");
            PropertyField(m_VignetteType);
            // DrawHeader("Interval Frequency");
            PropertyField(m_VignetteSize);
            PropertyField(m_SizeOffset);
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

