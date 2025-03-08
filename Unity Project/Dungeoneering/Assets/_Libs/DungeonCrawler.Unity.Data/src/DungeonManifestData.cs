using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.IO;

using UnityEngine;
using UnityEngine.Events;

namespace CaptainCoder.Dungeoneering.Unity.Data
{
    public class DungeonManifestData
    {
        private readonly UnityEvent<DungeonCrawlerManifest> _onManifestLoaded = new();
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
            _onManifestLoaded.Invoke(_manifest);
            return true;
        }

        public void AddListener(UnityAction<DungeonCrawlerManifest> onChange)
        {
            _onManifestLoaded.AddListener(onChange);
            if (_manifest != null)
            {
                onChange.Invoke(_manifest);
            }
        }

        public void RemoveListener(UnityAction<DungeonCrawlerManifest> onChange) => _onManifestLoaded.RemoveListener(onChange);


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

        public void RemoveDungeon(Dungeon dungeon)
        {
            if (Manifest.Dungeons.Remove(dungeon.Name))
            {
                _onManifestLoaded.Invoke(_manifest);
            }
        }
    }
}