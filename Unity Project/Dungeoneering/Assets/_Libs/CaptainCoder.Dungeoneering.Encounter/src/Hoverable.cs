using System.Collections;

using CaptainCoder.Unity.Assertions;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CaptainCoder.Dungeoneering.Encounter
{
    public class Hoverable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
    {
        [AssertIsSet][SerializeField] private RectTransform _hoverTarget;
        [SerializeField] private float _hoverDelay = .5f;
        [field: SerializeField] public UnityEvent<RectTransform> OnHoverStart { get; private set; }
        [field: SerializeField] public UnityEvent OnHoverEnd { get; private set; }

        public void Hover() => OnHoverStart.Invoke(_hoverTarget);

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Pointer Entered");
            StartCoroutine(PointerEntered());
        }


        private IEnumerator PointerEntered()
        {
            yield return new WaitForSeconds(_hoverDelay);
            Hover();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopAllCoroutines();
            OnHoverEnd.Invoke();
        }

        public void OnPointerMove(PointerEventData eventData)
        {
        }

        internal void Cancel()
        {
            StopAllCoroutines();
            OnHoverEnd.Invoke();
        }
    }
}