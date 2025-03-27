using CaptainCoder.Dungeoneering.Encounter;

using UnityEngine;
using UnityEngine.EventSystems;

public class MouseHUDListener : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Ray screenRay = Camera.main.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(screenRay, out RaycastHit hitInfo) && hitInfo.collider.TryGetComponent<IClickHandler>(out IClickHandler handler))
        {
            handler.OnClick();
        }
    }
}