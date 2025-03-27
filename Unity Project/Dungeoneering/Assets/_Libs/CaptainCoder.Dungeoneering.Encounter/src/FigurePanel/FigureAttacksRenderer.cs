using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public sealed class FigureAttacksREnderer : MonoBehaviour, IFigureRenderer
    {
        [AssertIsSet][SerializeField] private AttackInfoRenderer _infoPrefab;
        [AssertIsSet][SerializeField] private Transform _attacksParent;
        public void Render(FigureData data)
        {
            _attacksParent.DestroyAllChildren();
            foreach (var attack in data.Attacks)
            {
                var renderer = Instantiate(_infoPrefab, _attacksParent);
                renderer.Render(attack);
            }
        }
    }
}