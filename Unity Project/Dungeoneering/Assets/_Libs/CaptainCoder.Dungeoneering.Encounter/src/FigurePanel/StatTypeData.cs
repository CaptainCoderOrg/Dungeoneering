using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/Stat/Trait Type Data")]
    public class TraitTypeData : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
    }
}