using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
namespace CaptainCoder.Dungeoneering.Unity
{
    public class RenderTextureMouseEvents : MonoBehaviour, IScrollHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public Camera TargetCamera;
        public RenderTexture TargetTexture;

        [field: SerializeField]
        public UnityEvent<ScrollData> OnScrollEvent {get; private set; } = new();
        private MouseEvents _mouseEvents;

        private GameObject _start, _end, _cube;

        void Awake()
        {
            _start = GameObject.Find("Start");
            _end = GameObject.Find("End");
            _cube = GameObject.Find("Cube");
        }

        /// <summary>
        /// Given a screen position, normalizes that position based on the size of the viewport and the size of the render texture.
        /// This position can be safely passed into ScreenToWorld methods.
        /// </summary>
        /// <param name="screenPoint"></param>
        /// <returns></returns>    
        private Vector2 NormalizeScreenPoint(Vector2 screenPoint)
        {
            RectTransform rect = (RectTransform)transform;
            Vector3[] corners = {default, default, default, default};
            rect.GetWorldCorners(corners);
            Vector2 onScreenSize = rect.rect.size;
            Vector2 scale = new (TargetTexture.width/onScreenSize.x, TargetTexture.height/onScreenSize.y);
            return (screenPoint - (Vector2)corners[0])*scale;
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

        
        private bool _isDrag = false;
        private Vector2 _startPosition;

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("Drag started");
            if (TryGetWorldPoint(eventData.position, out Vector3 worldPoint))
            {
                _isDrag = true;
                _startPosition = eventData.position;
                worldPoint.y = _start.transform.position.y;
                _start.transform.position = worldPoint;   
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDrag) { return; }
            _isDrag = false;
            Debug.Log("Drag Ended");
            if (TryGetWorldPoint(eventData.position, out Vector3 end))
            {
                end.y = _end.transform.position.y;
                _end.transform.position = end;
            }
            
            
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDrag) { return; }
            if (TryGetWorldPoint(eventData.position, out Vector3 worldPoint))
            {
                worldPoint.y = _end.transform.position.y;
                _end.transform.position = worldPoint;
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