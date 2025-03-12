using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap;

namespace CaptainCoder.Dungeoneering.Unity.Data;
internal class LoadedManifest
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

    internal void DeleteDungeon(Dungeon dungeon)
    {
        if (Manifest.Dungeons.Remove(dungeon.Name))
        {
            _onManifestChanged?.Invoke(new DungeonRemovedEvent(dungeon));
        }
    }
}

public abstract record class DungeonManifestChanged;
public sealed record class ManifestChanged(DungeonCrawlerManifest Manifest) : DungeonManifestChanged;
public sealed record class DungeonRemovedEvent(Dungeon Dungeon) : DungeonManifestChanged;