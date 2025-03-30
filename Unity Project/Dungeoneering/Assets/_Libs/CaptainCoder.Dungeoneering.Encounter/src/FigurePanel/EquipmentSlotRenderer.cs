using CaptainCoder.Unity.Assertions;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public sealed class EquipmentSlotRenderer : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private Image _image;
        [AssertIsSet][SerializeField] private SimpleTooltip _simpleTooltip;

        public void Render(EquipmentData data)
        {
            if (data == null)
            {
                _image.enabled = false;
                return;
            }
            _image.enabled = true;
            _image.sprite = data.Sprite;
            _simpleTooltip.Tooltip = data.Name;
        }
    }
}