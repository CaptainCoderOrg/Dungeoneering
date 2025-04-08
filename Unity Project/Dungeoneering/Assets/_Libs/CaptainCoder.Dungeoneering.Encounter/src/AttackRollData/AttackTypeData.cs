using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/AttackTypeData")]
    public class AttackTypeData : ObservableSO
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public bool RequiresLineOfSight { get; private set; }
        [field: SerializeField] public bool IsRanged { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public DieData AttackDie { get; private set; }
        [TextArea(3, 5)][SerializeField] private string _tooltip;
        public string Tooltip => _tooltip;
    }
}