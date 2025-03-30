using System;
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
        [field: SerializeField] public HeldEquipmentData LeftHand { get; private set; }
        [field: SerializeField] public HeldEquipmentData RightHand { get; private set; }
        [field: SerializeField] public WornEquipmentData WornArmor { get; private set; }
        [field: SerializeField] public AccessoryEquipmentData Accessory { get; private set; }
        [field: SerializeField] public List<EquipmentData> Inventory { get; private set; }
    }
}