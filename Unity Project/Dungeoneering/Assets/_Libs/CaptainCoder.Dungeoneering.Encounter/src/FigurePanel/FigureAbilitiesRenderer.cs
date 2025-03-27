using CaptainCoder.Unity.Assertions;

using TMPro;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public sealed class FigureAbilitiesRenderer : MonoBehaviour, IFigureRenderer
    {
        [AssertIsSet][SerializeField] private TextMeshProUGUI _infoPrefab;
        [AssertIsSet][SerializeField] private Transform _abilitiesParent;
        public void Render(FigureData data)
        {
            _abilitiesParent.DestroyAllChildren();
            foreach (var ability in data.Abilities)
            {
                var info = Instantiate(_infoPrefab, _abilitiesParent);
                info.text = ability;
            }
        }
    }
}