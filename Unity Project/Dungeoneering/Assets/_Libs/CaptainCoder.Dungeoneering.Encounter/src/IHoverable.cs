
using UnityEngine.EventSystems;

namespace CaptainCoder.Dungeoneering.Encounter;

public interface IHoverable
{
    public void OnHover(PointerEventData pointerEventData);
}