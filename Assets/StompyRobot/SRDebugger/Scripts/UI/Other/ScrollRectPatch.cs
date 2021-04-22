using UnityEngine;
using UnityEngine.UI;

namespace SRDebugger.UI.Other
{
    [RequireComponent(typeof (ScrollRect))]
    [ExecuteInEditMode]
    public class ScrollRectPatch : MonoBehaviour
    {
        public RectTransform Content;
        public Mask ReplaceMask;
        public RectTransform Viewport;
#if !(UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_4_7)

        private void Awake()
        {
            var scrollRect = GetComponent<ScrollRect>();

            scrollRect.content = Content;
            scrollRect.viewport = Viewport;

            if (ReplaceMask != null)
            {
                var go = ReplaceMask.gameObject;

                Destroy(go.GetComponent<Graphic>());
                Destroy(go.GetComponent<CanvasRenderer>());
                Destroy(ReplaceMask);

                go.AddComponent<RectMask2D>();
            }
        }

#endif
    }
}
