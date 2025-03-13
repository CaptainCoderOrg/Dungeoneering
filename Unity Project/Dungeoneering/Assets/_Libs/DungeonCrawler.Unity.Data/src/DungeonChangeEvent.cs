using System.Collections.Generic;

using CaptainCoder.Dungeoneering.DungeonMap;

namespace CaptainCoder.Dungeoneering.Unity.Data;
public abstract record DungeonChangeEvent;
public sealed record class DungeonLoaded(Dungeon Dungeon) : DungeonChangeEvent;
public sealed record class DungeonUnloaded(Dungeon Dungeon) : DungeonChangeEvent;
public sealed record class TilesChanged(IEnumerable<TileReference> Tiles) : DungeonChangeEvent;
public sealed record class DefaultTileTextureChanged(Dungeon Dungeon, TextureReference Texture) : DungeonChangeEvent;
public sealed record class DefaultWallTextureChanged(Dungeon Dungeon, WallType WallType, TextureReference Texture) : DungeonChangeEvent;
public sealed record class SyncedStateChange(Dungeon Dungeon, bool IsSynced) : DungeonChangeEvent;