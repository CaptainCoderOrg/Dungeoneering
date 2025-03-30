using System.Collections;

using CaptainCoder.Unity.Assertions;

using TMPro;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class TooltipController : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private TextMeshProUGUI _label;
        [AssertIsSet][SerializeField] private CanvasGroup _canvasGroup;
        public string Text
        {
            get => _label.text;
            set => _label.text = value;
        }

        void Awake() => Hide();

        public void ShowAbove(RectTransform parent)
        {
            transform.position = parent.position;
            StartCoroutine(Show());
        }

        public IEnumerator Show()
        {
            yield return null;
            // We must wait one frame for any visual changes to take place then
            // we calculate if we are on the screen and adjust accordingly
            ((RectTransform)transform).EnsureOnScreen();
            _canvasGroup.alpha = 1;
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
        }
    }
}