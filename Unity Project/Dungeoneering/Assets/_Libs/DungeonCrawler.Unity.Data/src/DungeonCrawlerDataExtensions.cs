

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
    public static void UseDefaultTexture(this DungeonCrawlerData data, TileReference tile)
    {
        tile.Dungeon.TileTextures.Textures.Remove(tile.Position);
        data.MaterialCache.RemoveTexture(tile);
        if (data.CurrentDungeon.Dungeon == tile.Dungeon)
        {
            data.CurrentDungeon.AddChange(tile);
        }
    }

}