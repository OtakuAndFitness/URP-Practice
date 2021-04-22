using UnityEngine;
using UnityEngine.UI;

namespace SRDebugger.UI.Other
{
    [RequireComponent(typeof (ScrollRect))]
    public class ScrollSettingsBehaviour : MonoBehaviour
    {
        public const float ScrollSensitivity = 40f;

        private void Awake()
        {
            var scrollRect = GetComponent<ScrollRect>();
            scrollRect.scrollSensitivity = ScrollSensitivity;

            if (!Internal.SRDebuggerUtil.IsMobilePlatform)
            {
                scrollRect.movementType = ScrollRect.MovementType.Clamped;
                scrollRect.inertia = false;
            }
        }
    }
}
