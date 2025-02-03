using UnityEngine;
using CaptainCoder.Dungeoneering.Player;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;

namespace CaptainCoder.Dungeoneering.Unity
{

    public class PlayerViewController : MonoBehaviour
    {
        [field: SerializeField]
        public PlayerViewData PlayerView { get; private set; }
        [field: SerializeField]
        public DungeonData Dungeon { get; private set; }
        public void HandleInput(MovementAction action)
        {
            PlayerView.View = PlayerControls.Move(Dungeon.Dungeon, PlayerView.View, action);
        }
    }

}