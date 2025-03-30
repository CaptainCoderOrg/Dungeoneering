
using CaptainCoder.Unity.Assertions;

using TMPro;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EquipmentTypeRenderer : MonoBehaviour, IEquipmentRenderer
    {
        [AssertIsSet][SerializeField] private TextMeshProUGUI _label;
        public void Render(EquipmentData data)
        {
            _label.text = data switch
            {
                HeldEquipmentData held when held.Hands == 1 => "Held",
                HeldEquipmentData held when held.Hands == 2 => "Held\nTwo-Handed",
                WornEquipmentData => "Armor",
                AccessoryEquipmentData => "Accessory",
                _ => "Item"
            };
        }
    }
}