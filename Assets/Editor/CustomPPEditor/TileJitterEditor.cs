using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [VolumeComponentEditor(typeof(TileJitter))]
    public class TileJitterEditor : VolumeComponentEditor
    {
        // private SerializedDataParameter m_FilterMode;
        private SerializedDataParameter m_JitterDirection;
        private SerializedDataParameter m_IntervalType;
        private SerializedDataParameter m_Frequency;
        private SerializedDataParameter m_SplittingDirection;
        private SerializedDataParameter m_SplittingNumber;
        private SerializedDataParameter m_Amount;
        private SerializedDataParameter m_Speed;


        public override void OnEnable()
        {
            var o = new PropertyFetcher<TileJitter>(serializedObject);
            
            // m_FilterMode = Unpack(o.Find(x => x.FilterMode));
            m_JitterDirection = Unpack(o.Find(x => x.JitterDirection));
            m_IntervalType = Unpack(o.Find(x => x.IntervalType));
            m_Frequency = Unpack(o.Find(x => x.Frequency));
            m_Amount = Unpack(o.Find(x => x.Amount));
            m_SplittingDirection = Unpack(o.Find(x => x.SplittingDirection));
            m_Speed = Unpack(o.Find(x => x.Speed));
            m_SplittingNumber = Unpack(o.Find(x => x.SplittingNumber));

        }

        public override void OnInspectorGUI()
        {            
            
            // PropertyField(m_FilterMode);
            DrawHeader("Splitting Property");
            PropertyField(m_SplittingDirection);
            PropertyField(m_SplittingNumber);
            DrawHeader("Interval Frequency");
            PropertyField(m_IntervalType);
            if (m_IntervalType.value.intValue != (int)IntervalType.Infinite)
            {
                PropertyField(m_Frequency);
            }
            DrawHeader("Jitter Property");
            PropertyField(m_JitterDirection);
            PropertyField(m_Amount);
            PropertyField(m_Speed);
            
        }
    }
}

