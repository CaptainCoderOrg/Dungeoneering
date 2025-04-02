using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public sealed class EffectsRenderer : MonoBehaviour, ILivingEntityRenderer
    {
        [AssertIsSet][SerializeField] private EffectIconRenderer[] _renderers;

        public void Render(LivingEntityData data)
        {
            for (int ix = 0; ix < _renderers.Length; ix++)
            {
                var renderer = _renderers[ix];
                if (ix >= data.Effects.Count) { renderer.Hide(); }
                else { renderer.Render(data.Effects[ix]); }
            }
        }
    }
}