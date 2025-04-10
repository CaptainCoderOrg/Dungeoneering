using System.Collections;
using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Unity.Assertions;

using NaughtyAttributes;

using TMPro;

using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace CaptainCoder.Dungeoneering.Encounter
{
    public class DiceHUD : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private DiceBoxController _diceBoxController;
        [AssertIsSet][SerializeField] private ToggleablePanel _toggleablePanel;
        [AssertIsSet][SerializeField] private DieFaceRenderer[] _dieFaceRenderers;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _damageLabel;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _accuracyLabel;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _powerLabel;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _splitLabel;

        private int _damage;
        private int _damageBonus;
        private int _accuracy;
        private int _accuracyBonus;
        private int _power;
        private int _powerSpent;
        private int _bonusTotal;
        private int _requiredAccuracy;


        void Awake()
        {
            _diceBoxController.OnResult += HandleDiceResults;
        }

        private void HandleDiceResults(IEnumerable<DieResult> result)
        {
            _damage = 0;
            _damageBonus = 0;
            _accuracy = 0;
            _accuracyBonus = 0;
            _power = 0;
            _powerSpent = 0;
            _bonusTotal = 0;
            RenderDice(result);
            UpdateLabels();
            Rebuild();
        }

        private void UpdateLabels()
        {
            _damageLabel.text = $"{_damage + _damageBonus}";
            _accuracyLabel.text = $"{_accuracy + _accuracyBonus}";
            _powerLabel.text = $"{_power - _powerSpent}/{_power}";
            _splitLabel.text = $"{_bonusTotal - _damageBonus - _accuracyBonus}/{_bonusTotal}";
        }

        private void RenderDice(IEnumerable<DieResult> result)
        {
            int ix = 0;
            foreach (DieResult die in result)
            {
                _damage += die.Damage;
                _accuracy += die.Accuracy;
                _power += die.Power;
                _bonusTotal += die.Split;
                DieFaceRenderer renderer = _dieFaceRenderers[ix++];
                renderer.RenderDieFace(die.Die, die.Face);
                renderer.Show();
            }
            for (; ix < _dieFaceRenderers.Length; ix++)
            {
                DieFaceRenderer renderer = _dieFaceRenderers[ix++];
                renderer.Hide();
            }
        }

        [Button]
        private void Rebuild()
        {
            StopAllCoroutines();
            StartCoroutine(RebuildAtEndOfFrame());
        }

        private IEnumerator RebuildAtEndOfFrame()
        {
            for (int ix = 0; ix < 5; ix++)
            {
                yield return null;
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
            }
        }

        public void SetRequiredAccuracy(int requiredAccuracy)
        {
            _requiredAccuracy = requiredAccuracy;
        }

        public void SetDice(IEnumerable<DieData> dice)
        {
            _diceBoxController.SetDice(dice);
            RenderDice(dice.Select(d => new DieResult(d, 0)));
        }

        public void Show()
        {
            _toggleablePanel.IsEnabled = true;
            Rebuild();
        }
        public void Hide() => _toggleablePanel.IsEnabled = false;
        internal void Roll()
        {
            _damageLabel.text = "?";
            _accuracyLabel.text = $"?";
            _powerLabel.text = "?/?";
            _splitLabel.text = "?/?";
            _diceBoxController.Roll();
        }
    }
}