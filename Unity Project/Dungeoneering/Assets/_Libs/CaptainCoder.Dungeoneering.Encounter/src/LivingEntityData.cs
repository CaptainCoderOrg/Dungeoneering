using System.Collections.Generic;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/Living Entity Data")]
    public class LivingEntityData : ObservableSO
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Portrait { get; private set; }
        [field: SerializeField] public int BaseHealth { get; private set; }
        [field: SerializeField] public int Wounds { get; private set; }
        public int Health => BaseHealth - Wounds;
        [field: SerializeField] public int Speed { get; private set; }
        [field: SerializeField] public int Armor { get; private set; }
        [field: SerializeField] public AnimationData SpawnAnimation { get; private set; }
        [field: SerializeField] public AnimationData AttackAnimation { get; private set; }
        [field: SerializeField] public AnimationData IdleAnimation { get; private set; }
    }
}