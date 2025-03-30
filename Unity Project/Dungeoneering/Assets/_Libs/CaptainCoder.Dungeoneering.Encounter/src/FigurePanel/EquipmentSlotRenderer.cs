using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Unity.Assertions;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public sealed class EquipmentSlotRenderer : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [AssertIsSet][SerializeField] private Image _image;
        [AssertIsSet][SerializeField] private SimpleTooltip _simpleTooltip;
        public EquipmentSlotReference EquipmentSlotReference { get; private set; }
        private Image _dragging;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (EquipmentSlotReference.Data == null) { return; }
            _dragging = Instantiate(_image, GetComponentsInParent<Canvas>().Last().transform);
            _dragging.raycastTarget = false;
            Color color = _dragging.color;
            color.a = 0.75f;
            _dragging.color = color;
            _dragging.transform.position = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(_dragging == null) { return; }
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
                    dropZone.SlotRenderer.EquipmentSlotReference.TrySwapEquipment(EquipmentSlotReference, out string message);
                    Debug.Log(message);
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
            _simpleTooltip.Tooltip = EquipmentSlotReference.Data.Name;
        }

        public void Render(EquipmentSlotReference slot)
        {
            if (EquipmentSlotReference != null) { EquipmentSlotReference.OnDataChanged -= Redraw; }
            EquipmentSlotReference = slot;
            EquipmentSlotReference.OnDataChanged += Redraw;
            Redraw();
        }
    }

    public abstract record class EquipmentSlotReference(HeroEntityData Hero)
    {
        public event System.Action OnDataChanged
        {
            add => DataChanged += value;
            remove => DataChanged -= value;
        }
        protected System.Action DataChanged { get; private set; }
        public abstract EquipmentData Data { get; set; }
        public abstract bool CanHold(EquipmentData data, out string result);
        public bool TrySetEquipment(EquipmentData data, out string result)
        {
            if (Data != null)
            {
                result = "Slot was already occupied";
                return false;
            }
            if (!CanHold(data, out result)) { return false; }
            Data = data;
            return true;
        }
        public bool TrySwapEquipment(EquipmentSlotReference other, out string result)
        {
            if (!CanHold(other.Data, out result) || !other.CanHold(Data, out result))
            {
                return false;
            }
            (other.Data, Data) = (Data, other.Data);
            result = "Swapped";
            return true;
        }
    }
    public sealed record class BackpackSlotReference(HeroEntityData Hero, int SlotIndex) : EquipmentSlotReference(Hero)
    {
        public override bool CanHold(EquipmentData data, out string result)
        {
            result = "Success";
            return true;
        }
        public override EquipmentData Data
        {
            get => Hero.Inventory.Count > SlotIndex ? Hero.Inventory[SlotIndex] : null;
            set
            {
                while (Hero.Inventory.Count <= SlotIndex) { Hero.Inventory.Add(null); }
                Hero.Inventory[SlotIndex] = value;
                DataChanged?.Invoke();
            }
        }

    }

    public sealed record class LeftHandSlotReference(HeroEntityData Hero) : EquipmentSlotReference(Hero)
    {
        public override EquipmentData Data
        {
            get => Hero.LeftHand;
            set
            {
                if (Data is HeldEquipmentData previous && previous.Hands == 2)
                {
                    Hero.RightHandSlot.SetTwoHanded(null);
                }
                HeldEquipmentData data = (HeldEquipmentData)value;
                Hero.LeftHand = data;
                if (data != null && data.Hands == 2)
                {
                    Hero.RightHandSlot.SetTwoHanded(data);
                }
                DataChanged?.Invoke();
            }
        }

        public override bool CanHold(EquipmentData data, out string result)
        {
            if (data == null)
            {
                result = "Success";
                return true;
            }
            if (data is not HeldEquipmentData heldEquipmentData)
            {
                result = "Not a held item";
                return false;
            }
            if (heldEquipmentData.Hands == 2 && Hero.RightHand != null)
            {
                result = "Requires two hands";
                return false;
            }
            result = "Success";
            return true;
        }

        internal void SetTwoHanded(HeldEquipmentData data)
        {
            Hero.LeftHand = data;
            DataChanged?.Invoke();
        }
    }
    public sealed record class RightHandSlotReference(HeroEntityData Hero) : EquipmentSlotReference(Hero)
    {
        public override EquipmentData Data
        {
            get => Hero.RightHand;
            set
            {
                if (Data is HeldEquipmentData previous && previous.Hands == 2)
                {
                    Hero.LeftHandSlot.SetTwoHanded(null);
                }
                HeldEquipmentData data = (HeldEquipmentData)value;
                Hero.RightHand = data;
                if (data != null && data.Hands == 2)
                {
                    Hero.LeftHandSlot.SetTwoHanded(data);
                }
                DataChanged?.Invoke();
            }
        }

        public override bool CanHold(EquipmentData data, out string result)
        {
            if (data == null)
            {
                result = "Success";
                return true;
            }
            if (data is not HeldEquipmentData heldEquipmentData)
            {
                result = "Not a held item";
                return false;
            }
            if (heldEquipmentData.Hands == 2 && Hero.LeftHand != null)
            {
                result = "Requires two hands";
                return false;
            }
            result = "Success";
            return true;
        }

        internal void SetTwoHanded(HeldEquipmentData data)
        {
            Hero.RightHand = data;
            DataChanged?.Invoke();
        }
    }
    public sealed record class WornEquipmentSlotReference(HeroEntityData Hero) : EquipmentSlotReference(Hero)
    {
        public override EquipmentData Data
        {
            get => Hero.WornArmor;
            set
            {
                Hero.WornArmor = (WornEquipmentData)value;
                DataChanged?.Invoke();
            }
        }

        public override bool CanHold(EquipmentData data, out string result)
        {
            if (data == null)
            {
                result = "Success";
                return true;
            }
            if (data is not WornEquipmentData)
            {
                result = "Not a worn item";
                return false;
            }
            result = "Success";
            return true;
        }
    }
    public sealed record class AccessorySlotReference(HeroEntityData Hero) : EquipmentSlotReference(Hero)
    {
        public override EquipmentData Data
        {
            get => Hero.Accessory;
            set
            {
                Hero.Accessory = (AccessoryEquipmentData)value;
                DataChanged?.Invoke();
            }
        }

        public override bool CanHold(EquipmentData data, out string result)
        {
            if (data == null)
            {
                result = "Success";
                return true;
            }
            if (data is not AccessoryEquipmentData)
            {
                result = "Not an accessory item";
                return false;
            }
            result = "Success";
            return true;
        }
    }
}