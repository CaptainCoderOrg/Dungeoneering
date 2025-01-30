namespace CaptainCoder.Dungeoneering.DungeonMap;

public record Position(int X, int Y);

public static class PositionExtensions
{
    public static Position Step(this Position position, Facing direction) => (direction, position) switch
    {
        (Facing.North, Position(int _, int y)) => position with { Y = y - 1 },
        (Facing.South, Position(int _, int y)) => position with { Y = y + 1 },
        (Facing.East, Position(int x, int _)) => position with { X = x + 1 },
        (Facing.West, Position(int x, int _)) => position with { X = x - 1 },
        _ => throw new NotImplementedException($"Unknown Facing: {direction}"),
    };

    public static Position Clamp(this Position position, int maxX, int maxY)
    {
        return position with { X = Math.Clamp(position.X, 0, maxX), Y = Math.Clamp(position.Y, 0, maxY) };
    }
}