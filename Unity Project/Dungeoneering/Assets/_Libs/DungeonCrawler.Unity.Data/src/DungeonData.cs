using System.Collections.Generic;

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
            OnStateChanged?.Invoke(_dungeon, _hasChanged);
        }
    }
    // TODO: Consolidate events into a single event type
    public event System.Action<Dungeon, bool> OnStateChanged;
    public event System.Action<DungeonChangedData> OnChange;
    public event System.Action<TilesChangedData> OnTilesChanged;
    private Dungeon _dungeon;
    private TilesChangedData _changes = new();
    public Dungeon Dungeon
    {
        get => _dungeon;
        set
        {
            if (_dungeon == value) { return; }
            HasChanged = false;
            DungeonChangedData change = new(_dungeon, value);
            _dungeon = value;
            OnChange?.Invoke(change);
        }
    }
    public void AddObserver(System.Action<DungeonChangedData> handle)
    {
        OnChange += handle;
        if (_dungeon != null)
        {
            handle.Invoke(new DungeonChangedData(null, _dungeon));
        }
    }

    public void RemoveObserver(System.Action<DungeonChangedData> handle) => OnChange -= handle;

    private void Notify()
    {
        if (_preventNotify) { return; }
        if (_changes.Tiles.Count == 0) { return; }
        OnTilesChanged?.Invoke(_changes);
        _changes = new();
    }

    // TODO: Redesign message type here so force notify is not required
    internal void ForceNotify() => OnChange.Invoke(new DungeonChangedData(Dungeon, Dungeon));

    internal void AddChange(TileReference tile)
    {
        if (tile.Dungeon != Dungeon) { throw new System.ArgumentException($"Cannot add change to dungeon that is not loaded"); }
        _changes.AddChange(tile.Dungeon, tile.Position);
        HasChanged = true;
        Notify();
    }

    internal void AddChange(WallReference wall)
    {
        if (wall.Dungeon != Dungeon) { throw new System.ArgumentException($"Cannot add change to dungeon that is not loaded"); }
        _changes.AddChange(wall.Dungeon, wall.Position);
        HasChanged = true;
        Notify();
    }
}

public record DungeonChangedData(Dungeon Previous, Dungeon New);

public class TilesChangedData
{
    public HashSet<(Dungeon, Position)> Tiles { get; private set; } = new();
    public bool AddChange(Dungeon dungeon, Position position) => Tiles.Add((dungeon, position));
}