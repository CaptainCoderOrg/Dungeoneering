using System.Collections;

using CaptainCoder.Unity.Assertions;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    [RequireComponent(typeof(CanvasGroup), typeof(LayoutElement))]
    public class ToggleablePanel : MonoBehaviour
    {

        [AssertIsSet][SerializeField] private CanvasGroup _canvasGroup;
        [AssertIsSet][SerializeField] private LayoutElement _layoutElement;
        [SerializeField] private bool _isEnabled = true;
        public event System.Action OnChange;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                if (_isEnabled) { Show(); }
                else { Hide(); }
            }
        }

        void Awake()
        {
            _canvasGroup ??= GetComponent<CanvasGroup>();
            _layoutElement ??= GetComponent<LayoutElement>();
            if (_isEnabled) { Show(); }
            else { Hide(); }
        }

        public void Show()
        {
            _layoutElement.ignoreLayout = false;
            OnChange?.Invoke();
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(ShowAtEndOfFrame());
            }
        }

        public IEnumerator ShowAtEndOfFrame()
        {
            yield return null;
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            _layoutElement.ignoreLayout = true;
            OnChange?.Invoke();
        }

        public void Toggle() => IsEnabled = !IsEnabled;
    }
}