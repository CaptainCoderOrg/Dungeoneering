using System.Collections;

using CaptainCoder.Unity.Assertions;

using NaughtyAttributes;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class HUDController : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private CanvasGroup _canvasGroup;
        void Awake()
        {
            _canvasGroup.alpha = 0;
            StartCoroutine(RebuildAtEndOfFrame());
        }

        [Button]
        private void ForceRebuild()
        {
            StartCoroutine(RebuildAtEndOfFrame());
        }

        private IEnumerator RebuildAtEndOfFrame()
        {
            yield return null;
            yield return null;
            _canvasGroup.alpha = 1;
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
        }
    }
}