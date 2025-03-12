using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity.Data;
internal class DungeonManifestData
{
    private System.Action<DungeonManifestChanged> _onManifestChanged;
    private DungeonCrawlerManifest _manifest;
    internal DungeonCrawlerManifest Manifest
    {
        get => _manifest;
        set
        {
            _manifest = value;
            _onManifestChanged?.Invoke(new ManifestChanged(_manifest));
        }
    }
    internal void AddObserver(System.Action<DungeonManifestChanged> onChange)
    {
        _onManifestChanged += onChange;
        if (_manifest != null)
        {
            onChange.Invoke(new ManifestChanged(_manifest));
        }
    }

    internal void RemoveObserver(System.Action<DungeonManifestChanged> onChange) => _onManifestChanged -= onChange;
    internal void AddDungeon(Dungeon dungeon)
    {
        _manifest.AddDungeon(dungeon.Name, dungeon);
        _onManifestChanged?.Invoke(new DungeonAddedEvent(dungeon));
    }

    internal void DeleteDungeon(Dungeon dungeon)
    {
        if (Manifest.Dungeons.Remove(dungeon.Name))
        {
            _onManifestChanged?.Invoke(new DungeonRemovedEvent(dungeon));
        }
    }

    internal void SyncTextureData(TextureReference texture, Texture2D newTexture)
    {
        _manifest.Textures[texture.TextureName] = new Texture(texture.TextureName, ImageConversion.EncodeToPNG(newTexture));
        texture.SetTexture(newTexture);
        _onManifestChanged?.Invoke(new TextureUpdatedEvent(texture));
    }
}

public abstract record class DungeonManifestChanged;
public record class ManifestChanged(DungeonCrawlerManifest Manifest) : DungeonManifestChanged;
public record class DungeonRemovedEvent(Dungeon Dungeon) : DungeonManifestChanged;
public record class DungeonAddedEvent(Dungeon Dungeon) : DungeonManifestChanged;
public record class TextureUpdatedEvent(TextureReference TextureRef) : DungeonManifestChanged;