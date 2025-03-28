using CaptainCoder.Unity.Assertions;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class AttackIconController : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private TooltipElementData _tooltipElement;
        [AssertIsSet][SerializeField] private Image _image;
        [AssertIsSet][SerializeField] private Hoverable _hoverable;
        private AttackTypeData _attackType;
        public AttackTypeData AttackType
        {
            get => _attackType;
            set
            {
                _attackType = value;
                _image.sprite = value.Sprite;
            }
        }

        void Awake()
        {
            _hoverable.OnHoverStart.AddListener(ShowHover);
            _hoverable.OnHoverEnd.AddListener(HideHover);
        }

        private void HideHover() => _tooltipElement.Tooltip.Hide();

        private void ShowHover(RectTransform target)
        {
            _tooltipElement.Tooltip.Text = _attackType.Tooltip;
            _tooltipElement.Tooltip.ShowAbove(target);
        }
    }
}