using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonCrawler;

namespace CaptainCoder.Dungeoneering.DungeonMap;
public static class DungeonExtensions
{
    public static WallMap Copy(this WallMap wallMap) => new() { Map = new(wallMap.Map) };
    public static EventMap Copy(this EventMap wallMap) => new() { Events = wallMap.Events.ToDictionary(kv => kv.Key, kv => kv.Value.ToList()) };
    public static WallTextureMap Copy(this WallTextureMap wallMap) => new()
    {
        DefaultDoor = wallMap.DefaultDoor,
        DefaultSecretDoor = wallMap.DefaultSecretDoor,
        DefaultSolid = wallMap.DefaultSolid,
        Textures = new(wallMap.Textures)
    };

    public static TileTextureMap Copy(this TileTextureMap wallMap) => new()
    {
        Default = wallMap.Default,
        Textures = new(wallMap.Textures),
    };

    public static Dungeon Copy(this Dungeon toCopy) => new()
    {
        Name = toCopy.Name,
        Walls = toCopy.Walls.Copy(),
        EventMap = toCopy.EventMap.Copy(),
        WallTextures = toCopy.WallTextures.Copy(),
        TileTextures = toCopy.TileTextures.Copy()
    };

    public static DungeonCrawlerManifest Copy(this DungeonCrawlerManifest original) => new()
    {
        Dungeons = original.Dungeons.ToDictionary(kv => kv.Key, kv => kv.Value.Copy()),
        Scripts = new(original.Scripts),
        Textures = original.Textures.ToDictionary(kv => kv.Key, kv => kv.Value.Copy())
    };

    public static Texture Copy(this Texture texture) => new(texture.Name, texture.Data.ToArray());

    /// <summary>
    /// Given a set of positions, wall types, and facings, calculates the changes necessary to change the walls on the boundary of the selected tiles.
    /// </summary>
    /// <param name="dungeon"></param>
    /// <param name="tiles"></param>
    /// <param name="wallType"></param>
    /// <param name="facings"></param>
    /// <returns></returns>
    public static IEnumerable<WallChangeData> FindWallChanges(this Dungeon dungeon, ISet<Position> tiles, WallType wallType, params Facing[] facings)
    {
        if (tiles.Count == 0) { yield break; }
        foreach (Facing facing in facings)
        {
            foreach (Position position in tiles)
            {
                Position neighbor = position.Step(facing);
                if (tiles.Contains(neighbor)) { continue; }
                WallType originalWallType = dungeon.Walls[position, facing];
                if (wallType != WallType.None && originalWallType != WallType.None) { continue; }
                yield return new WallChangeData(
                    new WallData(position, facing, wallType),
                    new WallData(position, facing, originalWallType)
                );
            }
        }
    }

    public static void SetBorderWalls(this Dungeon dungeon, ISet<Position> tiles, WallType wallType, params Facing[] facings) =>
        dungeon.SetWalls(FindWallChanges(dungeon, tiles, wallType, facings).Select(c => c.Changed));

    public static void SetWalls(this Dungeon dungeon, IEnumerable<WallData> walls)
    {
        foreach (var wall in walls)
        {
            dungeon.SetWallType(wall);
        }
    }

    private static void SetWallType(this Dungeon dungeon, WallData wall)
    {
        if (dungeon.Walls[wall.Position, wall.Facing] == wall.WallType) { return; }
        dungeon.WallTextures.Textures.Remove((wall.Position, wall.Facing));
        dungeon.WallTextures.Textures.Remove((wall.Position.Step(wall.Facing), wall.Facing.Opposite()));
        dungeon.Walls.SetWall(wall.Position, wall.Facing, wall.WallType);
    }
}