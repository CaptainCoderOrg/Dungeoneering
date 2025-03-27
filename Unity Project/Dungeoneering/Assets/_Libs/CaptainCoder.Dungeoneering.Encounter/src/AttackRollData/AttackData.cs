using System.Collections.Generic;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/AttackData")]
    public class AttackData : ObservableSO
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public AttackTypeData AttackType { get; private set; }
        [field: SerializeField] public List<DieData> PowerDice { get; private set; }
    }
}