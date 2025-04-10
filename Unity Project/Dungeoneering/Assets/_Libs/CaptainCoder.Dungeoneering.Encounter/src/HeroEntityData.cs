using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/Hero Entity Data")]
    public sealed class HeroEntityData : LivingEntityData
    {
        [field: SerializeField] public int BaseStamina { get; private set; }
        [field: SerializeField] public int Exertion { get; set; }
        public int MaxStamina => BaseStamina + TraitEffects().Where(te => te.TraitType == TraitDatabase.StaminaTrait).Sum(te => te.Value);
        public override IEnumerable<TraitEffect> TraitEffects() => base.TraitEffects().Concat(Equipped().SelectMany(e => e.WornTraitEffects));
        public IEnumerable<EquipmentData> Equipped()
        {
            if (LeftHand != null) yield return LeftHand;
            if (RightHand != null) yield return RightHand;
            if (WornArmor != null) yield return WornArmor;
            if (Accessory != null) yield return Accessory;
        }
        public int Stamina => MaxStamina - Exertion;
        [field: SerializeField] private HeldEquipmentData _leftHand;
        public HeldEquipmentData LeftHand
        {
            get => _leftHand;
            set
            {
                _leftHand = value;
                base.Notify(EquipmentChangedEvent.Instance);
            }
        }
        [field: SerializeField] private HeldEquipmentData _rightHand;
        public HeldEquipmentData RightHand
        {
            get => _rightHand;
            set
            {
                _rightHand = value;
                base.Notify(EquipmentChangedEvent.Instance);
            }
        }
        [field: SerializeField] private WornEquipmentData _wornArmor;
        public WornEquipmentData WornArmor
        {
            get => _wornArmor;
            set
            {
                _wornArmor = value;
                base.Notify(EquipmentChangedEvent.Instance);
            }
        }
        [field: SerializeField] private AccessoryEquipmentData _accessory;
        public AccessoryEquipmentData Accessory
        {
            get => _accessory;
            set
            {
                _accessory = value;
                base.Notify(EquipmentChangedEvent.Instance);
            }
        }
        [field: SerializeField] public List<EquipmentData> Inventory { get; private set; }
        [field: SerializeField] public List<DieData> MeleeSkillDice { get; set; }
        [field: SerializeField] public List<DieData> RangeSkillDice { get; set; }
        [field: SerializeField] public List<DieData> MagicSkillDice { get; set; }
        private BackpackSlotReference[] _backpackReferences;
        public BackpackSlotReference[] BackpackReferences => _backpackReferences ??= new BackpackSlotReference[] { new(this, 0), new(this, 1), new(this, 2), new(this, 3) };
        private LeftHandSlotReference _leftHandSlot;
        public LeftHandSlotReference LeftHandSlot => _leftHandSlot ??= new(this);
        private RightHandSlotReference _rightHandSlot;
        public RightHandSlotReference RightHandSlot => _rightHandSlot ??= new(this);
        private WornEquipmentSlotReference _wornSlot;
        public WornEquipmentSlotReference WornSlot => _wornSlot ??= new(this);
        private AccessorySlotReference _accessorySlot;
        public AccessorySlotReference AccessorySlot => _accessorySlot ??= new(this);

        public IEnumerable<DieData> GetDice(AttackData attack)
        {
            if (attack.AttackType == attack.AttackTypeDatabase.Melee)
            {
                return MeleeSkillDice;
            }
            else if (attack.AttackType == attack.AttackTypeDatabase.Range)
            {
                return RangeSkillDice;
            }
            else if (attack.AttackType == attack.AttackTypeDatabase.Magic)
            {
                return MagicSkillDice;
            }
            Debug.LogWarning($"Could not determine skill dice", attack);
            throw new System.Exception($"Could not determin skill dice.");
        }


        public override void OnBeforeEnterPlayMode()
        {
            base.OnBeforeEnterPlayMode();
            _backpackReferences = null;
            _leftHandSlot = null;
            _rightHandSlot = null;
            _wornSlot = null;
            _accessorySlot = null;
        }

        internal override string TraitValueText(TraitTypeData traitTypeData)
        {
            if (traitTypeData == TraitDatabase.StaminaTrait)
            {
                return $"{Stamina}/{MaxStamina}";
            }
            if (traitTypeData == TraitDatabase.HealthTrait)
            {
                return $"{Health}/{MaxHealth}";
            }
            if (traitTypeData == TraitDatabase.ArmorTrait)
            {
                return Armor.ToString();
            }
            if (traitTypeData == TraitDatabase.SpeedTrait)
            {
                return Speed.ToString();
            }
            Debug.LogError($"Entity {this} does not have the trait {traitTypeData}", this);
            return null;
        }

        internal override string TraitDetails(TraitTypeData traitTypeData)
        {
            if (traitTypeData == TraitDatabase.StaminaTrait)
            {
                return TraitDetails(BaseStamina, TraitDatabase.StaminaTrait, TraitEffects());
            }
            if (traitTypeData == TraitDatabase.HealthTrait)
            {
                return TraitDetails(BaseHealth, TraitDatabase.HealthTrait, TraitEffects());
            }
            if (traitTypeData == TraitDatabase.ArmorTrait)
            {
                return TraitDetails(BaseArmor, TraitDatabase.ArmorTrait, TraitEffects());
            }
            if (traitTypeData == TraitDatabase.SpeedTrait)
            {
                return TraitDetails(BaseSpeed, TraitDatabase.SpeedTrait, TraitEffects());
            }
            Debug.LogError($"Entity {this} does not have the trait {traitTypeData}", this);
            return null;
        }
    }
}