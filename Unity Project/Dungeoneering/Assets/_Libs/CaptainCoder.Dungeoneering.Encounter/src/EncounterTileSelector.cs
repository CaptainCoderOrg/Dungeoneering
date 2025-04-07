using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EncounterTileSelector : MonoBehaviour
    {

        [AssertIsSet][SerializeField] private MeshRenderer _highlightRenderer;

        public void ShowHighlight()
        {
            _highlightRenderer.enabled = true;
        }
    }
}