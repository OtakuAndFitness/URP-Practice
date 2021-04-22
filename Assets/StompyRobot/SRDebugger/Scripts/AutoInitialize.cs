#if UNITY_5

using UnityEngine;

namespace SRDebugger
{
    public static class AutoInitialize
    {
        [RuntimeInitializeOnLoadMethod]
        public static void OnLoad()
        {
            if (Settings.Instance.IsEnabled && Settings.Instance.AutoLoad)
            {
                SRDebug.Init();
            }
        }
    }
}

#endif
