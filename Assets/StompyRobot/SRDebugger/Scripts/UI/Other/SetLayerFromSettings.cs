﻿using SRF;

namespace SRDebugger.UI.Other
{
    public class SetLayerFromSettings : SRMonoBehaviour
    {
        private void Start()
        {
            gameObject.SetLayerRecursive(Settings.Instance.DebugLayer);
        }
    }
}
