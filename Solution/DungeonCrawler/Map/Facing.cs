namespace CaptainCoder.Dungeoneering.DungeonMap;

public enum Facing
{
    North = 0,
    East = 1,
    South = 2,
    West = 3,
}

public static class FacingExtensions
{
    public static Facing[] Values => [Facing.North, Facing.East, Facing.South, Facing.West];
    public static Facing Rotate(this Facing facing) => facing switch
    {
        Facing.North => Facing.East,
        Facing.East => Facing.South,
        Facing.South => Facing.West,
        Facing.West => Facing.North,
        _ => throw new Exception($"Unknown Facing: {facing}"),
    };

    public static Facing RotateCounterClockwise(this Facing facing) => facing switch
    {
        Facing.North => Facing.West,
        Facing.West => Facing.South,
        Facing.South => Facing.East,
        Facing.East => Facing.North,
        _ => throw new Exception($"Unknown Facing: {facing}"),
    };

    public static Facing Opposite(this Facing facing) => facing switch
    {
        Facing.North => Facing.South,
        Facing.West => Facing.East,
        Facing.South => Facing.North,
        Facing.East => Facing.West,
        _ => throw new Exception($"Unknown Facing: {facing}"),
    };
}