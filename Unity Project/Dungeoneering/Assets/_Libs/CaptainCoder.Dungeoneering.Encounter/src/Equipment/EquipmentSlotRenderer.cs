using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Unity.Assertions;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public sealed class EquipmentSlotRenderer : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
    {
        [AssertIsSet][SerializeField] private Image _image;
        [AssertIsSet][SerializeField] private SimpleTooltip _simpleTooltip;
        public EquipmentSlotReference EquipmentSlotReference { get; private set; }
        private Image _dragging;
        private EquipmentInfoPanel _equipmentInfoPanel;

        void Awake()
        {
            _equipmentInfoPanel = FindFirstObjectByType<EquipmentInfoPanel>();
            Debug.Assert(_equipmentInfoPanel != null, $"Could not find {nameof(EquipmentInfoPanel)}", this);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (EquipmentSlotReference.Data == null) { return; }
            _equipmentInfoPanel.Hide();
            _dragging = Instantiate(_image, GetComponentsInParent<Canvas>().Last().transform);
            _dragging.raycastTarget = false;
            Color color = _dragging.color;
            color.a = 0.75f;
            _dragging.color = color;
            _dragging.transform.position = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_dragging == null) { return; }
            _dragging.transform.position = eventData.position;
        }

        private readonly List<RaycastResult> _raycastResults = new();

        public void OnEndDrag(PointerEventData eventData)
        {
            if (EquipmentSlotReference.Data == null) { return; }
            Destroy(_dragging.gameObject);
            _dragging = null;
            EventSystem.current.RaycastAll(eventData, _raycastResults);
            foreach (var result in _raycastResults)
            {
                if (result.gameObject.TryGetComponent<EquipmentSlotDropZone>(out var dropZone))
                {
                    dropZone.SlotRenderer.EquipmentSlotReference.TrySwapEquipment(EquipmentSlotReference, out string _);
                    // Debug.Log(message, this);
                }
            }
        }

        private void Redraw()
        {
            if (EquipmentSlotReference.Data == null)
            {
                _image.enabled = false;
                return;
            }
            _image.enabled = true;
            _image.sprite = EquipmentSlotReference.Data.Sprite;
            _simpleTooltip.Tooltip = $"<b><u>{EquipmentSlotReference.Data.Name}</u></b>\nClick for Details";
        }

        public void Render(EquipmentSlotReference slot)
        {
            if (EquipmentSlotReference != null) { EquipmentSlotReference.OnDataChanged -= Redraw; }
            EquipmentSlotReference = slot;
            EquipmentSlotReference.OnDataChanged += Redraw;
            Redraw();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _equipmentInfoPanel.Toggle(EquipmentSlotReference, transform.position);
            _simpleTooltip.Hide();
        }
    }


}