using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/StatusData")]
    public class StatusData : ObservableSO
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public EffectData Effect { get; private set; }
    }
}