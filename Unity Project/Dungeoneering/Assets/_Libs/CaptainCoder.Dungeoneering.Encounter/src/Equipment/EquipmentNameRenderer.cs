
using CaptainCoder.Unity.Assertions;

using TMPro;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EquipmentNameRenderer : MonoBehaviour, IEquipmentRenderer
    {
        [AssertIsSet][SerializeField] private TextMeshProUGUI _label;
        public void Render(EquipmentData data) => _label.text = data.Name;
    }
}