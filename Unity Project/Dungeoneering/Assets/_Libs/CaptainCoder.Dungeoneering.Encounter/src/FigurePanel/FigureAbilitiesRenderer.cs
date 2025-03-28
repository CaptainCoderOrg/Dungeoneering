using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public sealed class FigureAbilitiesRenderer : MonoBehaviour, IFigureRenderer
    {
        [AssertIsSet][SerializeField] private FigureAbilityRenderer _abilityRendererPrefab;
        [AssertIsSet][SerializeField] private Transform _abilitiesParent;
        public void Render(FigureData data)
        {
            _abilitiesParent.DestroyAllChildren();
            foreach (var ability in ((EnemyEntityData)(data.EntityData)).Abilities)
            {
                var info = Instantiate(_abilityRendererPrefab, _abilitiesParent);
                info.Ability = ability;
            }
        }
    }
}