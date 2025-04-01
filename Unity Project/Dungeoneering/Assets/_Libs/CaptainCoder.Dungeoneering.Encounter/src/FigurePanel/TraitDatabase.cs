using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/Stat/Trait Database")]
    public class TraitDatabase : ScriptableObject
    {
        [field: SerializeField] public TraitTypeData StaminaTrait { get; private set; }
        [field: SerializeField] public TraitTypeData HealthTrait { get; private set; }
        [field: SerializeField] public TraitTypeData SpeedTrait { get; private set; }
        [field: SerializeField] public TraitTypeData ArmorTrait { get; private set; }
    }
}