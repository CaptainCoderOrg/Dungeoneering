using System;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.IO;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity.Data;

public static class DungeonCrawlerDataExtensions
{
    /// <summary>
    /// Save the current DungeonData with the ManifestData
    /// </summary>
    /// <param name="data"></param>
    public static void SyncWithManifest(this DungeonCrawlerData data)
    {
        Dungeon copy = data.CurrentDungeon.Copy();
        data.MaterialCache.RemoveDungeonReferences(data.Manifest.Dungeons[copy.Name]);
        data.MaterialCache.AddDungeonReferences(copy);
        data.Manifest.Dungeons[copy.Name] = copy;
        data.CurrentDungeonData.HasChanged = false;
    }

    /// <summary>
    /// Sets the specified tile to use the new texture
    /// </summary>
    public static void SetTexture(this DungeonCrawlerData data, TileReference tile, TextureReference newTexture)
    {
        data.MaterialCache.SetTexture(tile, newTexture);
        if (data.CurrentDungeon == tile.Dungeon)
        {
            data.CurrentDungeonData.AddChange(tile);
        }
    }

    /// <summary>
    /// Sets the specified wall to use the new texture
    /// </summary>
    public static void SetTexture(this DungeonCrawlerData data, WallReference wall, TextureReference newTexture)
    {
        data.MaterialCache.SetTexture(wall, newTexture);
        if (data.CurrentDungeon == wall.Dungeon)
        {
            data.CurrentDungeonData.AddChange(wall);
        }
    }

    /// <summary>
    /// Sets the specified tile to use the default texture
    /// </summary>
    public static void UseDefaultTexture(this DungeonCrawlerData data, TileReference tileRef)
    {
        tileRef.Dungeon.TileTextures.Textures.Remove(tileRef.Position);
        data.MaterialCache.RemoveTexture(tileRef);
        if (data.CurrentDungeon == tileRef.Dungeon)
        {
            data.CurrentDungeonData.AddChange(tileRef);
        }
    }

    /// <summary>
    /// Sets the specified wall to use the default texture
    /// </summary>
    public static void UseDefaultTexture(this DungeonCrawlerData data, WallReference wallRef)
    {
        wallRef.Dungeon.WallTextures.Textures.Remove((wallRef.Position, wallRef.Facing));
        data.MaterialCache.RemoveTexture(wallRef);
        if (data.CurrentDungeon == wallRef.Dungeon)
        {
            data.CurrentDungeonData.AddChange(wallRef);
        }
    }

    /// <summary>
    /// Sets the default tile texture for the specified dungeon to the new texture
    /// </summary>
    public static void SetDefaultTileTexture(this DungeonCrawlerData data, Dungeon targetDungeon, TextureReference newTexture)
    {
        if (!data.Manifest.Dungeons.TryGetValue(targetDungeon.Name, out Dungeon dungeon))
        {
            throw new System.InvalidOperationException($"The specified dungeon {targetDungeon.Name} does not exist in the manifest.");
        }
        TextureReference previousTexture = data.MaterialCache.GetTexture(dungeon.TileTextures.Default);
        dungeon.TileTextures.Default = newTexture.TextureName;
        previousTexture.DefaultTileDungeons.Remove(dungeon);
        newTexture.DefaultTileDungeons.Add(dungeon);
        if (targetDungeon.Name == data.CurrentDungeon.Name)
        {
            previousTexture.DefaultTileDungeons.Remove(data.CurrentDungeon);
            newTexture.DefaultTileDungeons.Add(data.CurrentDungeon);
            data.CurrentDungeonData.SetDefaultTileTexture(newTexture);
        }
    }

    /// <summary>
    /// Sets the default texture for the specified dungeon and wall type.
    /// </summary>
    public static void SetDefaultWallTexture(this DungeonCrawlerData data, Dungeon targetDungeon, TextureReference newTexture, WallType wallType)
    {
        if (!data.Manifest.Dungeons.TryGetValue(targetDungeon.Name, out Dungeon dungeon))
        {
            throw new System.InvalidOperationException($"The specified dungeon {targetDungeon.Name} does not exist in the manifest.");
        }
        TextureReference previousTexture = data.MaterialCache.GetTexture(dungeon.WallTextures.DefaultSolid);
        dungeon.WallTextures.SetDefaultTexture(wallType, newTexture.TextureName);
        previousTexture.RemoveDefaultWall(wallType, dungeon);
        newTexture.AddDefaultWall(wallType, dungeon);
        if (targetDungeon.Name == data.CurrentDungeon.Name)
        {
            previousTexture.RemoveDefaultWall(wallType, data.CurrentDungeon);
            newTexture.AddDefaultWall(wallType, data.CurrentDungeon);
            data.CurrentDungeonData.SetDefaultWallTexture(wallType, newTexture);
        }
    }

