using System;
using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Unity.Assertions;

using NaughtyAttributes;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace CaptainCoder.Dungeoneering.Encounter
{
    public class AttackConfirmationDialogue : MonoBehaviour
    {
        [SerializeField] private EncounterController _encounterController;
        public EncounterController EncounterController => _encounterController ??= GetComponentInParent<EncounterController>();
        [AssertIsSet][SerializeField] private ToggleablePanel _toggleablePanel;
        [AssertIsSet][SerializeField] private ToggleablePanel _selectAttackLabel;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _attackNameLabel;
        [AssertIsSet][SerializeField] private ToggleablePanel _selectATargetLabel;
        [AssertIsSet][SerializeField] private ToggleablePanel _invalidTargetLabel;
        [AssertIsSet][SerializeField] private ToggleablePanel _attackInformation;
        [AssertIsSet][SerializeField] private ToggleablePanel _targetInformation;
        [AssertIsSet][SerializeField] private IconButton _confirmButton;
        [AssertIsSet][SerializeField] private IconButton _cancelButton;
        [AssertIsSet][SerializeField] private AttackIconController _attackIcon;
        [AssertIsSet][SerializeField] private DieIconController[] _dieIcons;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _nameLabel;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _healthLabel;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _armorLabel;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _aimLabel;
        [SerializeField] private FigureData _figureData;
        private readonly List<DieData> _attackDice = new();
        public FigureData Attacker
        {
            get => _figureData;
            set
            {
                _figureData = value;
                SelectAttack();
            }
        }
        [SerializeField] private AttackData _attackData;
        public AttackData Attack
        {
            get => _attackData;
            set
            {
                _attackData = value;
                if (_attackData == null)
                {
                    _selectAttackLabel.Show();
                    _attackInformation.Hide();
                    EncounterController.HeroTurnController.OnAttackTargetSelected -= HandleTargetChanged;
                }
                else
                {
                    _selectAttackLabel.Hide();
                    _attackInformation.Show();
                    EncounterController.HeroTurnController.ShowPossibleAttacks(Attacker, Attack);
                    EncounterController.HeroTurnController.OnAttackTargetSelected += HandleTargetChanged;
                    _attackDice.Clear();
                    _attackDice.AddRange(CalculateAttackDice((HeroEntityData)Attacker.EntityData, _attackData));
                    RenderAttackDice(_attackDice);
                }
            }
        }

        private void HandleTargetChanged(AttackTargetSelectedEvent @event) => AttackInfo = @event;

        private AttackInfo _attackInfo;

        private AttackTargetSelectedEvent _lastAttackEvent;
        public AttackTargetSelectedEvent AttackInfo
        {
            get => _lastAttackEvent;
            set
            {
                _lastAttackEvent = value;
                Render(_lastAttackEvent);
            }
        }

        private void Render(AttackTargetSelectedEvent attackInfo)
        {
            if (_lastAttackEvent == null || _lastAttackEvent is NoAttackTargetSelected)
            {
                _invalidTargetLabel.Hide();
                _selectATargetLabel.Show();
                _targetInformation.Hide();
                _confirmButton.Enabled = false;
            }
            else if (_lastAttackEvent is ValidAttackTargetSelected valid)
            {
                _attackInfo = valid.Attack;
                _invalidTargetLabel.Hide();
                _selectATargetLabel.Hide();
                _targetInformation.Show();
                LivingEntityData entity = valid.Attack.Target.Figure.EntityData;
                _nameLabel.text = entity.Name;
                _healthLabel.text = $"{entity.Health}/{entity.MaxHealth}";
                _armorLabel.text = entity.Armor.ToString();
                _aimLabel.text = valid.Attack.Distance.ToString();
                _confirmButton.Enabled = true;
            }
            else if (_lastAttackEvent is InvalidAttackTargetSelected)
            {
                _selectATargetLabel.Show();
                _invalidTargetLabel.Show();
                _targetInformation.Hide();
                _confirmButton.Enabled = false;
            }
            else
            {
                throw new Exception($"Unexpected event: {attackInfo}");
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
        }

        void Awake()
        {
            _encounterController = GetComponentInParent<EncounterController>();
        }

        private static IEnumerable<DieData> CalculateAttackDice(HeroEntityData attacker, AttackData attack)
        {
            yield return attack.AttackType.AttackDie;
            foreach (var die in attack.PowerDice)
            {
                yield return die;
            }
            foreach (var die in attacker.GetDice(attack))
            {
                yield return die;
            }
        }

        private void RenderAttackDice(IEnumerable<DieData> dice)
        {
            _attackNameLabel.text = Attack.Name;
            _attackIcon.AttackType = Attack.AttackType;
            int ix = 0;
            foreach (var die in dice)
            {
                _dieIcons[ix].Die = die;
                _dieIcons[ix].CanvasGroupHider.Show();
                ix++;
            }
            for (; ix < _dieIcons.Length; ix++)
            {
                _dieIcons[ix].CanvasGroupHider.Hide();
            }
        }

        private IEnumerable<AttackData> PossibleAttacks()
        {
            if (Attacker.EntityData is HeroEntityData hero)
            {
                if (hero.LeftHand != null && hero.LeftHand.Attack != null)
                {
                    yield return hero.LeftHand.Attack;
                }

                if (hero.RightHand != null && hero.RightHand.Attack != null)
                {
                    yield return hero.RightHand.Attack;
                }

            }
            else
            {
                Debug.Log("Not a hero entity");
            }
        }

        [Button]
        public void SelectAttack()
        {
            var attacks = PossibleAttacks().ToHashSet();
            if (attacks.Count == 0)
            {
                Debug.LogWarning("Unarmed attack not implemented", this);
            }
            else if (attacks.Count == 1)
            {
                Attack = attacks.First();
            }
            else
            {
                Debug.LogWarning("Show attack selection not implemented", this);
            }
        }

        internal void Show() => _toggleablePanel.IsEnabled = true;
        internal void Hide() => _toggleablePanel.IsEnabled = false;
        internal void ClearTarget() => AttackInfo = null;

        public void CancelAttack()
        {
            Hide();
            EncounterController.HeroTurnController.CancelAttack();
        }

        public void ConfirmAttack()
        {
            Hide();
            EncounterController.HeroTurnController.ConfirmAttack(Attacker, Attack, _attackInfo, _attackDice);
        }
    }
}