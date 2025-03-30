
using CaptainCoder.Unity.Assertions;


using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EquipmentSpriteRenderer : MonoBehaviour, IEquipmentRenderer
    {
        [AssertIsSet][SerializeField] private Image _image;
        public void Render(EquipmentData data) => _image.sprite = data.Sprite;
    }
}