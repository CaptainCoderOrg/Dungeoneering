namespace Tests;

using CaptainCoder.Dungeoneering.DungeonMap;

using Shouldly;

public class Cursor_should
{
    [Theory]
    [InlineData(Facing.North, Facing.East)]
    [InlineData(Facing.East, Facing.South)]
    [InlineData(Facing.South, Facing.West)]
    [InlineData(Facing.West, Facing.North)]
    public void rotate_clockwise(Facing start, Facing expected)
    {
        Cursor underTest = new(new Position(0, 0), start);
        underTest.Rotate().Facing.ShouldBe(expected);
    }

    [Theory]
    [InlineData(Facing.North, Facing.West)]
    [InlineData(Facing.West, Facing.South)]
    [InlineData(Facing.South, Facing.East)]
    [InlineData(Facing.East, Facing.North)]
    public void rotate_counter_clockwise(Facing start, Facing expected)
    {
        Cursor underTest = new(new Position(0, 0), start);
        underTest.RotateCounterClockwise().Facing.ShouldBe(expected);
    }

    public static IEnumerable<object[]> MoveAndRotateTestData => [
        // Start, Move Direction, Expected state
        [new Cursor(new Position(10, 10), Facing.North), Facing.North, new Cursor(new Position(10, 9), Facing.North)],
        [new Cursor(new Position(1, 1), Facing.South), Facing.East, new Cursor(new Position(2, 1), Facing.East)],
        [new Cursor(new Position(8, 7), Facing.West), Facing.South, new Cursor(new Position(8, 8), Facing.South)],
        [new Cursor(new Position(2, 4), Facing.East), Facing.West, new Cursor(new Position(1, 4), Facing.West)],
    ];

    [Theory]
    [MemberData(nameof(MoveAndRotateTestData))]
    public void move_and_rotate(Cursor underTest, Facing move, Cursor expected)
    {
        underTest.MoveAndRotate(move).ShouldBe(expected);
    }

    public static IEnumerable<object[]> MoveTestData => [
        // Start, Move Direction, Expected state
        [new Cursor(new Position(10, 10), Facing.North), Facing.North, new Cursor(new Position(10, 9), Facing.North)],
        [new Cursor(new Position(1, 1), Facing.South), Facing.East, new Cursor(new Position(2, 1), Facing.South)],
        [new Cursor(new Position(8, 7), Facing.West), Facing.South, new Cursor(new Position(8, 8), Facing.West)],
        [new Cursor(new Position(2, 4), Facing.East), Facing.West, new Cursor(new Position(1, 4), Facing.East)],
    ];

    [Theory]
    [MemberData(nameof(MoveTestData))]
    public void move(Cursor underTest, Facing move, Cursor expected)
    {
        underTest.Move(move).ShouldBe(expected);
    }

    public static IEnumerable<object[]> TurnTestData => [
        [new Cursor(new Position(10, 10), Facing.North), Facing.East],
        [new Cursor(new Position(1, 1), Facing.South), Facing.West],
        [new Cursor(new Position(8, 7), Facing.West), Facing.South],
        [new Cursor(new Position(2, 4), Facing.East), Facing.North],
    ];

    [Theory]
    [MemberData(nameof(TurnTestData))]
    public void update_facing_on_turn(Cursor underTest, Facing newFacing)

    {
        underTest.Turn(newFacing).Facing.ShouldBe(newFacing);
    }

    [Theory]
    [MemberData(nameof(TurnTestData))]
    public void not_move_on_turn(Cursor underTest, Facing newFacing)

    {
        underTest.Turn(newFacing).Position.ShouldBe(underTest.Position);
    }
}