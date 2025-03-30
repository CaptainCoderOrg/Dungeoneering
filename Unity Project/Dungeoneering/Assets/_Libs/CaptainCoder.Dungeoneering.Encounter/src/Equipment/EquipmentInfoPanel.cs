
using System.Collections;

using CaptainCoder.Unity.Assertions;

using NaughtyAttributes;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EquipmentInfoPanel : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private CanvasGroup _canvasGroup;
        private IEquipmentRenderer[] _renderers;
        [SerializeField] private EquipmentData _equipment;
        private EquipmentSlotReference _current;
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
            Hide();
            Redraw();
        }

        [Button]
        private void ForceRedraw()
        {
            _renderers = GetComponentsInChildren<IEquipmentRenderer>();
            Redraw();
        }

        public void Toggle(EquipmentSlotReference slot, Vector3 position)
        {
            _equipment = slot.Data;
            if (_current == slot)
            {
                Toggle();
                return;
            }
            _current = slot;
            transform.position = position;
            ((RectTransform)transform).EnsureOnScreen();
            Redraw();
            Show();
        }

        public void Redraw()
        {
            if (Equipment == null) { return; }
            foreach (var renderer in _renderers)
            {
                renderer.Render(Equipment);
            }
        }

        public void Toggle()
        {
            if (Equipment == null)
            {
                Hide();
                return;
            }
            if (_canvasGroup.alpha == 0) { Show(); }
            else { Hide(); }
        }

        public void Show()
        {
            if (Equipment == null)
            {
                Hide();
                return;
            }
            StartCoroutine(ResizeAndShow());
        }

        public IEnumerator ResizeAndShow()
        {
            yield return null;
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}