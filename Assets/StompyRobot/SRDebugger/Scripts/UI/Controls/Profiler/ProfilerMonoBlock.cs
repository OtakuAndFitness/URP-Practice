using System;
using SRF;
using UnityEngine;
using UnityEngine.UI;

namespace SRDebugger.UI.Controls.Profiler
{
    public class ProfilerMonoBlock : SRMonoBehaviourEx
    {
        private float _lastRefresh;

        [RequiredField] public Text CurrentUsedText;

        [RequiredField] public GameObject NotSupportedMessage;

        [RequiredField] public Slider Slider;

        [RequiredField] public Text TotalAllocatedText;

        protected override void OnEnable()
        {
            base.OnEnable();

            NotSupportedMessage.SetActive(!UnityEngine.Profiling.Profiler.supported);
            CurrentUsedText.gameObject.SetActive(UnityEngine.Profiling.Profiler.supported);

            TriggerRefresh();
        }

        protected override void Update()
        {
            base.Update();

            if (SRDebug.Instance.IsDebugPanelVisible && (Time.realtimeSinceStartup - _lastRefresh > 1f))
            {
                TriggerRefresh();
                _lastRefresh = Time.realtimeSinceStartup;
            }
        }

        public void TriggerRefresh()
        {
            var max = UnityEngine.Profiling.Profiler.supported ? UnityEngine.Profiling.Profiler.GetMonoHeapSize() : GC.GetTotalMemory(false);
            var current = UnityEngine.Profiling.Profiler.GetMonoUsedSize();

            Slider.maxValue = max;
            Slider.value = current;

            var maxMb = (max >> 10);
            maxMb /= 1024; // On new line to workaround IL2CPP bug

            var currentMb = (current >> 10);
            currentMb /= 1024;

            TotalAllocatedText.text = "Total: <color=#FFFFFF>{0}</color>MB".Fmt(maxMb);

            if (currentMb > 0)
            {
                CurrentUsedText.text = "<color=#FFFFFF>{0}</color>MB".Fmt(currentMb);
            }
        }

        public void TriggerCollection()
        {
            GC.Collect();
            TriggerRefresh();
        }
    }
}
