using System.Collections.Generic;

using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity.Data;

public class MaterialCache
{
    private TextureDatabase _textureDatabase = new();
    private readonly Dictionary<TileReference, TextureReference> _tileReferences = new();
    private readonly Dictionary<WallReference, TextureReference> _wallReferences = new();
    private System.Action<CacheUpdateData> _onCacheChanged;
    private DungeonCrawlerManifest _manifest;
    internal void InitializeMaterialCache(DungeonCrawlerManifest manifest)
    {
        Debug.Log("Initializing Cache");
        Clear();
        _manifest = manifest;
        BuildMaterials();
        BuildReferences(_manifest.Dungeons.Values);
        _onCacheChanged?.Invoke(new CacheInitialized(_textureDatabase.Textures));

        void BuildMaterials()
        {
            foreach (Texture texture in _manifest.Textures.Values)
            {
                _textureDatabase.Create(texture);
            }
        }
    }

    internal void RemoveDungeonReferences(Dungeon dungeon)
    {
        if (dungeon == null)
        {
            Debug.LogWarning("Cannot remove references to a null dungeon.");
            return;
        }

        _textureDatabase.FromName(dungeon.WallTextures.DefaultDoor).DefaultDoorDungeons.Remove(dungeon);
        _textureDatabase.FromName(dungeon.WallTextures.DefaultSecretDoor).DefaultSecretDungeons.Remove(dungeon);
        _textureDatabase.FromName(dungeon.WallTextures.DefaultSolid).DefaultSolidDungeons.Remove(dungeon);
        _textureDatabase.FromName(dungeon.TileTextures.Default).DefaultTileDungeons.Remove(dungeon);

        foreach ((Position position, string _) in dungeon.TileTextures.Textures)
        {
            TileReference tileRef = new(dungeon, position);
            if (_tileReferences.Remove(tileRef, out TextureReference textureRef))
            {
                textureRef.Tiles.Remove(tileRef);
            }
        }

        foreach (((Position position, Facing facing), string _) in dungeon.WallTextures.Textures)
        {
            WallReference wallRef = new(dungeon, position, facing);
            if (_wallReferences.Remove(wallRef, out TextureReference tRef))
            {
                tRef.Walls.Remove(wallRef);
            }
        }
    }

