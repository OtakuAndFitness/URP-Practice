using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace UnityEditor.Rendering.Universal
{
    [VolumeComponentEditor(typeof(DigitalStripe))]
    public class DigitalStripeEditor : VolumeComponentEditor
    {
        private SerializedDataParameter m_FilterMode;
        private SerializedDataParameter m_Indensity;
        private SerializedDataParameter m_Frequency;
        private SerializedDataParameter m_StripeLength;
        private SerializedDataParameter m_NoiseTextureWidth;
        private SerializedDataParameter m_NoiseTextureHeight;
        private SerializedDataParameter m_NeedStripColorAdjust;
        private SerializedDataParameter m_StripColorAdjustColor;
        private SerializedDataParameter m_StripColorAdjustIndensity;


        public override void OnEnable()
        {
            var o = new PropertyFetcher<DigitalStripe>(serializedObject);
            
            m_FilterMode = Unpack(o.Find(x => x.FilterMode));
            m_Indensity = Unpack(o.Find(x => x.indensity));
            m_StripeLength = Unpack(o.Find(x => x.stripeLength));
            m_Frequency = Unpack(o.Find(x => x.frequency));
            m_NoiseTextureWidth = Unpack(o.Find(x => x.noiseTextureWidth));
            m_NoiseTextureHeight = Unpack(o.Find(x => x.noiseTextureHeight));
            m_NeedStripColorAdjust = Unpack(o.Find(x => x.needStripColorAdjust));
            m_StripColorAdjustColor = Unpack(o.Find(x => x.stripColorAdjustColor));
            m_StripColorAdjustIndensity = Unpack(o.Find(x => x.stripColorAdjustIndensity));

        }

        public override void OnInspectorGUI()
        {            
            
            PropertyField(m_FilterMode);
            DrawHeader("Core Property");
            PropertyField(m_Indensity);
            PropertyField(m_Frequency);
            DrawHeader("Stripe Generate");
            PropertyField(m_StripeLength);
            DrawHeader("Noise Texture Size");
            PropertyField(m_NoiseTextureWidth);
            PropertyField(m_NoiseTextureHeight);
            DrawHeader("Stripe Adjust Color");
            PropertyField(m_NeedStripColorAdjust);
            if (m_NeedStripColorAdjust.value.boolValue)
            {
                PropertyField(m_StripColorAdjustColor);
                PropertyField(m_StripColorAdjustIndensity);
            }
            
        }
    }
}

