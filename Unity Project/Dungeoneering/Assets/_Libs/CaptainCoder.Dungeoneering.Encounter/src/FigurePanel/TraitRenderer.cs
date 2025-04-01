using CaptainCoder.Unity.Assertions;

using TMPro;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public sealed class TraitRenderer : MonoBehaviour, ILivingEntityRenderer
    {
        [AssertIsSet][SerializeField] private TextMeshProUGUI _traitLabel;
        [AssertIsSet][SerializeField] private SimpleTooltip _simpleTooltip;
        [AssertIsSet][SerializeField] private TraitTypeData _traitTypeData;
        private LivingEntityData _entity;
        public void Render(LivingEntityData hero)
        {
            if (_entity == hero) { return; }
            if (_entity != null)
            {
                _entity.OnChanged -= Draw;
            }
            _entity = hero;
            _entity.OnChanged += Draw;
            Draw(EquipmentChangedEvent.Instance);
        }

        private void Draw(LivingEntityChangeEvent @event)
        {
            if (@event is not EquipmentChangedEvent) { return; }
            _traitLabel.text = _entity.TraitValueText(_traitTypeData);
            _simpleTooltip.Tooltip = _entity.TraitDetails(_traitTypeData);
        }
    }
}