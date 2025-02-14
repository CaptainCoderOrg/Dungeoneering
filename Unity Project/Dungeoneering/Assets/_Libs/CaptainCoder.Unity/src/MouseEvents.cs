using UnityEngine;
using UnityEngine.Events;

public class MouseEvents : MonoBehaviour
{
    [field: SerializeField]
    public UnityEvent OnClick { get; private set; } = new();
    public void Click() => OnClick.Invoke();
}