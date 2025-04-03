using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupHider : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;

        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Show()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}