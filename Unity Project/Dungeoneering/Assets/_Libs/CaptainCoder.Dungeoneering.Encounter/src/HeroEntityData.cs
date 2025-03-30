using System.Collections.Generic;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/Hero Entity Data")]
    public class HeroEntityData : LivingEntityData
    {
        [field: SerializeField] public int BaseStamina { get; private set; }
        [field: SerializeField] public int Exertion { get; private set; }
        public int Stamina => BaseStamina - Exertion;
        [field: SerializeField] public HeldEquipmentData LeftHand { get; set; }
        [field: SerializeField] public HeldEquipmentData RightHand { get; set; }
        [field: SerializeField] public WornEquipmentData WornArmor { get; set; }
        [field: SerializeField] public AccessoryEquipmentData Accessory { get; set; }
        [field: SerializeField] public List<EquipmentData> Inventory { get; private set; }
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

        public override void OnBeforeEnterPlayMode()
        {
            base.OnBeforeEnterPlayMode();
            _backpackReferences = null;
            _leftHandSlot = null;
            _rightHandSlot = null;
            _wornSlot = null;
            _accessorySlot = null;
        }
    }
}