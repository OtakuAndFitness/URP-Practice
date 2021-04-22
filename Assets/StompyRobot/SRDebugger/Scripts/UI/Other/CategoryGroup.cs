using SRF;
using UnityEngine;
using UnityEngine.UI;

namespace SRDebugger.UI.Other
{
    public class CategoryGroup : SRMonoBehaviourEx
    {
        [RequiredField] public RectTransform Container;

        [RequiredField] public Text Header;
    }
}
