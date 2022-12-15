using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace UnityEditor.Rendering.Universal
{
    [VolumeComponentEditor(typeof(WaveJitter))]
    public class WaveJitterEditor : VolumeComponentEditor
    {
        // private SerializedDataParameter m_FilterMode;
        private SerializedDataParameter m_JitterDirection;
        private SerializedDataParameter m_IntervalType;
        private SerializedDataParameter m_Frequency;
        private SerializedDataParameter m_RGBSplit;
        private SerializedDataParameter m_Speed;
        private SerializedDataParameter m_Amount;
        private SerializedDataParameter m_CustomResolution;
        private SerializedDataParameter m_Resolution;


        public override void OnEnable()
        {
            var o = new PropertyFetcher<WaveJitter>(serializedObject);
            
            // m_FilterMode = Unpack(o.Find(x => x.FilterMode));
            m_JitterDirection = Unpack(o.Find(x => x.JitterDirection));
            m_IntervalType = Unpack(o.Find(x => x.IntervalType));
            m_Frequency = Unpack(o.Find(x => x.Frequency));
            m_RGBSplit = Unpack(o.Find(x => x.RGBSplit));
            m_Speed = Unpack(o.Find(x => x.Speed));
            m_Amount = Unpack(o.Find(x => x.Amount));
            m_CustomResolution = Unpack(o.Find(x => x.CustomResolution));
            m_Resolution = Unpack(o.Find(x => x.Resolution));

        }

        public override void OnInspectorGUI()
        {            
            
            // PropertyField(m_FilterMode);
            DrawHeader("Jitter Direction");
            PropertyField(m_JitterDirection);
            DrawHeader("Interval Frequency");
            PropertyField(m_IntervalType);
            if (m_IntervalType.value.intValue != (int)IntervalType.Infinite)
            {
                PropertyField(m_Frequency);
            }
            DrawHeader("Core Property");
            PropertyField(m_RGBSplit);
            PropertyField(m_Speed);
            PropertyField(m_Amount);
            DrawHeader("Custom Jitter Resolution");
            PropertyField(m_CustomResolution);
            if (m_CustomResolution.value.boolValue)
            {
                PropertyField(m_Resolution);
            }

        }
    }
}

