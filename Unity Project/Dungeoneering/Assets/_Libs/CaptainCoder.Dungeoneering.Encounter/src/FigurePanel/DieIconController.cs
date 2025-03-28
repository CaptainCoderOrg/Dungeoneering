using CaptainCoder.Unity.Assertions;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class DieIconController : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private Image _image;
        [AssertIsSet][SerializeField] private Hoverable _hoverable;
        private DieData _die;
        public DieData Die
        {
            get => _die;
            set
            {
                _die = value;
                _image.sprite = value.Sprite;
                _image.color = value.UIAlbedo;
            }
        }
    }
}