using System;
using System.Collections.Generic;

using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;

using UnityEngine;
using UnityEngine.Events;

namespace CaptainCoder.Dungeoneering.Unity;

public record struct TileReference(Dungeon Dungeon, Position Position);
public record struct WallReference(string DungeonName, Position Position, Facing Facing);
public record struct TextureId(int Id)
{
    public static implicit operator int(TextureId tid) => tid.Id;
}
public class TextureReferences
{
    public string TextureName { get; private set; }
    public HashSet<TileReference> Tiles { get; private set; } = new();
    public TextureReferences(string name) => TextureName = name;
    public int Count => Tiles.Count;
    public void Clear() => Tiles.Clear();
}
public class MaterialCache
{
    private readonly Dictionary<string, SelectableMaterial> _materialCache = new();
    private readonly Dictionary<TextureId, TextureReferences> _references = new();
    private readonly Dictionary<TileReference, TextureId> _reverseLookup = new();
    private readonly UnityEvent<CacheUpdateData> _onCacheChanged = new();
    private DungeonCrawlerManifest Manifest => _manifestData.Manifest;
    private DungeonManifestData _manifestData;
    // TODO: This feels quite brittle, perhaps a parent object that wires things up for us
    private DungeonData _dungeonData;
    public DungeonData DungeonData
    {
        get => _dungeonData;
        set
        {
            if (value == null) { throw new ArgumentNullException(); }
            // if (_dungeonData == value) { return; }
            if (_dungeonData != null)
            {
                RemoveDungeonReferences(_dungeonData.Dungeon);
                _dungeonData.OnChange.RemoveListener(HandleDungeonChanged);
            }
            _dungeonData = value;
            _dungeonData.OnChange.AddListener(HandleDungeonChanged);
            if (_dungeonData?.Dungeon != null)
            {
                AddDungeonReferences(_dungeonData.Dungeon);
            }
            Debug.Log("MaterialCache.DungeonData set");
        }
    }

    private void HandleDungeonChanged(DungeonChangedData changes)
    {
        Debug.Log("DungeonData loaded");
        RemoveDungeonReferences(changes.Previous);
        AddDungeonReferences(changes.New);
    }

    public Dictionary<string, SelectableMaterial> InitializeMaterialCache(DungeonManifestData manifestData)
    {
        Debug.Log("Initializing Cache");
        Clear();
        _manifestData = manifestData;
        BuildMaterials();
        BuildReferences(_manifestData.Manifest.Dungeons.Values);
        if (_dungeonData?.Dungeon != null)
        {
            AddDungeonReferences(_dungeonData.Dungeon);
        }
        _onCacheChanged.Invoke(new CacheUpdateData(_materialCache, true));
        return _materialCache;

        void BuildMaterials()
        {
            foreach ((string textureName, Texture texture) in Manifest.Textures)
            {
                SelectableMaterial material = new(texture.ToMaterial());
                _materialCache[textureName] = material;
                _references[material.Id] = new TextureReferences(textureName);
            }
        }
    }

    private void RemoveDungeonReferences(Dungeon dungeon)
    {
        foreach ((Position position, string _) in dungeon.TileTextures.Textures)
        {
            TileReference reference = new(dungeon, position);
            if (_reverseLookup.TryGetValue(reference, out TextureId tId))
            {
                _references[tId].Tiles.Remove(reference);
            }
        }
    }


    private void AddDungeonReferences(Dungeon dungeon)
    {
        foreach ((Position position, string textureName) in dungeon.TileTextures.Textures)
        {
            if (!_materialCache.TryGetValue(textureName, out SelectableMaterial material))
            {
                // TODO: Consider defaulting to "default-tile.png" and removing the reference in the dungeon
                throw new System.Exception($"Dungeon references texture '{textureName}' that was not found in material cache.");
            }
            TileReference reference = new(dungeon, position);
            _references[material.Id].Tiles.Add(reference);
            _reverseLookup[reference] = material.Id;
        }
    }

    private void BuildReferences(IEnumerable<Dungeon> dungeons)
    {
        foreach (Dungeon dungeon in dungeons)
        {
            AddDungeonReferences(dungeon);
        }
    }

    // public void SetTexture(TileReference tile, TextureId tId)
    // {
    //     TextureReferences references = _references[tId];
    //     Manifest.Dungeons[tile.DungeonName].TileTextures.Textures[tile.Position] = references.TextureName;
    //     if (_reverseLookup.TryGetValue(tile, out TextureId originalId))
    //     {
    //         _references[originalId].Tiles.Remove(tile);
    //     }
    //     _references[tId].Tiles.Add(tile);
    // }

    public void RemoveTextureReference(TextureId id)
    {
        foreach (TileReference tileRef in _references[id].Tiles)
        {
            tileRef.Dungeon.TileTextures.Textures.Remove(tileRef.Position);
            if (tileRef.Dungeon == DungeonData.Dungeon)
            {
                DungeonData.RemoveFloorTexture(tileRef.Position);
            }
        }
        _references[id].Clear();
        DungeonData.Notify();
    }

    public void RemoveListener(UnityAction<CacheUpdateData> onChange) => _onCacheChanged.RemoveListener(onChange);

    public void AddListener(UnityAction<CacheUpdateData> onChange)
    {
        _onCacheChanged.AddListener(onChange);
        if (_materialCache != null)
        {
            onChange.Invoke(new CacheUpdateData(_materialCache, true));
        }
    }

    public void AddTexture(string name, Texture2D texture)
    {
        if (Manifest.Textures.ContainsKey(name)) { return; }
        Texture dungeonTexture = new(name, ImageConversion.EncodeToPNG(texture));
        Manifest.AddTexture(dungeonTexture);
        _materialCache.Add(name, new SelectableMaterial(dungeonTexture.ToMaterial()));
        _onCacheChanged.Invoke(new CacheUpdateData(_materialCache, false, name));
    }

    public void Clear()
    {
        _reverseLookup.Clear();
        _references.Clear();
        _materialCache.Clear();
        _onCacheChanged.RemoveAllListeners();
        _manifestData = null;
    }

    public SelectableMaterial GetTileMaterial(Dungeon d, Position p) => _materialCache.GetTileMaterial(d, p);
    public TileWallMaterials GetTileWallMaterials(Dungeon d, Position p) => _materialCache.GetTileWallMaterials(d, p);
    public SelectableMaterial GetMaterial(string textureName) => _materialCache[textureName];
}


public class CacheUpdateData
{
    public Dictionary<string, SelectableMaterial> Cache { get; private set; }
    public IEnumerable<string> Added { get; private set; }
    public readonly bool IsNewCache;

    public CacheUpdateData(Dictionary<string, SelectableMaterial> cache, bool isNew, params string[] added)
    {
        IsNewCache = isNew;
        Cache = cache;
        Added = added;
    }
}