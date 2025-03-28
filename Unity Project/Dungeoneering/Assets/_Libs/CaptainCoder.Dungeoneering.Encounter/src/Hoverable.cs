using System.Collections;

using CaptainCoder.Unity.Assertions;

using UnityEngine;
using UnityEngine.EventSystems;

namespace CaptainCoder.Dungeoneering.Encounter
{
    public class Hoverable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
    {
        [AssertIsSet][SerializeField] private TooltipElementData _tooltipElement;
        [AssertIsSet][SerializeField] public TooltipData TooltipData { get; set; }
        [AssertIsSet][SerializeField] private RectTransform _hoverTarget;
        [SerializeField] private float _hoverDelay = .5f;
        private PointerEventData _eventData;

        public void Hover()
        {
            TooltipData.Render(_tooltipElement.Tooltip);
            _tooltipElement.Tooltip.ShowAbove(_hoverTarget);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _eventData = eventData;
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
            _tooltipElement.Tooltip.Hide();
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            _eventData = eventData;
        }
    }
}