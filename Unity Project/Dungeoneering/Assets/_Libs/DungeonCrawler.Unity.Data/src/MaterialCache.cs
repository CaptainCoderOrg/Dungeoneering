using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;

using UnityEngine;
using UnityEngine.Events;

namespace CaptainCoder.Dungeoneering.Unity
{
    public class MaterialCache
    {
        private Dictionary<string, SelectableMaterial> _materialCache = new();
        private readonly UnityEvent<CacheUpdateData> _onCacheChanged = new();
        private DungeonCrawlerManifest _manifest;

        public Dictionary<string, SelectableMaterial> InitializeMaterialCache(DungeonCrawlerManifest manifest)
        {
            Debug.Log("Initializing Cache");
            _manifest = manifest;
            _materialCache = manifest.Textures.Values.ToDictionary(t => t.Name, t => new SelectableMaterial(t.ToMaterial()));
            _onCacheChanged.Invoke(new CacheUpdateData(_materialCache, true));
            return _materialCache;
        }

        public void RemoveListener(UnityAction<CacheUpdateData> onChange) => _onCacheChanged.RemoveListener(onChange);

        public void AddListener(UnityAction<CacheUpdateData> onChange)
        {
            _onCacheChanged.AddListener(onChange);
            if (_materialCache != null)
            {
                onChange.Invoke(new CacheUpdateData(_materialCache, true));
            }
        }

        public void AddTexture(string name, Texture2D texture)
        {
            if (_manifest.Textures.ContainsKey(name)) { return; }
            Texture dungeonTexture = new(name, ImageConversion.EncodeToPNG(texture));
            _manifest.AddTexture(dungeonTexture);
            _materialCache.Add(name, new SelectableMaterial(dungeonTexture.ToMaterial()));
            _onCacheChanged.Invoke(new CacheUpdateData(_materialCache, false, name));
        }

        public void Clear()
        {
            _materialCache.Clear();
            _onCacheChanged.RemoveAllListeners();
            _manifest = null;
        }

        public SelectableMaterial GetTileMaterial(Dungeon d, Position p) => _materialCache.GetTileMaterial(d, p);
        public TileWallMaterials GetTileWallMaterials(Dungeon d, Position p) => _materialCache.GetTileWallMaterials(d, p);
        public SelectableMaterial GetMaterial(string textureName) => _materialCache[textureName];
    }
}