using CaptainCoder.Dungeoneering.Player;
using CaptainCoder.Dungeoneering.Unity;
using UnityEngine;

public class PlayerViewCameraController : MonoBehaviour
{
    [field: SerializeField]
    public PlayerViewData PlayerView { get; private set; }

    public void OnEnable()
    {
        PlayerView.OnChange.AddListener(HandlePlayerViewChanged);
    }

    public void OnDisable()
    {
        PlayerView.OnChange.RemoveListener(HandlePlayerViewChanged);
    }

    private void HandlePlayerViewChanged(PlayerView view)
    {
        Debug.Log($"Player View Changed: {view}");
        transform.position = view.Position.ToVector3();
        transform.rotation = view.Facing.ToQuaternion();
    }
}
