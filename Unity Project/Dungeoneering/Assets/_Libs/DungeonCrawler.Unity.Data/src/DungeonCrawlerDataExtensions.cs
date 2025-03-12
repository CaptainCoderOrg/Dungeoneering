

using CaptainCoder.Dungeoneering.DungeonMap;

namespace CaptainCoder.Dungeoneering.Unity.Data;

public static class DungeonCrawlerDataExtensions
{
    /// <summary>
    /// Save the current DungeonData with the ManifestData
    /// </summary>
    /// <param name="data"></param>
    public static void SyncWithManifest(this DungeonCrawlerData data)
    {
        data.ManifestData.UpdateDungeon(data.CurrentDungeon.Dungeon);
        data.CurrentDungeon.HasChanged = false;
    }

    /// <summary>
    /// Sets the specified tile to use the new texture
    /// </summary>
    public static void SetTexture(this DungeonCrawlerData data, TileReference tile, TextureReference newTexture)
    {
        data.MaterialCache.SetTexture(tile, newTexture);
        if (data.CurrentDungeon.Dungeon == tile.Dungeon)
        {
            data.CurrentDungeon.AddChange(tile);
        }
    }

    /// <summary>
    /// Sets the specified wall to use the new texture
    /// </summary>
    public static void SetTexture(this DungeonCrawlerData data, WallReference wall, TextureReference newTexture)
    {
        data.MaterialCache.SetTexture(wall, newTexture);
        if (data.CurrentDungeon.Dungeon == wall.Dungeon)
        {
            data.CurrentDungeon.AddChange(wall);
        }
    }

    /// <summary>
    /// Sets the specified tile to use the default texture
    /// </summary>
    public static void UseDefaultTexture(this DungeonCrawlerData data, TileReference tileRef)
    {
        tileRef.Dungeon.TileTextures.Textures.Remove(tileRef.Position);
        data.MaterialCache.RemoveTexture(tileRef);
        if (data.CurrentDungeon.Dungeon == tileRef.Dungeon)
        {
            data.CurrentDungeon.AddChange(tileRef);
        }
    }

    /// <summary>
    /// Sets the specified wall to use the default texture
    /// </summary>
    public static void UseDefaultTexture(this DungeonCrawlerData data, WallReference wallRef)
    {
        wallRef.Dungeon.WallTextures.Textures.Remove((wallRef.Position, wallRef.Facing));
        data.MaterialCache.RemoveTexture(wallRef);
        if (data.CurrentDungeon.Dungeon == wallRef.Dungeon)
        {
            data.CurrentDungeon.AddChange(wallRef);
        }
    }

    /// <summary>
    /// Sets the default tile texture for the specified dungeon to the new texture
    /// </summary>
    public static void SetDefaultTileTexture(this DungeonCrawlerData data, Dungeon targetDungeon, TextureReference newTexture)
    {
        if (!data.ManifestData.Manifest.Dungeons.TryGetValue(targetDungeon.Name, out Dungeon dungeon))
        {
            throw new System.InvalidOperationException($"The specified dungeon {targetDungeon.Name} does not exist in the manifest.");
        }
        TextureReference previousTexture = data.MaterialCache.GetTexture(dungeon.TileTextures.Default);
        dungeon.TileTextures.Default = newTexture.TextureName;
        previousTexture.DefaultTileDungeons.Remove(dungeon);
        newTexture.DefaultTileDungeons.Add(dungeon);
        if (targetDungeon.Name == data.CurrentDungeon.Dungeon.Name)
        {
            previousTexture.DefaultTileDungeons.Remove(data.CurrentDungeon.Dungeon);
            newTexture.DefaultTileDungeons.Add(data.CurrentDungeon.Dungeon);
            data.CurrentDungeon.Dungeon.TileTextures.Default = newTexture.TextureName;
            data.CurrentDungeon.ForceNotify();
        }
    }

    /// <summary>
    /// Sets the default texture for the specified dungeon and wall type.
    /// </summary>
    public static void SetDefaultWallTexture(this DungeonCrawlerData data, Dungeon targetDungeon, TextureReference newTexture, WallType wallType)
    {
        if (!data.ManifestData.Manifest.Dungeons.TryGetValue(targetDungeon.Name, out Dungeon dungeon))
        {
            throw new System.InvalidOperationException($"The specified dungeon {targetDungeon.Name} does not exist in the manifest.");
        }
        TextureReference previousTexture = data.MaterialCache.GetTexture(dungeon.WallTextures.DefaultSolid);
        dungeon.WallTextures.SetDefaultTexture(wallType, newTexture.TextureName);
        previousTexture.RemoveDefaultWall(wallType, dungeon);
        newTexture.AddDefaultWall(wallType, dungeon);
        if (targetDungeon.Name == data.CurrentDungeon.Dungeon.Name)
        {
            previousTexture.RemoveDefaultWall(wallType, data.CurrentDungeon.Dungeon);
            newTexture.AddDefaultWall(wallType, data.CurrentDungeon.Dungeon);
            data.CurrentDungeon.Dungeon.WallTextures.SetDefaultTexture(wallType, newTexture.TextureName);
            data.CurrentDungeon.ForceNotify();
        }
    }

    private static void SetDefaultTexture(this WallTextureMap wallTextures, WallType wallType, string textureName)
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
        if (wallRef.Dungeon == data.CurrentDungeon.Dungeon)
        {
            data.CurrentDungeon.AddChange(wallRef);
        }
    }

    public static TextureReference GetTexture(this DungeonCrawlerData data, TileReference tileRef) => data.MaterialCache.GetTexture(tileRef);
    public static TextureReference GetTexture(this DungeonCrawlerData data, WallReference wallRef) => data.MaterialCache.GetTexture(wallRef);

}