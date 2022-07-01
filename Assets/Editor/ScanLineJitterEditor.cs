using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace UnityEditor.Rendering.Universal
{
    [VolumeComponentEditor(typeof(ScanLineJitter))]
    public class ScanLineJitterEditor : VolumeComponentEditor
    {
        private SerializedDataParameter m_FilterMode;
        private SerializedDataParameter m_JitterDirection;
        private SerializedDataParameter m_IntervalType;
        private SerializedDataParameter m_Frequency;
        private SerializedDataParameter m_JitterIndensity;


        public override void OnEnable()
        {
            var o = new PropertyFetcher<ScanLineJitter>(serializedObject);
            
            m_FilterMode = Unpack(o.Find(x => x.FilterMode));
            m_JitterDirection = Unpack(o.Find(x => x.JitterDirection));
            m_IntervalType = Unpack(o.Find(x => x.IntervalType));
            m_Frequency = Unpack(o.Find(x => x.Frequency));
            m_JitterIndensity = Unpack(o.Find(x => x.JitterIndensity));

        }

        public override void OnInspectorGUI()
        {            
            
            PropertyField(m_FilterMode);
            DrawHeader("Jitter Direction");
            PropertyField(m_JitterDirection);
            DrawHeader("Interval Frequency");
            PropertyField(m_IntervalType);
            if (m_IntervalType.value.intValue != (int)IntervalType.Infinite)
            {
                PropertyField(m_Frequency);
            }
            DrawHeader("Jitter Property");
            PropertyField(m_JitterIndensity);
            
        }
    }
}