    internal static void SetDefaultTexture(this WallTextureMap wallTextures, WallType wallType, string textureName)
    {
        System.Action<string> setter = wallType switch
        {
            WallType.Solid => s => wallTextures.DefaultSolid = s,
            WallType.Door => s => wallTextures.DefaultDoor = s,
            WallType.SecretDoor => s => wallTextures.DefaultSecretDoor = s,
            _ => throw new System.Exception($"Cannot set texture for wall type None"),
        };
        setter.Invoke(textureName);
    }

    /// <summary>
    /// Sets the specified wall to use the specified type
    /// </summary>
    /// <param name="data"></param>
    /// <param name="wallRef"></param>
    /// <param name="type"></param>
    public static void SetWallType(this DungeonCrawlerData data, WallReference wallRef, WallType type)
    {
        if (wallRef.Dungeon.Walls[wallRef.Position, wallRef.Facing] == type) { return; }
        wallRef.Dungeon.WallTextures.Textures.Remove((wallRef.Position, wallRef.Facing));
        wallRef.Dungeon.WallTextures.Textures.Remove((wallRef.Position.Step(wallRef.Facing), wallRef.Facing.Opposite()));
        wallRef.Dungeon.Walls.SetWall(wallRef.Position, wallRef.Facing, type);
        if (wallRef.Dungeon == data.CurrentDungeon)
        {
            data.CurrentDungeonData.AddChange(wallRef);
        }
    }

    public static void SyncTextureData(this DungeonCrawlerData data, TextureReference texture, Texture2D newTexture)
    {
        data.Manifest.Textures[texture.TextureName] = new Texture(texture.TextureName, ImageConversion.EncodeToPNG(newTexture));
        texture.SetTexture(newTexture);
    }

    public static bool HasTexture(this DungeonCrawlerData data, string textureName) => data.Manifest.Textures.ContainsKey(textureName);
    public static bool HasReference(this DungeonCrawlerData data, TileReference tileRef) => data.MaterialCache.HasReference(tileRef);
    public static TileWallTextures GetTileWallTextures(this DungeonCrawlerData data, TileReference tileRef) => data.MaterialCache.GetTileWallTextures(tileRef);
    public static TextureReference GetTexture(this DungeonCrawlerData data, TileReference tileRef) => data.MaterialCache.GetTexture(tileRef);
    public static TextureReference GetTexture(this DungeonCrawlerData data, WallReference wallRef) => data.MaterialCache.GetTexture(wallRef);
    public static TextureReference GetTexture(this DungeonCrawlerData data, string textureName) => data.MaterialCache.GetTexture(textureName);

    public static void DeleteTexture(this DungeonCrawlerData data, TextureReference textureRef)
    {
        data.Manifest.Textures.Remove(textureRef.TextureName);
        data.MaterialCache.DeleteTexture(textureRef);
    }

    public static void CreateTexture(this DungeonCrawlerData data, string name, Texture2D texture2D)
    {
        if (data.Manifest.Textures.ContainsKey(name)) { throw new InvalidOperationException($"A texture with the name {name} is already in the manifest."); }
        Texture dungeonTexture = new(name, ImageConversion.EncodeToPNG(texture2D));
        data.Manifest.AddTexture(dungeonTexture);
        data.MaterialCache.AddTextureData(dungeonTexture);
    }

    public static bool TryLoadManifest(this DungeonCrawlerData data, string json, out string message)
    {
        try
        {
            DungeonCrawlerManifest manifest = JsonExtensions.LoadModel<DungeonCrawlerManifest>(json);
            data.MaterialCache.InitializeMaterialCache(manifest);
            data.Manifest = manifest;
            data.CurrentDungeonData.Dungeon = manifest.Dungeons.First().Value.Copy();
            message = "Manifest loaded successfully";
            data.NotifyObservers(new ManifestInitialized(manifest));
        }
        // TODO: Figure out best exception type
        catch (Exception e)
        {
            message = $"Could not load manifest:\n\n{e}";
            Debug.LogError(message);
            return false;
        }
        return true;
    }

    public static bool TryAddDungeon(this DungeonCrawlerData data, Dungeon newDungeon, out string message)
    {
        if (data.Manifest.Dungeons.ContainsKey(newDungeon.Name))
        {
            message = $"A dungeon named {newDungeon.Name} already exists.";
            return false;
        }
        data.CurrentDungeonData.Dungeon = newDungeon.Copy();
        data.MaterialCache.AddDungeonReferences(newDungeon);
        data.Manifest.AddDungeon(newDungeon.Name, newDungeon);
        message = "Dungeon added";
        return true;
    }

    public static void DeleteDungeon(this DungeonCrawlerData data, Dungeon dungeon)
    {
        if (data.Manifest.Dungeons.Remove(dungeon.Name))
        {
            data.MaterialCache.RemoveDungeonReferences(dungeon);
            data.NotifyObservers(new DungeonRemovedEvent(dungeon));
        }
    }
}