using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EquipmentData : ObservableSO
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite Portrait { get; private set; }
        [field: SerializeField] public int Value { get; private set; }
        [field: SerializeField] public int ArmorBonus { get; private set; }
        [field: SerializeField] public int SpeedBonus { get; private set; }
        [field: SerializeField] public int StaminaBonus { get; private set; }
    }
}