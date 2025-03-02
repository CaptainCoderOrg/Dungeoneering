using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.Player;
using CaptainCoder.Dungeoneering.Unity.Data;

using UnityEngine;
using UnityEngine.InputSystem;

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
            if (Keyboard.current.shiftKey.isPressed)
            {
                HandleInputIgnoringWalls(action);
                return;
            }
            PlayerView.View = PlayerControls.Move(Dungeon.Dungeon, PlayerView.View, action);
        }

        private void HandleInputIgnoringWalls(MovementAction action)
        {
            (Position p, Facing f) = PlayerView.View;
            PlayerView newView = action switch
            {
                MovementAction.StepForward => new PlayerView(p.Step(f), f),
                MovementAction.StepBackward => new PlayerView(p.Step(f.Opposite()), f),
                MovementAction.StrafeLeft => new PlayerView(p.Step(f.RotateCounterClockwise()), f),
                MovementAction.StrafeRight => new PlayerView(p.Step(f.Rotate()), f),
                _ => PlayerControls.Move(Dungeon.Dungeon, PlayerView.View, action),
            };
            PlayerView.View = newView;
        }
    }

}