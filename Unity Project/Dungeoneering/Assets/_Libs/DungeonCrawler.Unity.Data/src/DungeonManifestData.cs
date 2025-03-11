using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.IO;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity.Data
{
    public class DungeonManifestData
    {
        private System.Action<DungeonManifestChanged> _onManifestChanged;
        private DungeonCrawlerManifest _manifest;
        public DungeonCrawlerManifest Manifest => _manifest;
        private MaterialCache _materialCache;

        public bool TryLoadManifest(string json, out string message)
        {
            try
            {
                _manifest = JsonExtensions.LoadModel<DungeonCrawlerManifest>(json);
                message = "Manifest loaded successfully";
            }
            // TODO: Figure out best exception type
            catch (System.Exception e)
            {
                message = $"Could not load manifest:\n\n{e}";
                Debug.LogError(message);
                return false;
            }
            // TODO: Consider using events to trigger the material cache to be initialized.
            _materialCache.InitializeMaterialCache(_manifest);
            _onManifestChanged?.Invoke(new ManifestLoadedEvent(_manifest));
            return true;
        }

        public void AddObserver(System.Action<DungeonManifestChanged> onChange)
        {
            _onManifestChanged += onChange;
            if (_manifest != null)
            {
                onChange.Invoke(new ManifestLoadedEvent(_manifest));
            }
        }

        public void RemoveObserver(System.Action<DungeonManifestChanged> onChange) => _onManifestChanged -= onChange;

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
            _onManifestChanged?.Invoke(new DungeonAddedEvent(dungeon));
            return true;
        }

        public void RemoveDungeon(Dungeon dungeon)
        {
            if (Manifest.Dungeons.Remove(dungeon.Name))
            {
                _onManifestChanged?.Invoke(new DungeonRemovedEvent(dungeon));
            }
        }

        public void UpdateTexture(TextureReference texture, Texture2D newTexture)
        {
            _manifest.Textures[texture.TextureName] = new Texture(texture.TextureName, ImageConversion.EncodeToPNG(newTexture));
            texture.SetTexture(newTexture);
            _onManifestChanged?.Invoke(new TextureUpdatedEvent(texture));
        }
    }

    public abstract record class DungeonManifestChanged;
    public record class ManifestLoadedEvent(DungeonCrawlerManifest Manifest) : DungeonManifestChanged;
    public record class DungeonRemovedEvent(Dungeon Dungeon) : DungeonManifestChanged;
    public record class DungeonAddedEvent(Dungeon Dungeon) : DungeonManifestChanged;
    public record class TextureUpdatedEvent(TextureReference TextureRef) : DungeonManifestChanged;
}