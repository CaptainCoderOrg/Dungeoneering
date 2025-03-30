using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public sealed class HeroEquipmentInfoRenderer : MonoBehaviour, IHeroEntityRenderer
    {
        [AssertIsSet][SerializeField] private EquipmentSlotRenderer _leftHand;
        [AssertIsSet][SerializeField] private EquipmentSlotRenderer _rightHand;
        [AssertIsSet][SerializeField] private EquipmentSlotRenderer _wornArmor;
        [AssertIsSet][SerializeField] private EquipmentSlotRenderer _accessory;
        [AssertIsSet][SerializeField] private EquipmentSlotRenderer[] _backpack;

        public void Render(HeroEntityData data)
        {
            _leftHand.Render(data.LeftHand);
            _rightHand.Render(data.RightHand);
            _wornArmor.Render(data.WornArmor);
            _accessory.Render(data.Accessory);
        }

        public void Render(LivingEntityData data) => Render((HeroEntityData)data);
    }
}