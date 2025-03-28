using CaptainCoder.Unity.Assertions;

using TMPro;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public sealed class FigureTraitsRenderer : MonoBehaviour, IFigureRenderer
    {
        [AssertIsSet][SerializeField] private TextMeshProUGUI _healthLabel;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _armorLabel;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _speedLabel;
        public void Render(FigureData data)
        {
            _healthLabel.text = $"{data.EntityData.Health}/{data.EntityData.BaseHealth}";
            _armorLabel.text = data.EntityData.Armor.ToString();
            _speedLabel.text = data.EntityData.Speed.ToString();
        }
    }
}