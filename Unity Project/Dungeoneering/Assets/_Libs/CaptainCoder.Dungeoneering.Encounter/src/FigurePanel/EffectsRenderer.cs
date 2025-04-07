using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public sealed class EffectsRenderer : MonoBehaviour, ILivingEntityRenderer
    {
        [AssertIsSet][SerializeField] private EffectIconRenderer[] _renderers;
        private LivingEntityData _entity;
        public LivingEntityData Entity
        {
            get => _entity;
            private set
            {
                if (_entity != null) { _entity.OnChanged -= HandleChange; }
                _entity = value;
                _entity.OnChanged += HandleChange;
                Render();
            }
        }

        private void HandleChange(LivingEntityChangeEvent @event) => Render();

        public void Render(LivingEntityData data) => Entity = data;

        public void Render()
        {
            for (int ix = 0; ix < _renderers.Length; ix++)
            {
                var renderer = _renderers[ix];
                if (ix >= Entity.Effects.Count) { renderer.Hide(); }
                else { renderer.Render(Entity.Effects[ix]); }
            }
        }
    }
}