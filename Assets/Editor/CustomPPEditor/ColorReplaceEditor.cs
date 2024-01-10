using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends.Effects
{
    [VolumeComponentEditor(typeof(ColorReplace))]
    public class ColorReplaceEditor : VolumeComponentEditor
    {
        // private SerializedDataParameter m_FilterMode;
        // private SerializedDataParameter m_ColorType;
        private SerializedDataParameter m_FromColor;
        private SerializedDataParameter m_ToColor;
        private SerializedDataParameter m_FromGradientColor;
        private SerializedDataParameter m_ToGradientColor;
        private SerializedDataParameter m_GridentSpeed;
        private SerializedDataParameter m_Range;
        private SerializedDataParameter m_Fuzziness;


        public override void OnEnable()
        {
            var o = new PropertyFetcher<ColorReplace>(serializedObject);
            
            // m_FilterMode = Unpack(o.Find(x => x.FilterMode));
            // m_ColorType = Unpack(o.Find(x => x.colorType));
            m_FromColor = Unpack(o.Find(x => x.FromColor));
            m_ToColor = Unpack(o.Find(x => x.ToColor));
            // m_FromGradientColor = Unpack(o.Find(x => x.FromGradientColor));
            // m_ToGradientColor = Unpack(o.Find(x => x.ToGradientColor));
            // m_GridentSpeed = Unpack(o.Find(x => x.gridentSpeed));
            m_Range = Unpack(o.Find(x => x.Range));
            m_Fuzziness = Unpack(o.Find(x => x.Fuzziness));

        }

        public override void OnInspectorGUI()
        {
            // PropertyField(m_ColorType);
            DrawHeader("From-To Color");
            // if (m_ColorType.value.intValue == (int) ColorType.Original)
            // {
                PropertyField(m_FromColor);
                PropertyField(m_ToColor);
            // }
            // else
            // {
                // PropertyField(m_FromGradientColor);
                // PropertyField(m_ToGradientColor);
                // PropertyField(m_GridentSpeed);
            // }
            DrawHeader("Color Precision");
            PropertyField(m_Range);
            PropertyField(m_Fuzziness);
            

        }
    }
}

