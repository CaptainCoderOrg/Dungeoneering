using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public sealed class EquipmentSlotDropZone : MonoBehaviour
    {
        [AssertIsSet][field: SerializeField] public EquipmentSlotRenderer SlotRenderer { get; private set; }
    }
}