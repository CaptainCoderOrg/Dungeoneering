using CaptainCoder.Unity.Assertions;

using NaughtyAttributes;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CaptainCoder.Dungeoneering.Encounter
{
    public class TacticOption : MonoBehaviour, IPointerClickHandler
    {
        [AssertIsSet][field: SerializeField] public TacticData PreperationData { get; private set; }
        [AssertIsSet][SerializeField] private Image _icon;
        [AssertIsSet][SerializeField] private SimpleTooltip _tooltip;
        [AssertIsSet][SerializeField] private Image _background;
        public event System.Action<TacticData> OnSelectOption;

        [Button]
        public void Awake()
        {
            _background.color = PreperationData.BackgroundAlbedo;
            _icon.sprite = PreperationData.Sprite;
            _tooltip.Tooltip = PreperationData.Tooltip;
        }

        public void Select()
        {
            _tooltip.HideHover();
            OnSelectOption?.Invoke(PreperationData);
        }

        public void OnPointerClick(PointerEventData eventData) => Select();
    }
}