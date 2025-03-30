using CaptainCoder.Unity.Assertions;

using TMPro;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class DiceTooltipController : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _dieLabel;
        [SerializeField] private DieFaceRenderer[] _faces;
        public DieData Die
        {
            set
            {
                _dieLabel.text = value.Name;
                _faces[0].RenderDieFace(value, 0);
                _faces[1].RenderDieFace(value, 1);
                _faces[2].RenderDieFace(value, 2);
                _faces[3].RenderDieFace(value, 3);
                _faces[4].RenderDieFace(value, 4);
                _faces[5].RenderDieFace(value, 5);
            }
        }

        void Awake() => Hide();

        public void ShowAbove(RectTransform parent)
        {
            transform.position = parent.position;
            ((RectTransform)transform).EnsureOnScreen();
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