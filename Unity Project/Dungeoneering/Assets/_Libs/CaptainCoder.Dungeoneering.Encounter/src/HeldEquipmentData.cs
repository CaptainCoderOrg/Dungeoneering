using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/Weapon Data")]
    public sealed class HeldEquipmentData : EquipmentData
    {
        [field: SerializeField] public int Hands { get; private set; }
        [field: SerializeField] public AttackData Attack { get; private set; }
    }
}