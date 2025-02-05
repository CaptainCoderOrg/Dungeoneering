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
            Vector2 position = eventData.position;
            RectTransform rect = (RectTransform)transform;
            Vector3[] corners = {default, default, default, default};
            rect.GetWorldCorners(corners);
            Vector2 screenP = position - (Vector2)corners[0];
            // TODO: Scale based on camera size, currently this expects the render texture to match the size of the
            // element on screen
            Vector3 worldP = TargetCamera.ScreenToWorldPoint(screenP);
            Ray ray = TargetCamera.ScreenPointToRay(screenP);
            bool hit = Physics.Raycast(ray, out RaycastHit hitInfo);
            if (hit && hitInfo.collider.TryGetComponent(out MouseEvents events))
            {
                events.OnClick.Invoke();
            }
            
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