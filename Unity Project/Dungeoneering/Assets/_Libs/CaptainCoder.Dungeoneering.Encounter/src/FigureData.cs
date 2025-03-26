using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    public class FigureData : ObservableSO
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public AnimationData SpawnAnimation { get; private set; }
        [field: SerializeField] public AnimationData AttackAnimation { get; private set; }
        [field: SerializeField] public AnimationData IdleAnimation { get; private set; }
    }
}