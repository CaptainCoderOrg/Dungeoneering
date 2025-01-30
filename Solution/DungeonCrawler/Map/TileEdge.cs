namespace CaptainCoder.Dungeoneering.DungeonMap;

public record class TileEdge(Position Position, Facing Facing) : IEquatable<TileEdge>
{
    public virtual bool Equals(TileEdge other)
    {
        if (other is null) { return false; }
        TileEdge normalized = this.Normalize();
        TileEdge normalizedOther = other.Normalize();
        return normalized.Position == normalizedOther.Position &&
               normalized.Facing == normalizedOther.Facing;
    }

    public override int GetHashCode()
    {
        TileEdge normalized = this.Normalize();
        return HashCode.Combine(normalized.Position, normalized.Facing);
    }
}

public static class TileEdgeExtensions
{
    public static TileEdge Normalize(this TileEdge edge)
    {
        return edge.Facing switch
        {
            Facing.North or Facing.East => edge,
            Facing.South => edge with { Facing = Facing.North, Position = edge.Position.Step(Facing.South) },
            Facing.West => edge with { Facing = Facing.East, Position = edge.Position.Step(Facing.West) },
            _ => throw new NotImplementedException($"Unknown facing: {edge.Facing}"),
        };
    }
}