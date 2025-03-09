using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.IO;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity.Data
{
    public class DungeonManifestData
    {
        public event System.Action<DungeonManifestChanged> OnManifestChanged;
        private DungeonCrawlerManifest _manifest;
        public DungeonCrawlerManifest Manifest => _manifest;
        private MaterialCache _materialCache;

        public bool TryLoadManifest(string json, out DungeonCrawlerManifest loaded)
        {
            try
            {
                loaded = JsonExtensions.LoadModel<DungeonCrawlerManifest>(json);
            }
            // TODO: Figure out best exception type
            catch (System.Exception e)
            {
                Debug.LogError($"Could not load manifest: {e}");
                loaded = null;
                return false;
            }
            _manifest = loaded;
            // TODO: Consider using events to trigger the material cache to be initialized.
            _materialCache.InitializeMaterialCache(_manifest);
            OnManifestChanged?.Invoke(new ManifestLoadedEvent(_manifest));
            return true;
        }

        public void AddObserver(System.Action<DungeonManifestChanged> onChange)
        {
            OnManifestChanged += onChange;
            if (_manifest != null)
            {
                onChange.Invoke(new ManifestLoadedEvent(_manifest));
            }
        }

        public void RemoveObserver(System.Action<DungeonManifestChanged> onChange) => OnManifestChanged -= onChange;

        public DungeonManifestData(MaterialCache materialCache)
        {
            _materialCache = materialCache;
        }

        public void UpdateDungeon(Dungeon dungeon)
        {
            Dungeon copy = dungeon.Copy();
            _materialCache.RemoveDungeonReferences(_manifest.Dungeons[copy.Name]);
            _materialCache.AddDungeonReferences(copy);
            _manifest.Dungeons[copy.Name] = copy;
        }

        public bool TryAddDungeon(Dungeon dungeon, out string message)
        {
            if (_manifest.Dungeons.ContainsKey(dungeon.Name))
            {
                message = $"A dungeon named {dungeon.Name} already exists.";
                return false;
            }
            _manifest.AddDungeon(dungeon.Name, dungeon);
            message = "Dungeon added";
            OnManifestChanged?.Invoke(new DungeonAddedEvent(dungeon));
            return true;
        }

        public void RemoveDungeon(Dungeon dungeon)
        {
            if (Manifest.Dungeons.Remove(dungeon.Name))
            {
                OnManifestChanged?.Invoke(new DungeonRemovedEvent(dungeon));
            }
        }

        public void UpdateTexture(TextureReference texture, Texture2D newTexture)
        {
            _manifest.Textures[texture.TextureName] = new Texture(texture.TextureName, ImageConversion.EncodeToPNG(newTexture));
            texture.SetTexture(newTexture);
            OnManifestChanged.Invoke(new TextureUpdatedEvent(texture));
        }
    }

    public abstract record class DungeonManifestChanged;
    public record class ManifestLoadedEvent(DungeonCrawlerManifest Manifest) : DungeonManifestChanged;
    public record class DungeonRemovedEvent(Dungeon Dungeon) : DungeonManifestChanged;
    public record class DungeonAddedEvent(Dungeon Dungeon) : DungeonManifestChanged;
    public record class TextureUpdatedEvent(TextureReference TextureRef) : DungeonManifestChanged;
}