using CaptainCoder.Unity.Assertions;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class AttackIconController : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private Image _image;
        [AssertIsSet][SerializeField] private Hoverable _hoverable;
        private AttackTypeData _attackType;
        public AttackTypeData AttackType
        {
            get => _attackType;
            set
            {
                _image.sprite = value.Sprite;
                _hoverable.TooltipData = value.TooltipData;
            }
        }
    }
}