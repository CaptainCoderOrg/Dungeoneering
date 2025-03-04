namespace CaptainCoder.Dungeoneering.DungeonMap;

public class Dungeon() : IEquatable<Dungeon>
{

    public Guid DungeonID { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = $"Untitled";
    public WallMap Walls { get; init; } = new();
    public EventMap EventMap { get; init; } = new();
    public WallTextureMap WallTextures { get; init; } = new();
    public TileTextureMap TileTextures { get; init; } = new();

    public Tile GetTile(Position position)
    {
        Walls.TryGetWall(position, Facing.North, out WallType north);
        Walls.TryGetWall(position, Facing.East, out WallType east);
        Walls.TryGetWall(position, Facing.South, out WallType south);
        Walls.TryGetWall(position, Facing.West, out WallType west);
        TileWalls walls = new(north, east, south, west);
        return new Tile(position, walls);
    }

    public Dungeon(WallMap walls) : this()
    {
        Walls = walls;
    }

    public bool IsPassable(Position position, Facing facing)
    {
        return !Walls.TryGetWall(position, facing, out WallType wall) || wall.IsPassable();
    }

    public bool Equals(Dungeon other)
    {
        return DungeonID == other.DungeonID;//&& Name == other.Name && Walls.Equals(other.Walls) && EventMap.Equals(other.EventMap) && TileTextures.Equals(other.TileTextures);
    }
}

public record Tile(Position Position, TileWalls Walls);
public record TileWalls(WallType North, WallType East, WallType South, WallType West);