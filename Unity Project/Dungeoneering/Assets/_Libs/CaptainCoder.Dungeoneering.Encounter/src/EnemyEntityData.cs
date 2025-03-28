using System.Collections.Generic;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/Enemy Entity Data")]
    public class EnemyEntityData : LivingEntityData
    {
        [field: SerializeField] public List<FigureAbilityData> Abilities { get; private set; }
        [field: SerializeField] public List<AttackData> Attacks { get; private set; }
    }
}