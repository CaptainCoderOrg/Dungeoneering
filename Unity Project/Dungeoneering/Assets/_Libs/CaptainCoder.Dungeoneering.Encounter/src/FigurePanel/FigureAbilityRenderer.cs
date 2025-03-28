using CaptainCoder.Unity.Assertions;

using TMPro;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public sealed class FigureAbilityRenderer : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private TextMeshProUGUI _label;
        [AssertIsSet][SerializeField] private Hoverable _hoverable;
        [AssertIsSet][SerializeField] private TooltipElementData _tooltipElementData;
        private FigureAbilityData _ability;
        public FigureAbilityData Ability
        {
            get => _ability;
            set
            {
                _ability = value;
                _label.text = _ability.Name;
            }
        }

        void Awake()
        {
            _hoverable.OnHoverStart.AddListener(ShowHover);
            _hoverable.OnHoverEnd.AddListener(HideHover);
        }

        private void ShowHover(RectTransform target)
        {
            _tooltipElementData.Tooltip.Text = Ability.Tooltop;
            _tooltipElementData.Tooltip.ShowAbove(target);
        }

        private void HideHover() => _tooltipElementData.Tooltip.Hide();
    }
}