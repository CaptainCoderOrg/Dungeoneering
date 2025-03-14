using CaptainCoder.Dungeoneering.Unity.Data;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity;
public abstract record class TileChangedEvent;
public sealed record class DungeonCrawlerDataInitialized(DungeonCrawlerData Data) : TileChangedEvent;
public sealed record class TileReferenceUpdated(TileReference TileRef) : TileChangedEvent;