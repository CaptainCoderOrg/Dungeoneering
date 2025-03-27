using NaughtyAttributes;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class CanvasRebuilder : MonoBehaviour
    {
        [Button]
        public void ForceRebuild() => LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
        [Button]
        public void MarkForRebuild() => LayoutRebuilder.MarkLayoutForRebuild((RectTransform)transform);

        void Start() => ForceRebuild();
    }
}