using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [VolumeComponentEditor(typeof(LineBlock))]
    public class LineBlockEditor : VolumeComponentEditor
    {
        // private SerializedDataParameter m_FilterMode;
        private SerializedDataParameter m_BlockDirection;
        private SerializedDataParameter m_IntervalType;
        private SerializedDataParameter m_Frequency;
        private SerializedDataParameter m_Amount;
        private SerializedDataParameter m_LinesWidth;
        private SerializedDataParameter m_Speed;
        private SerializedDataParameter m_Offset;
        private SerializedDataParameter m_Alpha;


        public override void OnEnable()
        {
            var o = new PropertyFetcher<LineBlock>(serializedObject);
            
            // m_FilterMode = Unpack(o.Find(x => x.FilterMode));
            m_BlockDirection = Unpack(o.Find(x => x.BlockDirection));
            m_IntervalType = Unpack(o.Find(x => x.IntervalType));
            m_Frequency = Unpack(o.Find(x => x.Frequency));
            m_Amount = Unpack(o.Find(x => x.Amount));
            m_LinesWidth = Unpack(o.Find(x => x.LinesWidth));
            m_Speed = Unpack(o.Find(x => x.Speed));
            m_Offset = Unpack(o.Find(x => x.Offset));
            m_Alpha = Unpack(o.Find(x => x.Alpha));

        }

        public override void OnInspectorGUI()
        {            
            
            // PropertyField(m_FilterMode);
            DrawHeader("Block Direction");
            PropertyField(m_BlockDirection);
            DrawHeader("Interval Frequency");
            PropertyField(m_IntervalType);
            if (m_IntervalType.value.intValue != (int)IntervalType.Infinite)
            {
                PropertyField(m_Frequency);
            }
            DrawHeader("Core Property");
            PropertyField(m_Amount);
            PropertyField(m_LinesWidth);
            PropertyField(m_Speed);
            PropertyField(m_Offset);
            PropertyField(m_Alpha);
            
        }
    }
}

