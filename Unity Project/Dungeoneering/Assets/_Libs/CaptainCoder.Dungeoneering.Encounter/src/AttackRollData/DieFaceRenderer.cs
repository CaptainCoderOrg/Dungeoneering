using CaptainCoder.Unity.Assertions;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class DieFaceRenderer : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private Image _faceRenderer;
        [AssertIsSet][SerializeField] private Image _albedoRenderer;
        [AssertIsSet][SerializeField] private ToggleablePanel _toggleablePanel;

        public void RenderDieFace(DieData die, int faceIx)
        {
            _faceRenderer.sprite = die.GetFaceSprite(faceIx);
            _albedoRenderer.color = die.Albedo;
        }

        public void Show() => _toggleablePanel.IsEnabled = true;
        public void Hide() => _toggleablePanel.IsEnabled = false;
    }
}