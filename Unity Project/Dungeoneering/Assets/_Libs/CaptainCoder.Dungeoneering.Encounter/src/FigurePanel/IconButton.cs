using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    [RequireComponent(typeof(CanvasGroup), typeof(LayoutElement), typeof(SimpleTooltip))]
    public class IconButton : MonoBehaviour, IPointerClickHandler
    {
        private CanvasGroup _canvasGroup;
        private CanvasGroup CanvasGroup => _canvasGroup ??= GetComponent<CanvasGroup>();
        private LayoutElement _layoutElement;
        private LayoutElement LayoutElement => _layoutElement ??= GetComponent<LayoutElement>();
        private SimpleTooltip _simpleTooltip;
        public SimpleTooltip Tooltip => _simpleTooltip ??= GetComponent<SimpleTooltip>();
        [SerializeField] private bool _enabled = true;
        [SerializeField] private bool _isVisible = true;
        [field: SerializeField] public UnityEvent OnClick { get; private set; }
        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                CanvasGroup.alpha = GetAlpha();
            }
        }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                LayoutElement.ignoreLayout = !_isVisible;
                CanvasGroup.blocksRaycasts = _isVisible;
                CanvasGroup.alpha = GetAlpha();
            }
        }

        private float GetAlpha() => (_isVisible, _enabled) switch
        {
            (false, _) => 0,
            (true, true) => 1,
            (true, false) => 0.5f,
        };


        void Awake()
        {
            _layoutElement = GetComponent<LayoutElement>();
            _canvasGroup = GetComponent<CanvasGroup>();
            IsVisible = _isVisible;
            Enabled = _enabled;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_enabled) { OnClick?.Invoke(); }
        }
    }
}