using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonMap;

namespace CaptainCoder.Dungeoneering.Unity.Data;
public class DungeonData
{
    private bool _preventNotify = false;
    public bool PreventNotify
    {
        get => _preventNotify;
        set
        {
            _preventNotify = value;
            Notify();
        }
    }
    private bool _hasChanged = false;
    public bool HasChanged
    {
        get => _hasChanged;
        internal set
        {
            if (value == _hasChanged) { return; }
            _hasChanged = value;
            _onChange?.Invoke(new DungeonSyncedStateChanged(_dungeon, !_hasChanged));
        }
    }
    private System.Action<DungeonChanged> _onChange;
    private Dungeon _dungeon;
    private readonly HashSet<TileReference> _changes = new();
    public Dungeon Dungeon
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
    public void AddObserver(System.Action<DungeonChanged> handle)
    {
        _onChange += handle;
        if (_dungeon != null)
        {
            handle.Invoke(new DungeonLoaded(_dungeon));
        }
    }

    public void RemoveObserver(System.Action<DungeonChanged> handle) => _onChange -= handle;

    private void Notify()
    {
        if (_preventNotify) { return; }
        if (!_changes.Any()) { return; }
        _onChange?.Invoke(new DungeonTilesChanged(_changes.AsEnumerable()));
        _changes.Clear();
    }

    // TODO: Redesign message type here so force notify is not required
    // internal void ForceNotify() => OnChange.Invoke(new DungeonChangedData(Dungeon, Dungeon));

    internal void AddChange(TileReference tile)
    {
        if (tile.Dungeon != Dungeon) { throw new System.ArgumentException($"Cannot add change to dungeon that is not loaded"); }
        _changes.Add(tile);
        HasChanged = true;
        Notify();
    }

    internal void AddChange(WallReference wall)
    {
        if (wall.Dungeon != Dungeon) { throw new System.ArgumentException($"Cannot add change to dungeon that is not loaded"); }
        _changes.Add(new TileReference(wall.Dungeon, wall.Position));
        HasChanged = true;
        Notify();
    }
}

public abstract record DungeonChanged;
public sealed record class DungeonLoaded(Dungeon Dungeon) : DungeonChanged;
public sealed record class DungeonUnloaded(Dungeon Dungeon) : DungeonChanged;
public sealed record class DungeonTilesChanged(IEnumerable<TileReference> Tiles) : DungeonChanged;
public sealed record class DungeonSyncedStateChanged(Dungeon Dungeon, bool IsSynced) : DungeonChanged;