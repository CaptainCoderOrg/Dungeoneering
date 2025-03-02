using System;
using System.Collections.Generic;

using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap;

using UnityEngine;
using UnityEngine.Events;

namespace CaptainCoder.Dungeoneering.Unity.Data;

public class MaterialCache
{
    private TextureReferences _textureReferences = new();
    private readonly Dictionary<TileReference, TextureReference> _tileReferences = new();
    private readonly Dictionary<WallReference, TextureReference> _wallReferences = new();
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
        _onCacheChanged.Invoke(new CacheInitialized(_textureReferences.Textures));

        void BuildMaterials()
        {
            foreach (Texture texture in _manifest.Textures.Values)
            {
                _textureReferences.Create(texture);
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
            if (_tileReferences.TryGetValue(tileRef, out TextureReference textureRef))
            {
                textureRef.Tiles.Remove(tileRef);
            }
        }

        foreach (((Position position, Facing facing), string _) in dungeon.WallTextures.Textures)
        {
            WallReference wallRef = new(dungeon, position, facing);
            if (_wallReferences.TryGetValue(wallRef, out TextureReference tRef))
            {
                tRef.Walls.Remove(wallRef);
            }
        }
    }

    private void AddDungeonReferences(Dungeon dungeon)
    {
        foreach ((Position position, string textureName) in dungeon.TileTextures.Textures)
        {
            TextureReference textureRef = _textureReferences.FromName(textureName);
            TileReference tileRef = new(dungeon, position);
            textureRef.Tiles.Add(tileRef);
            _tileReferences[tileRef] = textureRef;
        }

        foreach (((Position position, Facing facing), string textureName) in dungeon.WallTextures.Textures)
        {
            TextureReference textureRef = _textureReferences.FromName(textureName);
            WallReference wallRef = new(dungeon, position, facing);
            textureRef.Walls.Add(wallRef);
            _wallReferences[wallRef] = textureRef;
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
        if (_wallReferences.TryGetValue(wallRef, out TextureReference oldRef))
        {
            oldRef.Walls.Remove(wallRef);
        }

        if (tId == _textureReferences.DefaultTextureId)
        {
            // Use the default texture for this dungeon
            _wallReferences.Remove(wallRef);
            wallRef.Dungeon.WallTextures.Textures.Remove((wallRef.Position, wallRef.Facing));
        }
        else
        {
            TextureReference newTexture = _textureReferences.FromId(tId);
            newTexture.Walls.Add(wallRef);
            wallRef.Dungeon.WallTextures.Textures[(wallRef.Position, wallRef.Facing)] = newTexture.TextureName;
            _wallReferences[wallRef] = newTexture;
        }

    }

    public void SetTexture(TileReference tileRef, TextureId tId)
    {
        if (_tileReferences.TryGetValue(tileRef, out TextureReference oldRef))
        {
            oldRef.Tiles.Remove(tileRef);
        }
        if (tId == _textureReferences.DefaultTextureId)
        {
            // Use the default texture for this dungeon
            _tileReferences.Remove(tileRef);
            tileRef.Dungeon.TileTextures.Textures.Remove(tileRef.Position);
        }
        else
        {
            TextureReference newTexture = _textureReferences.FromId(tId);
            newTexture.Tiles.Add(tileRef);
            _tileReferences[tileRef] = newTexture;
            tileRef.Dungeon.TileTextures.Textures[tileRef.Position] = newTexture.TextureName;
        }

    }

    public void RemoveTextureReference(TextureReference textureRef)
    {
        RemoveTileTextureReferences(textureRef);
        RemoveWallTextureReferences(textureRef);
        _manifest.Textures.Remove(textureRef.TextureName);
        _textureReferences.Remove(textureRef);
        DungeonData.Notify();
        _onCacheChanged.Invoke(new CacheRemoveTexture(textureRef));
    }

    private void RemoveWallTextureReferences(TextureReference textureRef)
    {
        foreach (WallReference wallRef in textureRef.Walls)
        {
            _wallReferences.Remove(wallRef);
            wallRef.Dungeon.WallTextures.Textures.Remove((wallRef.Position, wallRef.Facing));
            if (wallRef.Dungeon == DungeonData.Dungeon)
            {
                DungeonData.RemoveWallTexture(wallRef.Position, wallRef.Facing);
            }
        }
    }

    private void RemoveTileTextureReferences(TextureReference textureRef)
    {
        foreach (TileReference tileRef in textureRef.Tiles)
        {
            _tileReferences.Remove(tileRef);
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
        onChange.Invoke(new CacheInitialized(_textureReferences.Textures));
    }

    public void AddTexture(string name, Texture2D texture)
    {
        if (_manifest.Textures.ContainsKey(name)) { return; }
        Texture dungeonTexture = new(name, ImageConversion.EncodeToPNG(texture));
        _manifest.AddTexture(dungeonTexture);
        TextureReference created = _textureReferences.Create(dungeonTexture);
        _onCacheChanged.Invoke(new CacheAddTexture(created));
    }

    public void Clear()
    {
        _textureReferences.Clear();
        _tileReferences.Clear();
        _onCacheChanged.RemoveAllListeners();
        _manifest = null;
    }

    public TextureReference GetTexture(Dungeon d, Position p)
    {
        if (_tileReferences.TryGetValue(new TileReference(d, p), out TextureReference tRef))
        {
            return tRef;
        }
        string defaultTextureName = d.TileTextures.GetTileTextureName(p);
        return _textureReferences.FromName(defaultTextureName);
    }
    public TileWallTextures GetTileWallMaterials(Dungeon d, Position p) => _textureReferences.GetTileWallMaterials(d, p);
    public TextureReference GetTexture(string textureName) => _textureReferences.FromName(textureName);
    public TextureId GetFloorTexture(Dungeon dungeon, Position p)
    {
        if (_tileReferences.TryGetValue(new TileReference(dungeon, p), out TextureReference tRef))
        {
            return tRef.TextureId;
        }
        return _textureReferences.DefaultTextureId;
    }

    internal TextureId GetWallTexture(WallReference wallReference)
    {
        if (_wallReferences.TryGetValue(wallReference, out TextureReference tRef))
        {
            return tRef.TextureId;
        }
        return _textureReferences.DefaultTextureId;
    }
}

public abstract record class CacheUpdateData;
public sealed record class CacheInitialized(IEnumerable<TextureReference> Materials) : CacheUpdateData;
public sealed record class CacheAddTexture(TextureReference Material) : CacheUpdateData;
public sealed record class CacheRemoveTexture(TextureReference Removed) : CacheUpdateData;