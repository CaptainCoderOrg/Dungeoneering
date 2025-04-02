using CaptainCoder.Unity.Assertions;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [RequireComponent(typeof(Hoverable))]
    public class SimpleTooltip : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private TooltipElementData _tooltipElement;
        [TextArea(3, 5)][SerializeField] private string _tooltip;
        private Hoverable _hoverable;
        public string Tooltip { get => _tooltip; set => _tooltip = value; }

        public void Hide() => _hoverable.Cancel();

        void Awake()
        {
            _hoverable = GetComponent<Hoverable>();
            _hoverable.OnHoverStart.AddListener(ShowHover);
            _hoverable.OnHoverEnd.AddListener(HideHover);
        }

        private void HideHover()
        {
            _tooltipElement.Tooltip.Hide();
        }

        private void ShowHover(RectTransform target)
        {
            _tooltipElement.Tooltip.Text = _tooltip;
            _tooltipElement.Tooltip.ShowAbove(target);
        }
    }
}