using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonMap;

namespace CaptainCoder.Dungeoneering.Unity.Data;
internal class LoadedDungeon
{
    private bool _preventNotify = false;
    internal bool PreventNotify
    {
        get => _preventNotify;
        set
        {
            _preventNotify = value;
            NotifyTileChanges();
        }
    }
    private bool _hasChanged = false;
    internal bool HasChanged
    {
        get => _hasChanged;
        set
        {
            if (value == _hasChanged) { return; }
            _hasChanged = value;
            _onChange?.Invoke(new SyncedStateChange(_dungeon, !_hasChanged));
        }
    }
    private System.Action<DungeonChangeEvent> _onChange;
    private readonly HashSet<TileReference> _cachedTileChanges = new();
    private Dungeon _dungeon;
    internal Dungeon Dungeon
    {
        get => _dungeon;
        set
        {
            if (_dungeon == value) { return; }

            if (_dungeon != null)
            {
                DungeonUnloaded unloaded = new(_dungeon);
                _onChange?.Invoke(unloaded);
            }

            _dungeon = value;
            DungeonLoaded loaded = new(_dungeon);
            _onChange?.Invoke(loaded);
            HasChanged = false;
        }
    }
    internal void AddObserver(System.Action<DungeonChangeEvent> handle)
    {
        _onChange += handle;
        if (_dungeon != null)
        {
            handle.Invoke(new DungeonLoaded(_dungeon));
        }
    }

    internal void RemoveObserver(System.Action<DungeonChangeEvent> handle) => _onChange -= handle;

    private void NotifyTileChanges()
    {
        if (_preventNotify) { return; }
        if (!_cachedTileChanges.Any()) { return; }
        _onChange?.Invoke(new TilesChanged(_cachedTileChanges.AsEnumerable()));
        _cachedTileChanges.Clear();
    }

    internal void AddChange(TileReference tile)
    {
        if (tile.Dungeon != Dungeon) { throw new System.ArgumentException($"Cannot add change to dungeon that is not loaded"); }
        _cachedTileChanges.Add(tile);
        HasChanged = true;
        NotifyTileChanges();
    }

    internal void AddChange(WallReference wall)
    {
        if (wall.Dungeon != Dungeon) { throw new System.ArgumentException($"Cannot add change to dungeon that is not loaded"); }
        _cachedTileChanges.Add(new TileReference(wall.Dungeon, wall.Position));
        HasChanged = true;
        NotifyTileChanges();
    }

    internal void SetDefaultTileTexture(TextureReference newTexture)
    {
        _dungeon.TileTextures.Default = newTexture.TextureName;
        HasChanged = true;
        _onChange?.Invoke(new DefaultTileTextureChanged(_dungeon, newTexture));
    }

    internal void SetDefaultWallTexture(WallType wallType, TextureReference newTexture)
    {
        _dungeon.WallTextures.SetDefaultTexture(wallType, newTexture.TextureName);
        HasChanged = true;
        _onChange?.Invoke(new DefaultWallTextureChanged(_dungeon, wallType, newTexture));
    }
}

public abstract record DungeonChangeEvent;
public sealed record class DungeonLoaded(Dungeon Dungeon) : DungeonChangeEvent;
public sealed record class DungeonUnloaded(Dungeon Dungeon) : DungeonChangeEvent;
public sealed record class TilesChanged(IEnumerable<TileReference> Tiles) : DungeonChangeEvent;
public sealed record class DefaultTileTextureChanged(Dungeon Dungeon, TextureReference Texture) : DungeonChangeEvent;
public sealed record class DefaultWallTextureChanged(Dungeon Dungeon, WallType WallType, TextureReference Texture) : DungeonChangeEvent;
public sealed record class SyncedStateChange(Dungeon Dungeon, bool IsSynced) : DungeonChangeEvent;