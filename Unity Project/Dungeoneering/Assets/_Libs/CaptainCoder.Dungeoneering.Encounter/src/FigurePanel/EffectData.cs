using NaughtyAttributes;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/Effect Data")]
    public sealed class EffectData : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [TextArea(3, 5)][SerializeField] private string _description;
        public string Description => _description;
        [field: ShowAssetPreview][field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public TraitEffect[] TraitEffects { get; private set; }

#if UNITY_EDITOR
        void OnValidate()
        {
            for (int ix = 0; ix < TraitEffects.Length; ix++)
            {
                TraitEffects[ix].Source = Name;
            }
        }
#endif
    }
}