using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
namespace CaptainCoder.Dungeoneering.Unity
{
    public class RenderTextureMouseEvents : MonoBehaviour, IScrollHandler, IPointerClickHandler
    {
        public Camera TargetCamera;
        public RenderTexture TargetTexture;

        [field: SerializeField]
        public UnityEvent<ScrollData> OnScrollEvent {get; private set; } = new();

        public void OnPointerClick(PointerEventData eventData)
        {
            Vector2 position = eventData.position;
            RectTransform rect = (RectTransform)transform;
            Vector3[] corners = {default, default, default, default};
            rect.GetWorldCorners(corners);
            Vector2 onScreenSize = rect.rect.size;
            Vector2 scale = new (TargetTexture.width/onScreenSize.x, TargetTexture.height/onScreenSize.y);
            Vector2 screenP = (position - (Vector2)corners[0])*scale;
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