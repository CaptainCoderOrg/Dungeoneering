using System.Collections;
using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Unity.Assertions;

using NaughtyAttributes;

using TMPro;

using UnityEngine;
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
        [AssertIsSet][SerializeField] private TextMeshProUGUI _resultLabel;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _attackingLabel;
        [AssertIsSet][SerializeField] private CanvasGroup _increaseAttackButton;
        [AssertIsSet][SerializeField] private CanvasGroup _decreaseAttackButton;
        [AssertIsSet][SerializeField] private CanvasGroup _increaseAccuracyButton;
        [AssertIsSet][SerializeField] private CanvasGroup _decreaseAccuracyButton;

        private bool _isMiss = false;
        private int _damage;
        private int _damageBonus;
        private int _accuracy;
        private int _accuracyBonus;
        private int _power;
        private int _powerSpent;
        private int _bonusTotal;
        private int RemainingBonus => _bonusTotal - _accuracyBonus - _damageBonus;
        private AttackInfo _attackInfo;
        public AttackInfo AttackInfo
        {
            get => _attackInfo;
            set
            {
                _attackInfo = value;
                _attackingLabel.text = $"Attacking {_attackInfo.Target.Figure.EntityData.Name}";
            }
        }
        private FigureData _attacker;
        public FigureData Attacker
        {
            get => _attacker;
            set
            {
                _attacker = value;
            }
        }

        private AttackData _attack;
        public AttackData Attack
        {
            get => _attack;
            set
            {
                _attack = value;
            }
        }


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
            _isMiss = false;
            RenderDice(result);
            UpdateLabels();
            Rebuild();
        }

        private void UpdateLabels()
        {
            _damageLabel.text = $"{_damage + _damageBonus}";
            _accuracyLabel.text = $"{_accuracy + _accuracyBonus}";
            _powerLabel.text = $"{_power - _powerSpent}/{_power}";
            _splitLabel.text = $"{RemainingBonus}/{_bonusTotal}";
            _resultLabel.text = CalculateResultLabel();

            if (RemainingBonus > 0)
            {
                _increaseAttackButton.alpha = 1;
                _increaseAccuracyButton.alpha = 1;
            }
            else
            {
                _increaseAttackButton.alpha = 0.5f;
                _increaseAccuracyButton.alpha = 0.5f;
            }

            _decreaseAttackButton.alpha = _damageBonus > 0 ? 1 : 0.5f;
            _decreaseAccuracyButton.alpha = _accuracyBonus > 0 ? 1 : 0.5f;
        }

        private string CalculateResultLabel()
        {
            if (_isMiss)
            {
                return "X = MISS!";
            }
            if (_accuracy < _attackInfo.Distance)
            {
                return $"MISS - Requires {_attackInfo.Distance}<sprite name=\"accuracy\">";
            }
            int armor = _attackInfo.Target.Figure.EntityData.Armor;
            int totalDamage = _damage + _damageBonus;
            int wounds = Mathf.Max(0, totalDamage - armor);
            return $"{totalDamage}<sprite name=\"melee\"/> - {armor}<sprite name=\"armor\"> = {wounds} WOUNDS";
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
                _isMiss = _isMiss || die.IsMiss;
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
            _resultLabel.text = "Rolling...";
            _damageLabel.text = "?";
            _accuracyLabel.text = $"?";
            _powerLabel.text = "?/?";
            _splitLabel.text = "?/?";
            _diceBoxController.Roll();
        }

        public void AddDamageBonus()
        {
            if (RemainingBonus <= 0) { return; }
            _damageBonus++;
            UpdateLabels();
        }

        public void RemoveDamageBonus()
        {
            if (_damageBonus <= 0) { return; }
            _damageBonus--;
            UpdateLabels();
        }

        public void AddAccuracyBonus()
        {
            if (RemainingBonus <= 0) { return; }
            _accuracyBonus++;
            UpdateLabels();
        }

        public void RemoveAccuracyBonus()
        {
            if (_accuracyBonus <= 0) { return; }
            _accuracyBonus--;
            UpdateLabels();
        }

        public void Exert()
        {

        }

        public void Confirm()
        {
            Debug.LogWarning("Confirm not implemented!");
            Hide();
        }
    }
}