using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
namespace CaptainCoder.Dungeoneering.Unity
{
    public class RenderTextureMouseEvents : MonoBehaviour, IScrollHandler, IPointerClickHandler
    {
        public Camera TargetCamera;

        [field: SerializeField]
        public UnityEvent<ScrollData> OnScrollEvent {get; private set; } = new();

        public void OnPointerClick(PointerEventData eventData)
        {
            // TargetCamera.ScreenPointToRay
            Debug.Log($"Screen Size: {TargetCamera.pixelWidth}x{TargetCamera.pixelHeight}");
            Vector2 position = eventData.position;
            Debug.Log($"Screen Position: {position}");
            RectTransform rect = (RectTransform)transform;
            Debug.Log($"Rect: {rect.rect}");
            Debug.Log($"Position: {rect.position}");
            Debug.Log($"Pivot: {rect.pivot}");
            Vector3[] corners = {default, default, default, default};
            rect.GetWorldCorners(corners);
            Debug.Log($"Corners: {string.Join(",", corners)}");
            Vector2 screenP = position - (Vector2)corners[0];
            Debug.Log($"Screen: {screenP}");
            Vector3 worldP = TargetCamera.ScreenToWorldPoint(screenP);
            Debug.Log($"World P: {worldP}");
            Ray ray = TargetCamera.ScreenPointToRay(screenP);
            // public static bool Raycast(Ray ray, out RaycastHit hitInfo)
            bool hit = Physics.Raycast(ray, out RaycastHit hitInfo);
            Debug.Log($"Hit: {hit}");
            Debug.Log($"Hit Info: {hitInfo}");
            Debug.Log($"{hitInfo.collider.gameObject}");
            
        }

        public void OnScroll(PointerEventData eventData)
        {
            OnScrollEvent?.Invoke(new ScrollData(eventData));
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