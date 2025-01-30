
using CaptainCoder.Utils.DictionaryExtensions;

namespace CaptainCoder.Dungeoneering.DungeonMap;

public class WallMap() : IEquatable<WallMap>
{
    public event Action<Position, Facing, WallType>? OnWallChanged;
    public WallType this[Position position, Facing facing]
    {
        get => GetWall(position, facing);
        set => SetWall(position, facing, value);
    }
    public Dictionary<TileEdge, WallType> Map { get; set; } = new();
    public void SetWall(Position position, Facing facing, WallType wall)
    {
        Map[new TileEdge(position, facing).Normalize()] = wall;
        Notify(position, facing, wall);
    }
    public bool RemoveWall(Position position, Facing facing)
    {
        bool result = Map.Remove(new TileEdge(position, facing).Normalize());
        Notify(position, facing, WallType.None);
        return result;
    }
    public WallType GetWall(Position position, Facing facing) => Map.GetValueOrDefault(new TileEdge(position, facing).Normalize());
    public bool TryGetWall(Position position, Facing facing, out WallType wall) => Map.TryGetValue(new TileEdge(position, facing).Normalize(), out wall);
    public int Count => Map.Count;

    public WallMap(IEnumerable<(TileEdge, WallType)> edges) : this()
    {
        foreach ((TileEdge edge, WallType wall) in edges)
        {
            Map.Add(edge, wall);
        }
    }

    private void Notify(Position position, Facing facing, WallType wall)
    {
        OnWallChanged?.Invoke(position, facing, wall);
        OnWallChanged?.Invoke(position.Step(facing), facing.Opposite(), wall);
    }

    public bool Equals(WallMap other)
    {
        return Map.AllKeyValuesAreEqual(other.Map);
    }

    public void ClearOnWallChangedEvents() => OnWallChanged = null;
}

public static class WallMapExtensions
{
    public static WallMap CreateEmpty(int width, int height)
    {
        WallMap map = new();
        for (int x = 0; x < width; x++)
        {
            map.SetWall(new Position(x, 0), Facing.North, WallType.Solid);
            map.SetWall(new Position(x, height - 1), Facing.South, WallType.Solid);
        }

        for (int y = 0; y < height; y++)
        {
            map.SetWall(new Position(0, y), Facing.West, WallType.Solid);
            map.SetWall(new Position(width - 1, y), Facing.East, WallType.Solid);
        }
        return map;
    }

    public static WallMap RandomMap(int width, int height, double wallDensity, double doorDensity, double secretDoorDensity)
    {
        Random random = new();
        WallMap map = CreateEmpty(width, height);
        Facing[] directions = [Facing.North, Facing.West];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                foreach (Facing f in directions)
                {
                    if (random.NextDouble() < wallDensity)
                    {
                        if (random.NextDouble() < doorDensity)
                        {
                            if (random.NextDouble() < secretDoorDensity)
                            {
                                map.SetWall(new Position(x, y), f, WallType.SecretDoor);
                            }
                            else
                            {
                                map.SetWall(new Position(x, y), f, WallType.Door);
                            }
                        }
                        else
                        {
                            map.SetWall(new Position(x, y), f, WallType.Solid);
                        }
                    }
                }
            }
        }
        return map;
    }
}