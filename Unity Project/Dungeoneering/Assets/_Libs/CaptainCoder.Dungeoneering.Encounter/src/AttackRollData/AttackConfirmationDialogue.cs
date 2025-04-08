using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Unity.Assertions;

using NaughtyAttributes;

using TMPro;

using UnityEngine;

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
        [AssertIsSet][SerializeField] private ToggleablePanel _attackInformation;
        [AssertIsSet][SerializeField] private ToggleablePanel _targetInformation;
        [AssertIsSet][SerializeField] private IconButton _confirmButton;
        [AssertIsSet][SerializeField] private IconButton _cancelButton;
        [AssertIsSet][SerializeField] private AttackIconController _attackIcon;
        [AssertIsSet][SerializeField] private DieIconController[] _dieIcons;
        [SerializeField] private FigureData _figureData;
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
                }
                else
                {
                    _selectAttackLabel.Hide();
                    _attackInformation.Show();
                    EncounterController.HeroTurnController.ShowPossibleAttacks(Attacker, Attack);
                    RenderAttackDice();
                }
            }
        }

        [SerializeField] private FigureData _targetData;
        public FigureData Target
        {
            get => _targetData;
            set
            {
                _targetData = value;
                if (_targetData == null)
                {
                    _selectATargetLabel.Show();
                    _targetInformation.Hide();
                }
                else
                {
                    _selectATargetLabel.Hide();
                    _targetInformation.Show();
                }
            }
        }

        void Awake()
        {
            _encounterController = GetComponentInParent<EncounterController>();
        }

        private void RenderAttackDice()
        {
            _attackNameLabel.text = Attack.Name;
            _attackIcon.AttackType = Attack.AttackType;
            _dieIcons[0].Die = Attack.AttackType.AttackDie;
            _dieIcons[0].CanvasGroupHider.Show();
            int ix = 1;
            foreach (var die in Attack.PowerDice)
            {
                _dieIcons[ix].Die = die;
                _dieIcons[ix].CanvasGroupHider.Show();
                ix++;
            }
            HeroEntityData hero = (HeroEntityData)Attacker.EntityData;
            foreach (var die in hero.GetDice(Attack))
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
        internal void ClearTarget() => Target = null;
    }
}