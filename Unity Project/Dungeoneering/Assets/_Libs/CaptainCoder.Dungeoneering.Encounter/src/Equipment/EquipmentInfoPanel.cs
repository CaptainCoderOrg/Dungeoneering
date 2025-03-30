
using CaptainCoder.Unity.Assertions;

using NaughtyAttributes;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EquipmentInfoPanel : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private CanvasGroup _canvasGroup;
        private IEquipmentRenderer[] _renderers;
        [SerializeField] private EquipmentData _equipment;
        public EquipmentData Equipment
        {
            get => _equipment;
            private set
            {
                _equipment = value;
                Redraw();
            }
        }

        void Awake()
        {
            _renderers = GetComponentsInChildren<IEquipmentRenderer>();
            Redraw();
        }

        [Button]
        private void ForceRedraw()
        {
            _renderers = GetComponentsInChildren<IEquipmentRenderer>();
            Redraw();
        }

        public void Redraw()
        {
            if (Equipment == null) { return; }
            foreach (var renderer in _renderers)
            {
                renderer.Render(Equipment);
            }
        }

        public void Show()
        {
            _canvasGroup.alpha = 1;
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
        }
    }
}