using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [VolumeComponentEditor(typeof(Led))]
    public class LedEditor : VolumeComponentEditor
    {
        private SerializedDataParameter m_pixelSize;
        private SerializedDataParameter m_ledRadius;
        private SerializedDataParameter m_useAutoScreenRatio;
        private SerializedDataParameter m_backgroundColor;
        private SerializedDataParameter m_pixelRatio;


        public override void OnEnable()
        {
            var o = new PropertyFetcher<Led>(serializedObject);
            
            // m_FilterMode = Unpack(o.Find(x => x.FilterMode));
            m_pixelSize = Unpack(o.Find(x => x.pixelSize));
            m_ledRadius = Unpack(o.Find(x => x.ledRadius));
            m_useAutoScreenRatio = Unpack(o.Find(x => x.useAutoScreenRatio));
            m_pixelRatio = Unpack(o.Find(x => x.pixelRatio));
            m_backgroundColor = Unpack(o.Find(x => x.backgroundColor));

        }

        public override void OnInspectorGUI()
        {            
            
            // PropertyField(m_FilterMode);
            DrawHeader("Core Property");
            PropertyField(m_pixelSize);
            PropertyField(m_useAutoScreenRatio);
            if (!m_useAutoScreenRatio.value.boolValue)
            {
                PropertyField(m_pixelRatio);
            }
            DrawHeader("Pixel Scale");
            PropertyField(m_backgroundColor);

        }
    }
}

