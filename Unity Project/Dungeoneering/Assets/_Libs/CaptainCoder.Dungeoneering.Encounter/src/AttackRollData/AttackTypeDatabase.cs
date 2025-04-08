using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/AttackTypeDatabase")]
    public class AttackTypeDatabase : ObservableSO
    {
        [field: SerializeField] public AttackTypeData Melee { get; private set; }
        [field: SerializeField] public AttackTypeData Range { get; private set; }
        [field: SerializeField] public AttackTypeData Magic { get; private set; }
    }
}