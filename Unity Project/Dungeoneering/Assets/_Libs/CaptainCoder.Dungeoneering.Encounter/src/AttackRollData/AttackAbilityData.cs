using NaughtyAttributes;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{

    [CreateAssetMenu(menuName = "DC/Attack Ability Data")]
    public class AttackAbilityData : ObservableSO, IAttackAbility
    {
        [field: ShowAssetPreview][field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: TextArea(3, 5)][field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public int PowerCost { get; private set; }
        [field: SerializeField] public int DamageBonus { get; private set; }
        [field: SerializeField] public int AccuracyBonus { get; private set; }
    }

}