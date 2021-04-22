using SRF;
using UnityEngine.UI;

namespace SRDebugger.UI.Other
{
    public class VersionTextBehaviour : SRMonoBehaviourEx
    {
        public string Format = "SRDebugger {0}";

        [RequiredField] public Text Text;

        protected override void Start()
        {
            base.Start();

            Text.text = string.Format(Format, SRDebug.Version);
        }
    }
}
