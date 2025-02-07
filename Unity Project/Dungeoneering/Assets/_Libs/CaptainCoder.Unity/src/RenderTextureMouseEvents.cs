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

        [field: SerializeField]
        public UnityEvent<ScrollData> OnScrollEvent {get; private set; } = new();
        private GameObject _cube;

        void Awake()
        {
            _cube = GameObject.Find("Cube");
        }

        private void ScaleCube(Vector3 corner1, Vector3 corner2)
        {
            corner1.y = .25f;
            corner2.y = -.25f;
            Vector3 center = (corner1 + corner2) * 0.5f;
            Vector3 scale = new (
                1 + Mathf.Abs(corner2.x - corner1.x),
                Mathf.Abs(corner2.y - corner1.y),
                1 + Mathf.Abs(corner2.z - corner1.z)
            );
            _cube.transform.position = center;
            _cube.transform.localScale = scale;
        }

        public Collider[] PerformBoxCast()
        {

            Vector3 center = _cube.transform.position;
            Vector3 halfExtents = _cube.transform.localScale * 0.45f;
            Vector3 direction = Vector3.down;
            float maxDistance = 0.1f;

            // Perform the BoxCast
            RaycastHit[] hits = Physics.BoxCastAll(center, halfExtents, direction, transform.rotation, maxDistance);
            return hits.Select(h => h.collider).Where(c => c != null).ToArray();
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
        private Vector3 _startPosition;

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("Drag started");
            if (TryGetWorldPoint(eventData.position, out Vector3 worldPoint))
            {
                _isDrag = true;
                _startPosition = worldPoint;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDrag) { return; }
            _isDrag = false;
            Debug.Log("Drag Ended");
            if (TryGetWorldPoint(eventData.position, out Vector3 end))
            {
                ScaleCube(_startPosition, end);
                Collider[] all = PerformBoxCast();
                Debug.Log($"Hits: {all.Length}");
                Debug.Log(string.Join(", ", (object[])all));
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDrag) { return; }
            if (TryGetWorldPoint(eventData.position, out Vector3 worldPoint))
            {
                ScaleCube(_startPosition, worldPoint);
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