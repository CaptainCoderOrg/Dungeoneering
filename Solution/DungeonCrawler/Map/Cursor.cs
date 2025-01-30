namespace CaptainCoder.Dungeoneering.DungeonMap;
public record class Cursor(Position Position, Facing Facing);

public static class CursorExtensions
{
    public static Cursor Rotate(this Cursor cursor) => cursor with { Facing = cursor.Facing.Rotate() };
    public static Cursor RotateCounterClockwise(this Cursor cursor) => cursor with { Facing = cursor.Facing.RotateCounterClockwise() };
    public static Cursor Turn(this Cursor cursor, Facing facing) => cursor with { Facing = facing };
    public static Cursor MoveAndRotate(this Cursor cursor, Facing facing) => cursor with { Position = cursor.Position.Step(facing), Facing = facing };
    public static Cursor Move(this Cursor cursor, Facing facing) => cursor with { Position = cursor.Position.Step(facing) };

}