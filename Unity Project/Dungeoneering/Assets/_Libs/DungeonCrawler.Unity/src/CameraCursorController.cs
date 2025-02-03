using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.Player;
using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity
{
    public class CameraCursorController : MonoBehaviour
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

        private void HandlePlayerViewChanged(PlayerView exit, PlayerView enter)
        {
            transform.localRotation = enter.Facing switch
            {
                Facing.North => Quaternion.Euler(0, 0, 0),
                Facing.East => Quaternion.Euler(0, 0, -90),
                Facing.South => Quaternion.Euler(0, 0, -180),
                Facing.West => Quaternion.Euler(0, 0, -270),
                var facing => throw new System.Exception($"Unknown facing: {facing}"),
            };
        }

    }
}