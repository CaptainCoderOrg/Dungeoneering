
using CaptainCoder.Unity.Assertions;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class AttackAbilityRenderer : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private ToggleablePanel _toggleablePanel;
        [AssertIsSet][SerializeField] private SimpleTooltip _tooltip;
        [AssertIsSet][SerializeField] private CanvasGroup _confirmButton;
        [AssertIsSet][SerializeField] private CanvasGroup[] _powerIcons;
        [AssertIsSet][SerializeField] private Image _abilityIcon;
        [SerializeField] private AttackAbilityData _attackAbilityData;
        public event System.Action<AttackAbilityData> OnSelected;

        public AttackAbilityData AttackAbility
        {
            get => _attackAbilityData;
            set
            {
                OnSelected = null;
                _attackAbilityData = value;
                _abilityIcon.sprite = _attackAbilityData.Icon;
                _tooltip.Tooltip = _attackAbilityData.Description;
                for (int ix = 0; ix < _powerIcons.Length; ix++)
                {
                    CanvasGroup icon = _powerIcons[ix];
                    icon.alpha = ix < _attackAbilityData.PowerCost ? 1 : 0;
                }
            }
        }

        public bool IsEnabled
        {
            get => _toggleablePanel.IsEnabled;
            set => _toggleablePanel.IsEnabled = value;
        }

        private bool _isAvailable = false;
        public bool IsAvailable
        {
            get => _isAvailable;
            set
            {
                _isAvailable = value;
                _confirmButton.alpha = _isAvailable ? 1 : 0.5f;
            }
        }

        public void Selected()
        {
            Debug.Log("Selected");
            OnSelected?.Invoke(_attackAbilityData);
        }
    }
}