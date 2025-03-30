using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public sealed class HeroInventoryInfoRenderer : MonoBehaviour, IHeroEntityRenderer
    {
        [AssertIsSet][SerializeField] private EquipmentSlotRenderer[] _backpack;

        public void Render(HeroEntityData data)
        {
            for (int ix = 0; ix < _backpack.Length; ix++)
            {
                _backpack[ix].Render(data.BackpackReferences[ix]);
            }
        }

        public void Render(LivingEntityData data) => Render((HeroEntityData)data);
    }
}