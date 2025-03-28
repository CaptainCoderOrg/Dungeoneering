using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/Figure Ability Data")]
    public class FigureAbilityData : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [TextArea(3, 5)][SerializeField] private string _toolTip;
        public string Tooltop => _toolTip;
    }
}