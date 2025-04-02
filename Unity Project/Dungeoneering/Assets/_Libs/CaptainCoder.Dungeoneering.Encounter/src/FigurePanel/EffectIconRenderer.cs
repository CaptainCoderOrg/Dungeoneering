using CaptainCoder.Unity.Assertions;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public sealed class EffectIconRenderer : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private CanvasGroup _canvasGroup;
        [AssertIsSet][SerializeField] private SimpleTooltip _simpleTooltip;
        [AssertIsSet][SerializeField] private Image _icon;
        public void Render(EffectData data)
        {
            if (data == null) { Hide(); }
            _icon.sprite = data.Icon;
            _simpleTooltip.Tooltip = data.Description;
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}