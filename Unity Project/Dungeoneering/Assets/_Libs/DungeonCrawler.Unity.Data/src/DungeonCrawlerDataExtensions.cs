

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

}