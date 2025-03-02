using System;
using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using CaptainCoder.Dungeoneering.Unity.Data;

using UnityEngine;
using UnityEngine.Events;

namespace CaptainCoder.Dungeoneering.Unity;

public class MaterialCache
{
    public static readonly TextureId DefaultTexture = new(0);
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

    public void InitializeMaterialCache(DungeonManifestData manifestData)
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
        _onCacheChanged.Invoke(new CacheInitialized(_references.Values.Select(r => r.Material)));

        void BuildMaterials()
        {
            foreach ((string textureName, Texture texture) in Manifest.Textures)
            {
                SelectableMaterial material = new(texture.ToMaterial());
                _references[material.Id] = new TextureReferences(textureName, material);
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
            TextureReferences textureRef = TextureReferences.FromName(textureName);
            TileReference tileRef = new(dungeon, position);
            textureRef.Tiles.Add(tileRef);
            _tileReferenceToTextureId[tileRef] = textureRef.TextureId;
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
        TextureReferences.Remove(references.TextureName);
        _manifestData.Manifest.Textures.Remove(references.TextureName);
        _references.Remove(id);
        DungeonData.Notify();
    }

    public void RemoveListener(UnityAction<CacheUpdateData> onChange) => _onCacheChanged.RemoveListener(onChange);

    public void AddListener(UnityAction<CacheUpdateData> onChange)
    {
        _onCacheChanged.AddListener(onChange);
        onChange.Invoke(new CacheInitialized(_references.Values.Select(r => r.Material)));
    }

    public void AddTexture(string name, Texture2D texture)
    {
        if (Manifest.Textures.ContainsKey(name)) { return; }
        Texture dungeonTexture = new(name, ImageConversion.EncodeToPNG(texture));
        Manifest.AddTexture(dungeonTexture);
        SelectableMaterial material = new(dungeonTexture.ToMaterial());
        TextureReferences references = new(name, material);
        _references[material.Id] = references;
        _onCacheChanged.Invoke(new CacheAddTexture(material));
    }

    public void Clear()
    {
        _tileReferenceToTextureId.Clear();
        _references.Clear();
        _onCacheChanged.RemoveAllListeners();
        _manifestData = null;
    }

    public SelectableMaterial GetTileMaterial(Dungeon d, Position p)
    {
        if (_tileReferenceToTextureId.TryGetValue(new TileReference(d, p), out TextureId tId))
        {
            return _references[tId].Material;
        }
        // TODO: Dungeons need a default TextureId for tiles / walls / doors / etc
        string defaultTextureName = d.TileTextures.GetTileTextureName(p);
        return TextureReferences.FromName(defaultTextureName).Material;
    }
    public TileWallMaterials GetTileWallMaterials(Dungeon d, Position p) => TextureReferences.GetTileWallMaterials(d, p);
    public SelectableMaterial GetMaterial(string textureName) => TextureReferences.FromName(textureName).Material;
    public TextureId GetFloorTexture(Dungeon dungeon, Position p) => _tileReferenceToTextureId.GetValueOrDefault(new TileReference(dungeon, p), DefaultTexture);
}

public abstract record class CacheUpdateData;
public sealed record class CacheInitialized(IEnumerable<SelectableMaterial> Materials) : CacheUpdateData;
public sealed record class CacheAddTexture(SelectableMaterial Material) : CacheUpdateData;
public sealed record class CacheRemoveTexture(TextureId Removed) : CacheUpdateData;