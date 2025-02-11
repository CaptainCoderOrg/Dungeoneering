using UnityEngine;
namespace CaptainCoder.Unity
{
    public class UIForceScaleRatio : MonoBehaviour
    {
        [field: SerializeField]
        public float Ratio { get; private set; } = 16f/9f;
        private Vector2 _containerSize;

        void Update()
        {
            RectTransform parentRect = (RectTransform)transform.parent;
            if (parentRect.rect.size != _containerSize)
            {
                ResizeToContainer();
            }
        }

        private void ResizeToContainer()
        {
            RectTransform parentRect = (RectTransform)transform.parent;
            _containerSize = parentRect.rect.size;
            RectTransform rect = (RectTransform)transform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            float height = _containerSize.x/Ratio;
            if (height > _containerSize.y)
            {
                rect.sizeDelta = new Vector2(_containerSize.y*Ratio, _containerSize.y);
            }
            else
            {           
                rect.sizeDelta = new Vector2(_containerSize.x, height);
            }
        }
    }
}
