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
        private FigureData _figure;
        public void Render(FigureData data)
        {
            if (_figure != null) { _figure.EntityData.OnChanged -= HandleTraitsChanged; }
            _figure = data;
            _figure.EntityData.OnChanged += HandleTraitsChanged;
            Render();
        }

        private void HandleTraitsChanged(LivingEntityChangeEvent @event) => Render();

        private void Render()
        {
            _healthLabel.text = $"{_figure.EntityData.Health}/{_figure.EntityData.BaseHealth}";
            _armorLabel.text = _figure.EntityData.BaseArmor.ToString();
            _speedLabel.text = _figure.EntityData.BaseSpeed.ToString();
        }
    }
}