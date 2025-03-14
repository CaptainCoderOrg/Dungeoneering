using CaptainCoder.Dungeoneering.DungeonMap;
namespace CaptainCoder.Dungeoneering.Unity.Data;
public record struct WallReference(Dungeon Dungeon, Position Position, Facing Facing)
{
    public WallType WallType => Dungeon.Walls[Position, Facing];
}