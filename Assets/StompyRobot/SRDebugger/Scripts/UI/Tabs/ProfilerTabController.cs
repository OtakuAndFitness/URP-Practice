//#define SR_CONSOLE_DEBUG

using SRF;
using UnityEngine.UI;

namespace SRDebugger.UI.Tabs
{
    public class ProfilerTabController : SRMonoBehaviourEx
    {
        private bool _isDirty;

        [RequiredField] public Toggle PinToggle;

        protected override void Start()
        {
            base.Start();

            PinToggle.onValueChanged.AddListener(PinToggleValueChanged);
            Refresh();
        }

        private void PinToggleValueChanged(bool isOn)
        {
            SRDebug.Instance.IsProfilerDocked = isOn;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _isDirty = true;
        }

        protected override void Update()
        {
            base.Update();

            if (_isDirty)
            {
                Refresh();
            }
        }

        private void Refresh()
        {
            PinToggle.isOn = SRDebug.Instance.IsProfilerDocked;
            _isDirty = false;
        }
    }
}
