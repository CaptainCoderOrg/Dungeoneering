

namespace CaptainCoder.Dungeoneering.Encounter
{
    [System.Serializable]
    public struct TraitEffect
    {
        public TraitTypeData TraitType;
        public int Value;
        public string Source;
    }

    public record struct TraitEffectWithSource(string Source, TraitEffect Effect);
}