using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/Equipment/Held Equipment Data")]
    public sealed class HeldEquipmentData : EquipmentData
    {
        [field: SerializeField] public bool IsTwoHanded { get; private set; }
        [field: SerializeField] public AttackData Attack { get; private set; }
    }
}