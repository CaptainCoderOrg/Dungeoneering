using CaptainCoder.Unity.Assertions;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class HeroActionButton : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private CanvasGroup _canvasGroup;
        [AssertIsSet][SerializeField] private LayoutElement _layoutElement;
        [AssertIsSet][SerializeField] private EventTrigger _eventTrigger;
        [AssertIsSet][field: SerializeField] public SimpleTooltip Tooltip { get; private set; }
        [SerializeField] private bool _isHiddenOnStart;
        [SerializeField] private bool _enabled = true;
        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                _eventTrigger.enabled = _enabled;
            }
        }

        void Awake()
        {
            Enabled = _enabled;
            if (_isHiddenOnStart) { Hide(); }
            else { Show(); }
        }

        public void Show()
        {
            _canvasGroup.alpha = _enabled ? 1 : 0.5f;
            _canvasGroup.blocksRaycasts = true;
            _layoutElement.ignoreLayout = false;
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            _layoutElement.ignoreLayout = true;
        }
    }
}