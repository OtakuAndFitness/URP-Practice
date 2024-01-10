using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [VolumeComponentEditor(typeof(ScharrNeon))]
    public class ScharrNeonEditor : VolumeComponentEditor
    {
        // private SerializedDataParameter m_FilterMode;
        private SerializedDataParameter m_edgeWidth;
        private SerializedDataParameter m_edgeNeonFade;
        private SerializedDataParameter m_backgroundFade;
        private SerializedDataParameter m_brigtness;
        private SerializedDataParameter m_backgroundColor;


        public override void OnEnable()
        {
            var o = new PropertyFetcher<ScharrNeon>(serializedObject);
            
            // m_FilterMode = Unpack(o.Find(x => x.FilterMode));
            m_edgeWidth = Unpack(o.Find(x => x.edgeWidth));
            m_edgeNeonFade = Unpack(o.Find(x => x.edgeNeonFade));
            m_backgroundFade = Unpack(o.Find(x => x.backgroundFade));
            m_brigtness = Unpack(o.Find(x => x.brightness));
            m_backgroundColor = Unpack(o.Find(x => x.backgroundColor));

        }

        public override void OnInspectorGUI()
        {            
            
            // PropertyField(m_FilterMode);
            DrawHeader("Edge Property");
            PropertyField(m_edgeWidth);
            PropertyField(m_edgeNeonFade);
            DrawHeader("Background Property( For Edge Neon Fade <1 )");
            PropertyField(m_backgroundFade);
            PropertyField(m_backgroundColor);
            DrawHeader("Edge Property");
            PropertyField(m_brigtness);

        }
    }
}

