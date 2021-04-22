using SRDebugger.Internal;
using SRF;
using UnityEngine;

namespace SRDebugger.UI.Other
{
    [RequireComponent(typeof (Canvas))]
    public class ConfigureCanvasFromSettings : SRMonoBehaviour
    {
        private void Start()
        {
            var c = GetComponent<Canvas>();

            SRDebuggerUtil.ConfigureCanvas(c);

            Destroy(this);
        }
    }
}
