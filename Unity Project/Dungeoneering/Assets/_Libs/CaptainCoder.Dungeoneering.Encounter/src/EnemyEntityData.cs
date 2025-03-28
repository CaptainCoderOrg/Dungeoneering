using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/Enemy Entity Data")]
    public class EnemyEntityData : LivingEntityData
    {
        [field: SerializeField] public List<FigureAbilityData> Abilities { get; private set; }
        [field: SerializeField] public List<AttackData> Attacks { get; private set; }

        internal EnemyEntityData Copy()
        {
            EnemyEntityData copy = CreateInstance<EnemyEntityData>();
            CopyTo(this, copy);
            copy.Abilities = Abilities.ToList();
            copy.Attacks = Attacks.ToList();
            return copy;
        }
    }
}