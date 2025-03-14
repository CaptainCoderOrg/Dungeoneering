using CaptainCoder.Dungeoneering.DungeonMap.Unity;

namespace CaptainCoder.Dungeoneering.Unity;
public abstract record class SelectionChangedEvent;
public sealed record class SelectionChanged(ReadOnlySetView<DungeonTile> Tiles, ReadOnlySetView<DungeonWall> Walls) : SelectionChangedEvent;