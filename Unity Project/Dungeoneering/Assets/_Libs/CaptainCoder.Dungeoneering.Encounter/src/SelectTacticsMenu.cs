using System.Collections;
using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class SelectTacticsMenu : MonoBehaviour
    {
        const string BeginTurnText = "Begin turn using selected tactics";
        const string SelectTacticsText = "Select tactics to begin turn";
        private EncounterController _encounterController;
        [AssertIsSet][SerializeField] private CanvasGroup _canvasGroup;
        [AssertIsSet][SerializeField] private SelectedTactic[] _selectedOptions;
        [AssertIsSet][SerializeField] private TacticOption[] _options;
        private HeroFigurePanel _attachedPanel;
        [AssertIsSet][SerializeField] private IconButton _confirmButton;
        [AssertIsSet][SerializeField] private IconButton _cancelButton;

        void Awake()
        {
            _encounterController = GetComponentInParent<EncounterController>();
            Debug.Assert(_encounterController != null, $"Could not locate {nameof(EncounterController)}", this);
            Hide();
        }

        void Start()
        {
            foreach (var option in _options)
            {
                option.OnSelectOption += Select;
            }
            foreach (var option in _selectedOptions)
            {
                option.OnChange += ValidateSelection;
            }
        }

        private bool TryValidateSelection()
        {
            if (_attachedPanel == null)
            {
                return false;
            }
            bool isValid = true;
            SelectedTactic[] nonNullTactics = _selectedOptions.Where(t => t.TacticData != null).ToArray();
            if (nonNullTactics.Length != _selectedOptions.Length) { isValid = false; }
            foreach (SelectedTactic selectedTactic in nonNullTactics)
            {
                IEnumerable<ITacticEffect> otherTactics = nonNullTactics.Where(t => t != selectedTactic).Select(t => t.TacticData.Effect);
                isValid &= selectedTactic.TacticData.Effect.TryValidate(_attachedPanel.FigureController.Figure, otherTactics, out string result);
                selectedTactic.ResultLabel.text = result;
            }
            return isValid;
        }

        private void ValidateSelection(SelectedTactic _)
        {
            _confirmButton.Enabled = TryValidateSelection();
            _confirmButton.Tooltip.Tooltip = _confirmButton.Enabled ? BeginTurnText : SelectTacticsText;
        }

        public void Select(TacticData data)
        {
            SelectedTactic option = _selectedOptions.FirstOrDefault(o => o.TacticData == null);
            if (option != null)
            {
                option.TacticData = data;
            }
            else
            {
                _selectedOptions[_selectedOptions.Length - 1].TacticData = data;
            }
        }

        public void Clear()
        {
            foreach (var option in _selectedOptions) { option.Clear(); }
            _confirmButton.Enabled = false;
        }

        public void SelectAndShow(HeroFigurePanel targetHeroPanel)
        {
            Clear();
            if (_attachedPanel != null) 
            { 
                _attachedPanel.OnMoved -= AttachToPanel;
                _attachedPanel.FigureController.Figure.EntityData.OnChanged -= HandleEntityChanged;
            }
            _attachedPanel = targetHeroPanel;
            _attachedPanel.OnMoved += AttachToPanel;
            _attachedPanel.FigureController.Figure.EntityData.OnChanged += HandleEntityChanged;
            AttachToPanel(targetHeroPanel);
        }

        private void HandleEntityChanged(LivingEntityChangeEvent @event) => TryValidateSelection();

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

        public void Cancel()
        {
            if (_attachedPanel != null) { _attachedPanel.OnMoved -= AttachToPanel; }
            _attachedPanel = null;
            Hide();
        }

        public void Confirm() => _encounterController.BeginTurn(_attachedPanel.FigureController, _selectedOptions.Select(o => o.TacticData));
    }
}