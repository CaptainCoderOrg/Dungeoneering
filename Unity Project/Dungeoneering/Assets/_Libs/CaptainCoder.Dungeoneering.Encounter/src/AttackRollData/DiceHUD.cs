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
        [SerializeField] private EncounterController _encounterController;
        [AssertIsSet][SerializeField] private DieData _bonusDie;
        [AssertIsSet][SerializeField] private DiceBoxController _diceBoxController;
        [AssertIsSet][SerializeField] private ToggleablePanel _toggleablePanel;
        [AssertIsSet][SerializeField] private DieFaceRenderer[] _dieFaceRenderers;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _damageLabel;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _accuracyLabel;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _powerLabel;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _splitLabel;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _resultLabel;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _attackingLabel;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _staminaLabel;
        [AssertIsSet][SerializeField] private CanvasGroup _staminaButton;
        [AssertIsSet][SerializeField] private CanvasGroup _increaseAttackButton;
        [AssertIsSet][SerializeField] private CanvasGroup _decreaseAttackButton;
        [AssertIsSet][SerializeField] private CanvasGroup _increaseAccuracyButton;
        [AssertIsSet][SerializeField] private CanvasGroup _decreaseAccuracyButton;

        private bool _isMiss = false;
        private int _damage;
        private int _damageBonus;
        private int _accuracy;
        private int _accuracyBonus;
        public int TotalAccuracy => _accuracy + _accuracyBonus;
        private int _power;
        private int _powerSpent;
        private int _bonusTotal;
        private int RemainingBonus => _bonusTotal - _accuracyBonus - _damageBonus;
        private readonly List<DieResult> _dice = new();
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
        public bool CanExert => !_isMiss && _attacker.EntityData is HeroEntityData hero && hero.Stamina > 0 && _dice.Count < 12;
        public HeroEntityData HeroAttacker => (HeroEntityData)_attacker.EntityData;

        public bool CanAddDamageBonus => !_isMiss && RemainingBonus > 0;
        public bool CanRemoveDamageBonus => !_isMiss && _damageBonus > 0;
        public bool CanAddAccuracyBonus => !_isMiss && RemainingBonus > 0;
        public bool CanRemoveAccuracyBonus => !_isMiss && _accuracyBonus > 0;
        private AttackResult _attackResult;

        void Awake()
        {
            _encounterController = GetComponentInParent<EncounterController>();
            _diceBoxController.OnResult += HandleDiceResults;
        }

        private void HandleDiceResults(IEnumerable<DieResult> result)
        {
            _dice.Clear();
            _dice.AddRange(result);
            _damageBonus = 0;
            _accuracyBonus = 0;
            _powerSpent = 0;
            _damage = 0;
            _accuracy = 0;
            _power = 0;
            _bonusTotal = 0;
            _isMiss = false;
            RenderDice(result);
            UpdateLabels();
            Rebuild();
        }

        private void UpdateLabels()
        {
            _attackResult = CalculateResult();
            _damageLabel.text = $"{_damage + _damageBonus}";
            _accuracyLabel.text = $"{_accuracy + _accuracyBonus}";
            _powerLabel.text = $"{_power - _powerSpent}/{_power}";
            _splitLabel.text = $"{RemainingBonus}/{_bonusTotal}";
            _resultLabel.text = _attackResult.Message;
            if (_attacker.EntityData is HeroEntityData)
            {
                _staminaLabel.text = $"{HeroAttacker.Stamina}";
                _staminaButton.alpha = CanExert ? 1 : 0.5f;
            }
            else
            {
                Debug.LogWarning("TODO: Hide stamina element when not a hero");
            }

            _increaseAccuracyButton.alpha = CanAddAccuracyBonus ? 1 : 0.5f;
            _increaseAttackButton.alpha = CanAddDamageBonus ? 1 : 0.5f;
            _decreaseAttackButton.alpha = CanRemoveDamageBonus ? 1 : 0.5f;
            _decreaseAccuracyButton.alpha = CanRemoveAccuracyBonus ? 1 : 0.5f;
        }

        private AttackResult CalculateResult()
        {
            if (_isMiss)
            {
                return new AttackResult("X = MISS!", 0);
            }
            if (TotalAccuracy < _attackInfo.Distance)
            {
                return new AttackResult($"MISS - Requires {_attackInfo.Distance}<sprite name=\"accuracy\">", 0);
            }
            int armor = _attackInfo.Target.Figure.EntityData.Armor;
            int totalDamage = _damage + _damageBonus;
            int wounds = Mathf.Max(0, totalDamage - armor);
            return new AttackResult($"{totalDamage}<sprite name=\"melee\"/> - {armor}<sprite name=\"armor\"> = {wounds} WOUNDS", wounds);
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
            if (!CanAddDamageBonus) { return; }
            _damageBonus++;
            UpdateLabels();
        }

        public void RemoveDamageBonus()
        {
            if (!CanRemoveDamageBonus) { return; }
            _damageBonus--;
            UpdateLabels();
        }

        public void AddAccuracyBonus()
        {
            if (!CanAddAccuracyBonus) { return; }
            _accuracyBonus++;
            UpdateLabels();
        }

        public void RemoveAccuracyBonus()
        {
            if (!CanRemoveAccuracyBonus) { return; }
            _accuracyBonus--;
            UpdateLabels();
        }

        public void Exert()
        {
            if (CanExert)
            {
                HeroAttacker.Exertion++;
                _diceBoxController.AddDie(_bonusDie);
                UpdateLabels();
            }
        }

        public void Confirm()
        {
            _attackInfo.Target.Figure.EntityData.Wounds += _attackResult.Wounds;
            _encounterController.HeroTurnController.CloseAttackPanel();
            Hide();
        }
    }

    public record struct AttackResult(string Message, int Wounds);
}