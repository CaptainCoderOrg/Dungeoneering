using System;
using System.Collections.Generic;

using CaptainCoder.Dungeoneering.DungeonMap;

using UnityEngine.Events;
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
            OnStateChanged.Invoke(_dungeon, _hasChanged);
        }
    }
    private MaterialCache _materialCache;
    public UnityEvent<Dungeon, bool> OnStateChanged { get; private set; } = new();
    public event System.Action<DungeonChangedData> OnChange;
    public UnityEvent<TilesChangedData> OnTilesChanged { get; private set; } = new();
    private Dungeon _dungeon;
    private TilesChangedData _changes = new();
    public Dungeon Dungeon
    {
        get => _dungeon;
        set
        {
            if (_dungeon == value) { return; }
            if (_dungeon != null)
            {
                _dungeon.Walls.OnWallChanged -= HandleWallChanged;
                _dungeon.WallTextures.OnTextureChange -= HandleWallTextureChanged;
            }
            HasChanged = false;
            DungeonChangedData change = new(_dungeon, value);
            _dungeon = value;
            _dungeon.Walls.OnWallChanged += HandleWallChanged;
            _dungeon.WallTextures.OnTextureChange += HandleWallTextureChanged;
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

    public void RemoveObserver(System.Action<DungeonChangedData> handle)
    {
        OnChange -= handle;
    }

    private void HandleWallTextureChanged(Position _, Facing __, string ___) => HasChanged = true;
    private void HandleWallChanged(Position _, Facing __, WallType ___) => HasChanged = true;

    public void SetWallType(Position position, Facing facing, WallType type)
    {
        if (Dungeon.Walls[position, facing] == type) { return; }
        Dungeon.WallTextures.Textures.Remove((position, facing));
        Dungeon.WallTextures.Textures.Remove((position.Step(facing), facing.Opposite()));
        Dungeon.Walls.SetWall(position, facing, type);
        _changes.AddChange(Dungeon, position);
        HasChanged = true;
        Notify();
    }

    private void Notify()
    {
        if (_preventNotify) { return; }
        if (_changes.Tiles.Count == 0) { return; }
        OnTilesChanged.Invoke(_changes);
        _changes = new();
    }

    public TextureReference GetTexture(Position p) => _materialCache.GetTexture(Dungeon, p);
    public TextureReference GetTexture(Position p, Facing f) => _materialCache.GetTexture(new WallReference(Dungeon, p, f));
    public string GetWallTextureName(Position p, Facing f) => Dungeon.GetWallTexture(p, f);

    public DungeonData(MaterialCache materialCache)
    {
        _materialCache = materialCache;
    }

    // TODO: Redesign message type here so force notify is not required
    internal void ForceNotify() => OnChange.Invoke(new DungeonChangedData(Dungeon, Dungeon));

    internal void SetDefaultWallTexture(TextureReference newTexture, WallType wallType)
    {
        Action<string> setter = wallType switch
        {
            WallType.Solid => s => _dungeon.WallTextures.DefaultSolid = s,
            WallType.Door => s => _dungeon.WallTextures.DefaultDoor = s,
            WallType.SecretDoor => s => _dungeon.WallTextures.DefaultSecretDoor = s,
            _ => throw new Exception($"Cannot set texture for wall type None"),
        };
        setter.Invoke(newTexture.TextureName);
        HasChanged = true;
        OnChange.Invoke(new DungeonChangedData(Dungeon, Dungeon));
    }

    internal void AddChange(TileReference tile)
    {
        if (tile.Dungeon != Dungeon) { throw new ArgumentException($"Cannot add change to dungeon that is not loaded"); }
        _changes.AddChange(tile.Dungeon, tile.Position);
        HasChanged = true;
        Notify();
    }

    internal void AddChange(WallReference wall)
    {
        if (wall.Dungeon != Dungeon) { throw new ArgumentException($"Cannot add change to dungeon that is not loaded"); }
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