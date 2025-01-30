namespace CaptainCoder.Dungeoneering.Player;
using CaptainCoder.Dungeoneering.DungeonMap;

public static class PlayerControls
{
    /// <summary>
    /// Given a dungeon and a PlayerView performs the specified MovementAction.
    /// </summary>
    public static PlayerView Move(this Dungeon dungeon, PlayerView view, MovementAction action)
    {
        return action switch
        {
            MovementAction.StepForward when
                dungeon.IsPassable(view.Position, view.Facing) => view with { Position = view.Position.Step(view.Facing) },

            MovementAction.StepBackward when
                view.Facing.Opposite() is Facing opposite &&
                dungeon.IsPassable(view.Position, opposite) => view with { Position = view.Position.Step(opposite) },

            MovementAction.StrafeLeft when
                view.Facing.RotateCounterClockwise() is Facing rotated &&
                dungeon.IsPassable(view.Position, rotated) => view with { Position = view.Position.Step(rotated) },

            MovementAction.StrafeRight when
                view.Facing.Rotate() is Facing rotated &&
                dungeon.IsPassable(view.Position, rotated) => view with { Position = view.Position.Step(rotated) },

            MovementAction.RotateLeft => view with { Facing = view.Facing.RotateCounterClockwise() },
            MovementAction.RotateRight => view with { Facing = view.Facing.Rotate() },

            _ => view,
        };
    }
}