using UnityEngine;
using UnityEngine.Events;

namespace CaptainCoder.Dungeoneering.Encounter
{
    public sealed class ClickHandler : MonoBehaviour, IClickHandler
    {
        [field: SerializeField] public UnityEvent OnClicked { get; private set; }
        public void OnClick() => OnClicked.Invoke();
    }
}