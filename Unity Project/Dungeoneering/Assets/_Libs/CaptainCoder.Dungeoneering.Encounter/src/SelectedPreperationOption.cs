using CaptainCoder.Unity.Assertions;

using NaughtyAttributes;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CaptainCoder.Dungeoneering.Encounter
{
    public class SelectedPreperationOption : MonoBehaviour, IPointerClickHandler
    {

        [SerializeField] private PreperationData _preperationData;
        const string DefaultTooltip = "<u>Tactic</u>Click a tactic to select it";
        public PreperationData PreperationData
        {
            get => _preperationData;
            set
            {
                _preperationData = value;
                if (_preperationData == null) { Clear(); }
                else { Render(); }
            }
        }
        [AssertIsSet][SerializeField] private Image _icon;
        [AssertIsSet][SerializeField] private SimpleTooltip _simpleTooltip;
        [AssertIsSet][SerializeField] private Image _background;

        void Awake()
        {
            _preperationData = null;
            Clear();
        }

        [Button]
        public void Clear()
        {
            _preperationData = null;
            _icon.enabled = false;
            _simpleTooltip.Tooltip = DefaultTooltip;
            _simpleTooltip.HideHover();
        }

        [Button]
        public void Render()
        {
            _icon.enabled = true;
            _icon.sprite = PreperationData.Sprite;
            _simpleTooltip.Tooltip = $"{_preperationData.Tooltip}\n\n<b>Click to Clear</b>";
        }

        public void OnPointerClick(PointerEventData eventData) => Clear();
    }
}