    internal void AddDungeonReferences(Dungeon dungeon)
    {
        _textureDatabase.FromName(dungeon.WallTextures.DefaultDoor).DefaultDoorDungeons.Add(dungeon);
        _textureDatabase.FromName(dungeon.WallTextures.DefaultSecretDoor).DefaultSecretDungeons.Add(dungeon);
        _textureDatabase.FromName(dungeon.WallTextures.DefaultSolid).DefaultSolidDungeons.Add(dungeon);
        _textureDatabase.FromName(dungeon.TileTextures.Default).DefaultTileDungeons.Add(dungeon);

        foreach ((Position position, string textureName) in dungeon.TileTextures.Textures)
        {
            TextureReference textureRef = _textureDatabase.FromName(textureName);
            TileReference tileRef = new(dungeon, position);
            textureRef.Tiles.Add(tileRef);
            _tileReferences[tileRef] = textureRef;
        }

        foreach (((Position position, Facing facing), string textureName) in dungeon.WallTextures.Textures)
        {
            TextureReference textureRef = _textureDatabase.FromName(textureName);
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

    internal void SetTexture(WallReference wallRef, TextureReference newTexture)
    {
        if (_wallReferences.TryGetValue(wallRef, out TextureReference oldRef))
        {
            oldRef.Walls.Remove(wallRef);
        }

        newTexture.Walls.Add(wallRef);
        wallRef.Dungeon.WallTextures.Textures[(wallRef.Position, wallRef.Facing)] = newTexture.TextureName;
        _wallReferences[wallRef] = newTexture;

    }

    internal void SetTexture(TileReference tileRef, TextureReference newTexture)
    {
        if (_tileReferences.TryGetValue(tileRef, out TextureReference oldRef))
        {
            oldRef.Tiles.Remove(tileRef);
        }

        newTexture.Tiles.Add(tileRef);
        _tileReferences[tileRef] = newTexture;
        tileRef.Dungeon.TileTextures.Textures[tileRef.Position] = newTexture.TextureName;
    }

    internal void RemoveTexture(TileReference tileRef)
    {
        if (_tileReferences.TryGetValue(tileRef, out TextureReference oldRef))
        {
            oldRef.Tiles.Remove(tileRef);
            _tileReferences.Remove(tileRef);
        }
    }

    internal void RemoveTexture(WallReference wallRef)
    {
        if (_wallReferences.TryGetValue(wallRef, out TextureReference oldRef))
        {
            oldRef.Walls.Remove(wallRef);
            _wallReferences.Remove(wallRef);
        }
    }

    internal void DeleteTexture(TextureReference textureRef)
    {
        if (textureRef.IsDefaultTexture)
        {
            throw new System.InvalidOperationException($"Cannot remove a default texture: {textureRef.TextureName}");
        }
        RemoveTileTextureReferences(textureRef);
        RemoveWallTextureReferences(textureRef);
        _manifest.Textures.Remove(textureRef.TextureName);
        _textureDatabase.Remove(textureRef);
        _onCacheChanged?.Invoke(new CacheRemoveTexture(textureRef));
    }

    private void RemoveWallTextureReferences(TextureReference textureRef)
    {
        foreach (WallReference wallRef in textureRef.Walls)
        {
            _wallReferences.Remove(wallRef);
            wallRef.Dungeon.WallTextures.Textures.Remove((wallRef.Position, wallRef.Facing));
        }
    }

    private void RemoveTileTextureReferences(TextureReference textureRef)
    {
        foreach (TileReference tileRef in textureRef.Tiles)
        {
            _tileReferences.Remove(tileRef);
            tileRef.Dungeon.TileTextures.Textures.Remove(tileRef.Position);
        }
    }

    internal void RemoveObserver(System.Action<CacheUpdateData> onChange) => _onCacheChanged -= onChange;

    internal void AddObserver(System.Action<CacheUpdateData> onChange)
    {
        _onCacheChanged += onChange;
        onChange.Invoke(new CacheInitialized(_textureDatabase.Textures));
    }

    internal void CreateTexture(string name, Texture2D texture)
    {
        if (_manifest.Textures.ContainsKey(name)) { return; }
        Texture dungeonTexture = new(name, ImageConversion.EncodeToPNG(texture));
        _manifest.AddTexture(dungeonTexture);
        TextureReference created = _textureDatabase.Create(dungeonTexture);
        _onCacheChanged?.Invoke(new CacheAddTexture(created));
    }

    internal void Clear()
    {
        _textureDatabase.Clear();
        _tileReferences.Clear();
        _manifest = null;
    }

    internal bool HasReference(TileReference tileReference) => _tileReferences.ContainsKey(tileReference);

    internal TextureReference GetTexture(TileReference tileReference)
    {
        if (_tileReferences.TryGetValue(tileReference, out TextureReference tRef))
        {
            return tRef;
        }
        string defaultTextureName = tileReference.Dungeon.TileTextures.GetTileTextureName(tileReference.Position);
        return _textureDatabase.FromName(defaultTextureName);
    }

    internal TileWallTextures GetTileWallTextures(TileReference tileRef) => _textureDatabase.GetTileWallMaterials(tileRef.Dungeon, tileRef.Position);
    internal TextureReference GetTexture(string textureName) => _textureDatabase.FromName(textureName);

    internal TextureReference GetTexture(WallReference wallReference)
    {
        if (_wallReferences.TryGetValue(wallReference, out TextureReference tRef))
        {
            return tRef;
        }
        return _textureDatabase.FromName(wallReference.Dungeon.GetWallTexture(wallReference.Position, wallReference.Facing));
    }
}

public abstract record class CacheUpdateData;
public sealed record class CacheInitialized(IEnumerable<TextureReference> Materials) : CacheUpdateData;
public sealed record class CacheAddTexture(TextureReference Material) : CacheUpdateData;
public sealed record class CacheRemoveTexture(TextureReference Removed) : CacheUpdateData;