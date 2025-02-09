using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
namespace CaptainCoder.Dungeoneering.Unity
{
    public class RenderTextureMouseEvents : MonoBehaviour, IScrollHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public Camera TargetCamera;
        public RenderTexture TargetTexture;
        private Canvas _parent;

        [field: SerializeField]
        public UnityEvent<ScrollData> OnScrollEvent {get; private set; } = new();
        [field: SerializeField]
        public UnityEvent<Vector3> OnDragStart { get; private set; } = new();
        [field: SerializeField]
        public UnityEvent<Vector3> OnDragged { get; private set; } = new();
        [field: SerializeField]
        public UnityEvent<Vector3> OnDragEnd { get; private set; } = new();
        private bool _isDrag = false;

        private void Awake()
        {
            _parent = GetComponentInParent<Canvas>();
        }
        
        /// <summary>
        /// Given a screen position, normalizes that position based on the size of the viewport and the size of the render texture.
        /// This position can be safely passed into ScreenToWorld methods.
        /// </summary>
        /// <param name="screenPoint"></param>
        /// <returns></returns>    
        private Vector2 NormalizeScreenPoint(Vector2 screenPoint)
        {
#if UNITY_EDITOR
            // The simplified math in this method works because the canvas is in screen space overlay mode.
            // If it weren't, we'd have to transform to world space through the camera
            Debug.Assert(_parent.renderMode == RenderMode.ScreenSpaceOverlay);
#endif
	
            RectTransform rectTransform = (RectTransform)transform;
            return (Vector2)rectTransform.InverseTransformPoint(screenPoint) - rectTransform.rect.position;
        }
        
        private bool TryGetWorldPoint(Vector2 screenPoint, out Vector3 worldPoint)
        {
            worldPoint = default;
            bool hit = Physics.Raycast(TargetCamera.ScreenPointToRay(NormalizeScreenPoint(screenPoint)), out RaycastHit hitInfo);
            if (hit)
            {
                worldPoint = hitInfo.transform.position;
                return true;
            }
            return false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isDrag) { return; }
            Vector2 position = eventData.position;
            bool hit = Physics.Raycast(TargetCamera.ScreenPointToRay(NormalizeScreenPoint(position)), out RaycastHit hitInfo);
            if (hit && hitInfo.collider.TryGetComponent(out MouseEvents events))
            {
                events.OnClick.Invoke();
            }
        }

        public void OnScroll(PointerEventData eventData)
        {
            OnScrollEvent?.Invoke(new ScrollData(eventData));
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (TryGetWorldPoint(eventData.position, out Vector3 worldPoint))
            {
                _isDrag = true;
                OnDragStart.Invoke(worldPoint);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDrag) { return; }
            _isDrag = false;
            if (TryGetWorldPoint(eventData.position, out Vector3 end))
            {
                OnDragEnd.Invoke(end);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDrag) { return; }
            if (TryGetWorldPoint(eventData.position, out Vector3 worldPoint))
            {
                OnDragged.Invoke(worldPoint);
            }
        }
    }

    public class ScrollData
    {
        public PointerEventData RawEvent { get; }
        public float Y => RawEvent.scrollDelta.y;
        public float X => RawEvent.scrollDelta.x;

        public ScrollData(PointerEventData rawEvent) => RawEvent = rawEvent;
    }
}