using System;

using CaptainCoder.Unity.Assertions;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class DieIconController : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private TooltipElementData _tooltipElement;
        [AssertIsSet][SerializeField] private Image _image;
        [AssertIsSet][SerializeField] private Hoverable _hoverable;
        private DieData _die;
        public DieData Die
        {
            get => _die;
            set
            {
                _die = value;
                _image.sprite = value.Sprite;
                _image.color = value.UIAlbedo;
                _image.enabled = true;
            }
        }

        internal void Hide()
        {
            _image.enabled = false;
        }

        void Awake()
        {
            _hoverable.OnHoverStart.AddListener(ShowHover);
            _hoverable.OnHoverEnd.AddListener(HideHover);
        }

        private void HideHover()
        {
            _tooltipElement.DiceTooltipController.Hide();
        }

        private void ShowHover(RectTransform target)
        {
            _tooltipElement.DiceTooltipController.Die = _die;
            _tooltipElement.DiceTooltipController.ShowAbove(target);
        }
    }
}