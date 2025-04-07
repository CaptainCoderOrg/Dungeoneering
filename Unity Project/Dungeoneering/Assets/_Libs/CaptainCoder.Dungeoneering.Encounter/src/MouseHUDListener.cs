using CaptainCoder.Dungeoneering.Encounter;

using UnityEngine;
using UnityEngine.EventSystems;

public class MouseHUDListener : MonoBehaviour, IPointerClickHandler, IPointerMoveHandler
{
    private IMouseEnteredHandler _lastEntered;
    public void OnPointerClick(PointerEventData eventData)
    {
        Ray screenRay = Camera.main.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(screenRay, out RaycastHit hitInfo) && hitInfo.collider.TryGetComponent<IClickHandler>(out IClickHandler handler))
        {
            handler.OnClick();
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        Ray screenRay = Camera.main.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(screenRay, out RaycastHit hitInfo) && hitInfo.collider.TryGetComponent<IMouseEnteredHandler>(out IMouseEnteredHandler handler))
        {
            if (handler == _lastEntered) { return; }
            _lastEntered?.OnExit();
            _lastEntered = handler;
            _lastEntered.OnEnter();
        }
    }
}