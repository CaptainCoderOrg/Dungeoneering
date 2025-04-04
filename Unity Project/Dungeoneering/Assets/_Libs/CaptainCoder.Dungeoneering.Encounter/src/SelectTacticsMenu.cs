using System.Collections;
using System.Linq;

using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class SelectTacticsMenu : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private CanvasGroup _canvasGroup;
        [AssertIsSet][SerializeField] private SelectedTactic[] _selectedOptions;
        [AssertIsSet][SerializeField] private TacticOption[] _options;
        private HeroFigurePanel _attachedPanel;

        void Awake()
        {
            Hide();
            foreach (var option in _options)
            {
                option.OnSelectOption += Select;
            }
        }

        public void Select(TacticData data)
        {
            SelectedTactic option = _selectedOptions.FirstOrDefault(o => o.PreperationData == null);
            if (option != null)
            {
                option.PreperationData = data;
            }
            else
            {
                _selectedOptions[_selectedOptions.Length - 1].PreperationData = data;
            }
        }

        public void SelectAndShow(HeroFigurePanel targetHeroPanel)
        {
            if (_attachedPanel != null) { _attachedPanel.OnMoved -= AttachToPanel; }
            _attachedPanel = targetHeroPanel;
            _attachedPanel.OnMoved += AttachToPanel;
            AttachToPanel(targetHeroPanel);
        }

        private void AttachToPanel(HeroFigurePanel panel) => StartCoroutine(AttachAtEndOfFrame(panel));

        private IEnumerator AttachAtEndOfFrame(HeroFigurePanel panel)
        {
            yield return null;
            yield return null;
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            transform.position = panel.TopLeftPivot.position;
            ((RectTransform)transform).EnsureOnScreen();
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}