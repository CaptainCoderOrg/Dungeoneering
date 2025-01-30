namespace Tests;

using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.Player;

using Shouldly;

public class PlayerControls_should
{
    public static Dungeon NoWalls { get; } = new();
    public static Dungeon SimpleSquareDungeon
    {
        get
        {
            Dungeon dungeon = new();
            dungeon.Walls.SetWall(new Position(0, 0), Facing.North, WallType.Solid);
            dungeon.Walls.SetWall(new Position(0, 0), Facing.South, WallType.Solid);
            dungeon.Walls.SetWall(new Position(0, 0), Facing.East, WallType.Solid);
            dungeon.Walls.SetWall(new Position(0, 0), Facing.West, WallType.Solid);
            return dungeon;
        }
    }

    [Theory]
    [InlineData(Facing.North, MovementAction.RotateRight, Facing.East)]
    [InlineData(Facing.East, MovementAction.RotateRight, Facing.South)]
    [InlineData(Facing.South, MovementAction.RotateRight, Facing.West)]
    [InlineData(Facing.West, MovementAction.RotateRight, Facing.North)]
    [InlineData(Facing.North, MovementAction.RotateLeft, Facing.West)]
    [InlineData(Facing.West, MovementAction.RotateLeft, Facing.South)]
    [InlineData(Facing.South, MovementAction.RotateLeft, Facing.East)]
    [InlineData(Facing.East, MovementAction.RotateLeft, Facing.North)]
    public void rotate_player(Facing original, MovementAction action, Facing expected)
    {
        Dungeon dungeon = new();
        PlayerView starting = new(new Position(0, 0), original);
        PlayerView actual = PlayerControls.Move(dungeon, starting, action);
        actual.Facing.ShouldBe(expected);
    }

    public static IEnumerable<object[]> UpdatePlayerViewWhenNoWallData => [
        [NoWalls, new PlayerView(new Position(0, 0), Facing.North), MovementAction.StepForward, new PlayerView(new Position(0, -1), Facing.North)],
        [NoWalls, new PlayerView(new Position(0, 0), Facing.North), MovementAction.StepBackward, new PlayerView(new Position(0, 1), Facing.North)],
        [NoWalls, new PlayerView(new Position(0, 0), Facing.North), MovementAction.StrafeLeft, new PlayerView(new Position(-1, 0), Facing.North)],
        [NoWalls, new PlayerView(new Position(0, 0), Facing.North), MovementAction.StrafeRight, new PlayerView(new Position(1, 0), Facing.North)],

        [NoWalls, new PlayerView(new Position(0, 0), Facing.East), MovementAction.StepForward, new PlayerView(new Position(1, 0), Facing.East)],
        [NoWalls, new PlayerView(new Position(0, 0), Facing.East), MovementAction.StepBackward, new PlayerView(new Position(-1, 0), Facing.East)],
        [NoWalls, new PlayerView(new Position(0, 0), Facing.East), MovementAction.StrafeLeft, new PlayerView(new Position(0, -1), Facing.East)],
        [NoWalls, new PlayerView(new Position(0, 0), Facing.East), MovementAction.StrafeRight, new PlayerView(new Position(0, 1), Facing.East)],

        [NoWalls, new PlayerView(new Position(0, 0), Facing.South), MovementAction.StepForward, new PlayerView(new Position(0, 1), Facing.South)],
        [NoWalls, new PlayerView(new Position(0, 0), Facing.South), MovementAction.StepBackward, new PlayerView(new Position(0, -1), Facing.South)],
        [NoWalls, new PlayerView(new Position(0, 0), Facing.South), MovementAction.StrafeLeft, new PlayerView(new Position(1, 0), Facing.South)],
        [NoWalls, new PlayerView(new Position(0, 0), Facing.South), MovementAction.StrafeRight, new PlayerView(new Position(-1, 0), Facing.South)],

        [NoWalls, new PlayerView(new Position(0, 0), Facing.West), MovementAction.StepForward, new PlayerView(new Position(-1, 0), Facing.West)],
        [NoWalls, new PlayerView(new Position(0, 0), Facing.West), MovementAction.StepBackward, new PlayerView(new Position(1, 0), Facing.West)],
        [NoWalls, new PlayerView(new Position(0, 0), Facing.West), MovementAction.StrafeLeft, new PlayerView(new Position(0, 1), Facing.West)],
        [NoWalls, new PlayerView(new Position(0, 0), Facing.West), MovementAction.StrafeRight, new PlayerView(new Position(0, -1), Facing.West)],
    ];

    [Theory]
    [MemberData(nameof(UpdatePlayerViewWhenNoWallData))]
    public void update_playerview_when_no_wall(Dungeon dungeon, PlayerView starting, MovementAction action, PlayerView expected)
    {
        PlayerView actual = PlayerControls.Move(dungeon, starting, action);
        actual.ShouldBe(expected);
    }

    public static IEnumerable<object[]> NotAllowMovementThroughWallsData => [
        [Facing.North, MovementAction.StepForward],
        [Facing.North, MovementAction.StepBackward],
        [Facing.North, MovementAction.StrafeLeft],
        [Facing.North, MovementAction.StrafeRight],

        [Facing.East, MovementAction.StepForward],
        [Facing.East, MovementAction.StepBackward],
        [Facing.East, MovementAction.StrafeLeft],
        [Facing.East, MovementAction.StrafeRight],

        [Facing.West, MovementAction.StepForward],
        [Facing.West, MovementAction.StepBackward],
        [Facing.West, MovementAction.StrafeLeft],
        [Facing.West, MovementAction.StrafeRight],

        [Facing.South, MovementAction.StepForward],
        [Facing.South, MovementAction.StepBackward],
        [Facing.South, MovementAction.StrafeLeft],
        [Facing.South, MovementAction.StrafeRight],
    ];

    [Theory]
    [MemberData(nameof(NotAllowMovementThroughWallsData))]

    public void not_allow_movement_through_walls(Facing facing, MovementAction action)
    {
        Dungeon dungeon = SimpleSquareDungeon;
        PlayerView starting = new(new Position(0, 0), facing);
        PlayerView actual = PlayerControls.Move(dungeon, starting, action);
        actual.ShouldBe(starting);
    }
}