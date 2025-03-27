using CaptainCoder.Unity.Assertions;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public sealed class FigurePortraitRenderer : MonoBehaviour, IFigureRenderer
    {
        [AssertIsSet][SerializeField] private Image _image;
        public void Render(FigureData data) => _image.sprite = data.Portrait;
    }
}