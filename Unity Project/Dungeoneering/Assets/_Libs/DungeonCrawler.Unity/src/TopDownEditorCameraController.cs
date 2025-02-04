using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
namespace CaptainCoder.Dungeoneering.Unity
{
    public class TopDownEditorCameraController : MonoBehaviour
    {
        [field: SerializeField]
        public Camera TargetCamera { get; private set; }
        [field: SerializeField]
        public RenderTextureMouseEvents Events { get; private set; }

        [field: SerializeField]
        public float ZoomScale { get; private set; } = 1f/6f;

        void Awake()
        {
            Debug.Assert(TargetCamera != null);
            Debug.Assert(Events != null);
            Events.OnScrollEvent.AddListener(HandleScroll);
        }

        private void HandleScroll(ScrollData scrollData) => Zoom(-scrollData.Y);

        public void Zoom(float delta)
        {
            TargetCamera.orthographicSize += ZoomScale * delta;
        }
    }
}