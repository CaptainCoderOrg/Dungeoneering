using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/Stat/Trait Type Data")]
    public class TraitTypeData : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string IconName { get; private set; }
        private string _spriteTag;
        public string SpriteTag => _spriteTag ??= $"<sprite name=\"{IconName}\"/>";

        void OnValidate()
        {
            _spriteTag = $"<sprite name=\"{IconName}\"/>";
        }
    }
}