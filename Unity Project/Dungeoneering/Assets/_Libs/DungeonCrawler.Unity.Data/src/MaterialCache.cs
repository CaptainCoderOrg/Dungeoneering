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
    public static readonly TextureId DefaultTextureId = new(0);
    // TODO: Consider creating new data structure to manage _references and reverse references
    private readonly Dictionary<TextureId, TextureReferences> _references = new();
    private readonly Dictionary<TileReference, TextureReferences> _tileReferenceToTextureId = new();
    private readonly Dictionary<WallReference, TextureReferences> _wallReferenceToTextureId = new();
    private readonly UnityEvent<CacheUpdateData> _onCacheChanged = new();
    private DungeonCrawlerManifest _manifest;
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

    public void InitializeMaterialCache(DungeonCrawlerManifest manifest)
    {
        Debug.Log("Initializing Cache");
        Clear();
        _manifest = manifest;
        BuildMaterials();
        BuildReferences(_manifest.Dungeons.Values);
        if (_dungeonData?.Dungeon != null)
        {
            AddDungeonReferences(_dungeonData.Dungeon);
        }
        _onCacheChanged.Invoke(new CacheInitialized(_references.Values.Select(r => r.Material)));

        void BuildMaterials()
        {
            foreach ((string textureName, Texture texture) in _manifest.Textures)
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
            TileReference tileRef = new(dungeon, position);
            if (_tileReferenceToTextureId.TryGetValue(tileRef, out TextureReferences textureRef))
            {
                textureRef.Tiles.Remove(tileRef);
            }
        }

        foreach (((Position position, Facing facing), string _) in dungeon.WallTextures.Textures)
        {
            WallReference wallRef = new(dungeon, position, facing);
            if (_wallReferenceToTextureId.TryGetValue(wallRef, out TextureReferences tRef))
            {
                tRef.Walls.Remove(wallRef);
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
            _tileReferenceToTextureId[tileRef] = textureRef;
        }

        foreach (((Position position, Facing facing), string textureName) in dungeon.WallTextures.Textures)
        {
            TextureReferences textureRef = TextureReferences.FromName(textureName);
            WallReference wallRef = new(dungeon, position, facing);
            textureRef.Walls.Add(wallRef);
            _wallReferenceToTextureId[wallRef] = textureRef;
        }
    }

    private void BuildReferences(IEnumerable<Dungeon> dungeons)
    {
        foreach (Dungeon dungeon in dungeons)
        {
            AddDungeonReferences(dungeon);
        }
    }

    public void SetTexture(WallReference wallRef, TextureId tId)
    {
        if (_wallReferenceToTextureId.TryGetValue(wallRef, out TextureReferences oldRef))
        {
            oldRef.Walls.Remove(wallRef);
        }
        TextureReferences newTexture = _references[tId];
        newTexture.Walls.Add(wallRef);
        wallRef.Dungeon.WallTextures.Textures[(wallRef.Position, wallRef.Facing)] = newTexture.TextureName;
        _wallReferenceToTextureId[wallRef] = newTexture;
    }

    public void SetTexture(TileReference tileRef, TextureId tId)
    {
        if (_tileReferenceToTextureId.TryGetValue(tileRef, out TextureReferences oldRef))
        {
            oldRef.Tiles.Remove(tileRef);
        }
        TextureReferences newTexture = _references[tId];
        newTexture.Tiles.Add(tileRef);
        tileRef.Dungeon.TileTextures.Textures[tileRef.Position] = newTexture.TextureName;
        _tileReferenceToTextureId[tileRef] = newTexture;
    }

    public void RemoveTextureReference(TextureId id)
    {
        TextureReferences textureRef = _references[id];
        RemoveTileTextureReferences(textureRef);
        RemoveWallTextureReferences(textureRef);
        TextureReferences.Remove(textureRef.TextureName);
        _manifest.Textures.Remove(textureRef.TextureName);
        _references.Remove(id);

        DungeonData.Notify();
        _onCacheChanged.Invoke(new CacheRemoveTexture(id));
    }

    private void RemoveWallTextureReferences(TextureReferences textureRef)
    {
        foreach (WallReference wallRef in textureRef.Walls)
        {
            _wallReferenceToTextureId.Remove(wallRef);
            wallRef.Dungeon.WallTextures.Textures.Remove((wallRef.Position, wallRef.Facing));
            if (wallRef.Dungeon == DungeonData.Dungeon)
            {
                DungeonData.RemoveWallTexture(wallRef.Position, wallRef.Facing);
            }
        }
    }

    private void RemoveTileTextureReferences(TextureReferences textureRef)
    {
        foreach (TileReference tileRef in textureRef.Tiles)
        {
            _tileReferenceToTextureId.Remove(tileRef);
            tileRef.Dungeon.TileTextures.Textures.Remove(tileRef.Position);
            if (tileRef.Dungeon == DungeonData.Dungeon)
            {
                DungeonData.RemoveFloorTexture(tileRef.Position);
            }
        }
    }

    public void RemoveListener(UnityAction<CacheUpdateData> onChange) => _onCacheChanged.RemoveListener(onChange);

    public void AddListener(UnityAction<CacheUpdateData> onChange)
    {
        _onCacheChanged.AddListener(onChange);
        onChange.Invoke(new CacheInitialized(_references.Values.Select(r => r.Material)));
    }

    public void AddTexture(string name, Texture2D texture)
    {
        if (_manifest.Textures.ContainsKey(name)) { return; }
        Texture dungeonTexture = new(name, ImageConversion.EncodeToPNG(texture));
        _manifest.AddTexture(dungeonTexture);
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
        _manifest = null;
    }

    public SelectableMaterial GetTileMaterial(Dungeon d, Position p)
    {
        if (_tileReferenceToTextureId.TryGetValue(new TileReference(d, p), out TextureReferences tRef))
        {
            return tRef.Material;
        }
        string defaultTextureName = d.TileTextures.GetTileTextureName(p);
        return TextureReferences.FromName(defaultTextureName).Material;
    }
    public TileWallMaterials GetTileWallMaterials(Dungeon d, Position p) => TextureReferences.GetTileWallMaterials(d, p);
    public SelectableMaterial GetMaterial(string textureName) => TextureReferences.FromName(textureName).Material;
    public TextureId GetFloorTexture(Dungeon dungeon, Position p)
    {
        if (_tileReferenceToTextureId.TryGetValue(new TileReference(dungeon, p), out TextureReferences tRef))
        {
            return tRef.TextureId;
        }
        return DefaultTextureId;
    }

    internal TextureId GetWallTexture(WallReference wallReference)
    {
        if (_wallReferenceToTextureId.TryGetValue(wallReference, out TextureReferences tRef))
        {
            return tRef.TextureId;
        }
        return DefaultTextureId;
    }
}

public abstract record class CacheUpdateData;
public sealed record class CacheInitialized(IEnumerable<SelectableMaterial> Materials) : CacheUpdateData;
public sealed record class CacheAddTexture(SelectableMaterial Material) : CacheUpdateData;
public sealed record class CacheRemoveTexture(TextureId Removed) : CacheUpdateData;