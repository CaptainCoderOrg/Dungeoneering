
using System.Collections.Generic;

using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;

namespace CaptainCoder.Dungeoneering.Unity.Editor;

public class WallSelectionData
{
    private static readonly Facing[] Facings = { Facing.North, Facing.East, Facing.South, Facing.West };
    private readonly HashSet<(Position, Facing)> _walls = new();
    public ISet<(Position, Facing)> Solid => _walls;
    private readonly HashSet<(Position, Facing)> _doors = new();
    public ISet<(Position, Facing)> Doors => _doors;
    private readonly HashSet<(Position, Facing)> _secretDoors = new();
    public ISet<(Position, Facing)> SecretDoors => _secretDoors;

    public void CountWalls(ISet<DungeonTile> tiles, ISet<DungeonWall> walls)
    {
        _walls.Clear();
        _doors.Clear();
        _secretDoors.Clear();
        foreach (var wall in walls)
        {
            HashSet<(Position, Facing)> wallSet = wall.WallType switch
            {
                WallType.Solid => _walls,
                WallType.Door => _doors,
                WallType.SecretDoor => _secretDoors,
                _ => null,
            };
            if (wallSet == null) { continue; }
            wallSet.Add((wall.Parent.Position, wall.Facing));
        }
        foreach (var tile in tiles)
        {
            foreach (var facing in Facings)
            {
                (Position p, Facing f) key = (tile.Position, facing);
                WallType wallType = tile.Dungeon.Walls.GetWall(key.p, key.f);
                if (wallType == WallType.Solid) { _walls.Add((key.p, key.f)); }
                else if (wallType == WallType.Door) { _doors.Add((key.p, key.f)); }
                else if (wallType == WallType.SecretDoor) { _secretDoors.Add((key.p, key.f)); }
            }
        }
    }

}