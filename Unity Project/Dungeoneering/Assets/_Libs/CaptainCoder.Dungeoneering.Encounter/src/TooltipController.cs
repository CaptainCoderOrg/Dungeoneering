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
            Show();
        }

        public void Show()
        {
            _canvasGroup.alpha = 1;
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
        }
    }
}