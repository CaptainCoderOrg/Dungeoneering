using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap;

namespace CaptainCoder.Dungeoneering.Unity.Data;

public abstract record class DungeonManifestChanged;
public sealed record class ManifestInitialized(DungeonCrawlerManifest Manifest) : DungeonManifestChanged;
public sealed record class DungeonRemovedEvent(Dungeon Dungeon) : DungeonManifestChanged;