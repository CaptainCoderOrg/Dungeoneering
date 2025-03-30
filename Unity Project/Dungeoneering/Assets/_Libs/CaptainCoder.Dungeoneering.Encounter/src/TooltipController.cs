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

        private readonly Vector3[] _corners = { default, default, default, default };
        public IEnumerator Show()
        {
            yield return null; 
            // We must wait one frame for any visual changes to take place then
            // we calculate if we are on the screen and adjust accordingly
            RectTransform rect = (RectTransform)transform;
            rect.GetWorldCorners(_corners);
            if (_corners[0].x < 10)
            {
                rect.position += new Vector3(-_corners[0].x + 10, 0, 0);
            }
            _canvasGroup.alpha = 1;
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
        }
    }
}