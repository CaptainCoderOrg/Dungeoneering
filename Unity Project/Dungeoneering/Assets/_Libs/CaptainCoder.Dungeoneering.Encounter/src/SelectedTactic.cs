using System;
using System.Collections.Generic;

using CaptainCoder.Unity.Assertions;

using NaughtyAttributes;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CaptainCoder.Dungeoneering.Encounter
{
    public class SelectedTactic : MonoBehaviour, IPointerClickHandler
    {

        [SerializeField] private TacticData _preperationData;
        const string DefaultTooltip = "<u>Tactic</u>\nClick a tactic to select it";
        const string DefaultResultLabel = "Select a Tactic";
        public TacticData TacticData
        {
            get => _preperationData;
            set
            {
                _preperationData = value;
                if (_preperationData == null) { Clear(); }
                else { Render(); }
                OnChange?.Invoke(this);
            }
        }
        [AssertIsSet][SerializeField] private Image _icon;
        [AssertIsSet][SerializeField] private SimpleTooltip _simpleTooltip;
        [AssertIsSet][SerializeField] private Image _background;
        [AssertIsSet][field: SerializeField] public TextMeshProUGUI ResultLabel { get; private set; }
        public event System.Action<SelectedTactic> OnChange;

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
            ResultLabel.text = DefaultResultLabel;
            OnChange?.Invoke(this);
        }

        [Button]
        public void Render()
        {
            _icon.enabled = true;
            _icon.sprite = TacticData.Sprite;
            _simpleTooltip.Tooltip = $"{_preperationData.Tooltip}\n\n<b>Click to Clear</b>";
        }

        public void OnPointerClick(PointerEventData eventData) => Clear();
    }
}