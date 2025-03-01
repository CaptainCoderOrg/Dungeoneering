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
    public static readonly TextureId DefaultTexture = new(0);
    // TODO: Eliminate _materialCache in favor of _references
    private readonly Dictionary<string, SelectableMaterial> _materialCache = new();
    // TODO: Consider creating new data structure to manage _references and reverse references
    private readonly Dictionary<TextureId, TextureReferences> _references = new();
    private readonly Dictionary<TileReference, TextureId> _tileReferenceToTextureId = new();
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
        }
    }

    private void HandleDungeonChanged(DungeonChangedData changes)
    {
        RemoveDungeonReferences(changes.Previous);
        AddDungeonReferences(changes.New);
    }

    // public TextureId GetTextureId(string textureName) => _materialCache[textureName].Id;

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
        _onCacheChanged.Invoke(new CacheInitialized(_materialCache.Values));
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
        if (dungeon == null)
        {
            Debug.LogWarning("Cannot remove references to a null dungeon.");
            return;
        }
        foreach ((Position position, string _) in dungeon.TileTextures.Textures)
        {
            TileReference reference = new(dungeon, position);
            if (_tileReferenceToTextureId.TryGetValue(reference, out TextureId tId))
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
            _tileReferenceToTextureId[reference] = material.Id;
        }
    }

    private void BuildReferences(IEnumerable<Dungeon> dungeons)
    {
        foreach (Dungeon dungeon in dungeons)
        {
            AddDungeonReferences(dungeon);
        }
    }

    public void SetTexture(TileReference reference, TextureId tId)
    {
        if (_tileReferenceToTextureId.TryGetValue(reference, out TextureId oldId))
        {
            _references[oldId].Tiles.Remove(reference);
        }
        reference.Dungeon.TileTextures.Textures[reference.Position] = _references[tId].TextureName;
        _references[tId].Tiles.Add(reference);
    }

    public void RemoveTextureReference(TextureId id)
    {
        RemoveTileTextureReferences(id);
        RemoveWallTextureReferences(id);
        _onCacheChanged.Invoke(new CacheRemoveTexture(id));
    }

    private void RemoveWallTextureReferences(TextureId id)
    {
        // TODO: Remove Wall References
        Debug.LogWarning($"Wall texture references were not removed!");
    }

    private void RemoveTileTextureReferences(TextureId id)
    {
        TextureReferences references = _references[id];
        foreach (TileReference tileRef in references.Tiles)
        {
            _tileReferenceToTextureId.Remove(tileRef);
            tileRef.Dungeon.TileTextures.Textures.Remove(tileRef.Position);
            if (tileRef.Dungeon == DungeonData.Dungeon)
            {
                DungeonData.RemoveFloorTexture(tileRef.Position);
            }
        }
        _materialCache.Remove(references.TextureName);
        _manifestData.Manifest.Textures.Remove(references.TextureName);
        _references.Remove(id);
        DungeonData.Notify();
    }

    public void RemoveListener(UnityAction<CacheUpdateData> onChange) => _onCacheChanged.RemoveListener(onChange);

    public void AddListener(UnityAction<CacheUpdateData> onChange)
    {
        _onCacheChanged.AddListener(onChange);
        if (_materialCache != null)
        {
            onChange.Invoke(new CacheInitialized(_materialCache.Values));
        }
    }

    public void AddTexture(string name, Texture2D texture)
    {
        if (Manifest.Textures.ContainsKey(name)) { return; }
        Texture dungeonTexture = new(name, ImageConversion.EncodeToPNG(texture));
        Manifest.AddTexture(dungeonTexture);
        SelectableMaterial material = new(dungeonTexture.ToMaterial());
        _materialCache[name] = material;
        TextureReferences references = new(name);
        _references[material.Id] = references;
        _onCacheChanged.Invoke(new CacheAddTexture(material));
    }

    public void Clear()
    {
        _tileReferenceToTextureId.Clear();
        _references.Clear();
        _materialCache.Clear();
        _onCacheChanged.RemoveAllListeners();
        _manifestData = null;
    }

    public SelectableMaterial GetTileMaterial(Dungeon d, Position p) => _materialCache.GetTileMaterial(d, p);
    public TileWallMaterials GetTileWallMaterials(Dungeon d, Position p) => _materialCache.GetTileWallMaterials(d, p);
    public SelectableMaterial GetMaterial(string textureName) => _materialCache[textureName];
    public TextureId GetFloorTexture(Dungeon dungeon, Position p) => _tileReferenceToTextureId.GetValueOrDefault(new TileReference(dungeon, p), DefaultTexture);
}

public abstract record class CacheUpdateData;
public sealed record class CacheInitialized(IEnumerable<SelectableMaterial> Materials) : CacheUpdateData;
public sealed record class CacheAddTexture(SelectableMaterial Material) : CacheUpdateData;
public sealed record class CacheRemoveTexture(TextureId Removed) : CacheUpdateData;


// public class CacheUpdateData
// {
//     public Dictionary<string, SelectableMaterial> Cache { get; private set; }
//     public IEnumerable<string> Added { get; private set; }
//     public IEnumerable<TextureId> Removed { get; private set; }
//     public readonly bool IsNewCache;

//     public CacheUpdateData(Dictionary<string, SelectableMaterial> cache, bool isNew, IEnumerable<string> added, IEnumerable<TextureId> removed)
//     {
//         IsNewCache = isNew;
//         Cache = cache;
//         Added = added;
//         Removed = removed;
//     }
// }