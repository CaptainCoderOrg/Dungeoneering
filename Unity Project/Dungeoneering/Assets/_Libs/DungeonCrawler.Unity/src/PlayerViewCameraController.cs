using CaptainCoder.Dungeoneering.Player;
using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity
{
    public class PlayerViewCameraController : MonoBehaviour
    {
        [field: SerializeField]
        public CameraMode CameraMode { get; private set; }
        [field: SerializeField]
        public PlayerViewData PlayerView { get; private set; }

        public void OnEnable()
        {
            PlayerView.OnChange.AddListener(HandlePlayerViewChanged);
            (transform.position, transform.rotation) = CameraMode.EndPositionAndRotation(PlayerView.View);            
        }

        public void OnDisable()
        {
            PlayerView.OnChange.RemoveListener(HandlePlayerViewChanged);
        }

        private void HandlePlayerViewChanged(PlayerView exit, PlayerView enter)
        {
            // print("Hello world!")
            Debug.Log($"Player View Changed: {exit} => {enter}");
            StartCoroutine(CameraMode.HandlePlayerViewChanged(transform, exit, enter));
        }

    }
}