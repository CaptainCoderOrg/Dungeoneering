using CaptainCoder.Unity.Assertions;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [RequireComponent(typeof(Hoverable))]
    public class SimpleTooltip : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private TooltipElementData _tooltipElement;
        [TextArea(3, 5)][SerializeField] private string _tooltip;

        void Awake()
        {
            Hoverable hoverable = GetComponent<Hoverable>();
            hoverable.OnHoverStart.AddListener(ShowHover);
            hoverable.OnHoverEnd.AddListener(HideHover);
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