using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [VolumeComponentEditor(typeof(Quad))]
    public class QuadEditor : VolumeComponentEditor
    {
        // private SerializedDataParameter m_FilterMode;
        private SerializedDataParameter m_pixelSize;
        // private SerializedDataParameter m_gridSize;
        private SerializedDataParameter m_useAutoScreenRatio;
        private SerializedDataParameter m_pixelRatio;
        private SerializedDataParameter m_pixelScaleX;
        private SerializedDataParameter m_pixelScaleY;


        public override void OnEnable()
        {
            var o = new PropertyFetcher<Quad>(serializedObject);
            
            // m_FilterMode = Unpack(o.Find(x => x.FilterMode));
            m_pixelSize = Unpack(o.Find(x => x.pixelSize));
            // m_gridSize = Unpack(o.Find(x => x.gridWidth));
            m_useAutoScreenRatio = Unpack(o.Find(x => x.useAutoScreenRatio));
            m_pixelRatio = Unpack(o.Find(x => x.pixelRatio));
            m_pixelScaleX = Unpack(o.Find(x => x.pixelScaleX));
            m_pixelScaleY = Unpack(o.Find(x => x.pixelScaleY));

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
            PropertyField(m_pixelScaleX);
            PropertyField(m_pixelScaleY);

        }
    }
}

