using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace CaptainCoder.Unity
{
    public class RenderTextureMouseEvents : MonoBehaviour, IScrollHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public Camera TargetCamera;
        public RenderTexture RenderTextureBase;
        private RenderTexture _renderTexture;
        private Matrix4x4 _screenToTextureMatrix = Matrix4x4.identity;

        [field: SerializeField]
        public UnityEvent<ScrollData> OnScrollEvent { get; private set; } = new();
        [field: SerializeField]
        public UnityEvent<Vector3> OnDragStart { get; private set; } = new();
        [field: SerializeField]
        public UnityEvent<Vector3> OnDragged { get; private set; } = new();
        [field: SerializeField]
        public UnityEvent<Vector3> OnDragEnd { get; private set; } = new();
        private bool _isDrag = false;
        private Vector2 _size;

        private Matrix4x4 ScreenToTextureMatrix
        {
            get
            {
                if (!transform.hasChanged)
                    return _screenToTextureMatrix;

                transform.hasChanged = false;
                RectTransform rectTransform = (RectTransform)transform;
                Rect rect = rectTransform.rect;
                Vector3 onScreenSize = rectTransform.TransformVector(rect.size);
                Vector2 bottomLeft = rectTransform.TransformPoint(rect.position);
                Vector2 scale = new(_renderTexture.width / onScreenSize.x, _renderTexture.height / onScreenSize.y);
                _screenToTextureMatrix.m00 = scale.x;
                _screenToTextureMatrix.m11 = scale.y;
                _screenToTextureMatrix.m03 = -bottomLeft.x * scale.x;
                _screenToTextureMatrix.m13 = -bottomLeft.y * scale.y;
                return _screenToTextureMatrix;
            }
        }

        private void InitializeRenderTexture()
        {
            _renderTexture = new(RenderTextureBase);
            TargetCamera.targetTexture = _renderTexture;
            GetComponent<RawImage>().texture = _renderTexture;
        }

        private void Start() => OnRectTransformDimensionsChange();

        private void OnRectTransformDimensionsChange()
        {
            transform.hasChanged = true;

            RectTransform rect = (RectTransform)transform;
            if (_size != rect.rect.size)
            {
                if (_renderTexture == null) { InitializeRenderTexture(); }
                _size = rect.rect.size;
                _renderTexture.Release();
                _renderTexture.width = (int)_size.x;
                _renderTexture.height = (int)_size.y;
                _renderTexture.Create();
                TargetCamera.Render();
            }
        }

        /// <summary>
        /// Given a screen position, normalizes that position based on the size of the viewport and the size of the render texture.
        /// This position can be safely passed into ScreenToWorld methods.
        /// </summary>
        /// <param name="screenPoint"></param>
        /// <returns></returns>    
        private Vector2 NormalizeScreenPoint(Vector2 screenPoint) => ScreenToTextureMatrix.MultiplyPoint(screenPoint);

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