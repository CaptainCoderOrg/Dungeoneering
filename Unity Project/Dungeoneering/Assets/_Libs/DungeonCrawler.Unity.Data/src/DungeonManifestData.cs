using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap.IO;

using UnityEngine;
using UnityEngine.Events;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{

    [CreateAssetMenu(fileName = "DungeonManifestData", menuName = "DC/Manifest")]
    public class DungeonManifestData : ObservableSO
    {
        private readonly UnityEvent<DungeonCrawlerManifest> _onManifestLoaded = new();
        private readonly UnityEvent<CacheUpdateData> _onCacheChanged = new();
        public UnityEvent<TilesChangedData> OnTilesChanged { get; private set; } = new();
        private TilesChangedData _changes = new();
        private DungeonCrawlerManifest _manifest;
        public DungeonCrawlerManifest Manifest => _manifest;
        private Dictionary<string, Material> _materialCache;
        public Dictionary<string, Material> MaterialCache => _materialCache ??= InitializeMaterialCache(Manifest);
        [field: SerializeField]
        public TextAsset ManifestJson { get; private set; }

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
            _materialCache = InitializeMaterialCache(_manifest);
            _onManifestLoaded.Invoke(_manifest);
            return true;
        }

        public void AddListener(UnityAction<CacheUpdateData> onChange)
        {
            _onCacheChanged.AddListener(onChange);
            if (_materialCache != null)
            {
                onChange.Invoke(new CacheUpdateData(_materialCache, true));
            }
        }

        public void RemoveListener(UnityAction<CacheUpdateData> onChange) => _onCacheChanged.RemoveListener(onChange);

        public void AddListener(UnityAction<DungeonCrawlerManifest> onChange)
        {
            _onManifestLoaded.AddListener(onChange);
            if (_manifest != null)
            {
                onChange.Invoke(_manifest);
            }
        }

        public void RemoveListener(UnityAction<DungeonCrawlerManifest> onChange) => _onManifestLoaded.RemoveListener(onChange);

        public void Notify()
        {
            if (_changes.Tiles.Count == 0) { return; }
            OnTilesChanged.Invoke(_changes);
            _changes = new();
        }

        private Dictionary<string, Material> InitializeMaterialCache(DungeonCrawlerManifest manifest)
        {
            Debug.Log("Initializing Cache");
            _materialCache = manifest.Textures.Values.ToDictionary(t => t.Name, t => t.ToMaterial());
            _onCacheChanged.Invoke(new CacheUpdateData(_materialCache, true));
            return _materialCache;
        }

        public void AddTexture(string name, Texture2D texture)
        {
            if (_manifest.Textures.ContainsKey(name)) { return; }
            Texture dungeonTexture = new(name, ImageConversion.EncodeToPNG(texture));
            _manifest.AddTexture(dungeonTexture);
            _materialCache.Add(name, dungeonTexture.ToMaterial());
            _onCacheChanged.Invoke(new CacheUpdateData(_materialCache, false, name));
        }

        protected override void AfterEnabled()
        {
            base.AfterEnabled();
            InitialLoad();
        }

        private void ClearListeners()
        {
            _materialCache = null;
            _manifest = null;
            _onCacheChanged.RemoveAllListeners();
            _onManifestLoaded.RemoveAllListeners();
            OnTilesChanged.RemoveAllListeners();
        }
        private void InitialLoad()
        {
            ClearListeners();
            Debug.Log("Loading Manifest");
            if (!TryLoadManifest(ManifestJson.text, out _manifest))
            {
                Debug.Log("Manifest could not be loaded");
            }
        }

        protected override void OnEnterPlayMode()
        {
            base.OnEnterPlayMode();
            InitialLoad();
        }

        protected override void OnExitPlayMode()
        {
            base.OnExitPlayMode();
            ClearListeners();
        }

        public void SetFloorTexture(Dungeon dungeon, Position position, string textureName)
        {
            dungeon.TileTextures.Textures[position] = textureName;
            _changes.AddChange(dungeon, position);
        }

        public void SetWallTexture(Dungeon dungeon, Position position, Facing facing, string textureName)
        {
            dungeon.SetTexture(position, facing, textureName);
            _changes.AddChange(dungeon, position);
        }

        public void SetWallType(Dungeon dungeon, Position position, Facing facing, WallType type)
        {
            if (dungeon.Walls[position, facing] == type) { return; }
            dungeon.WallTextures.Textures.Remove((position, facing));
            dungeon.WallTextures.Textures.Remove((position.Step(facing), facing.Opposite()));
            dungeon.Walls.SetWall(position, facing, type);
            _changes.AddChange(dungeon, position);
        }

        public string GetFloorTexture(Dungeon d, Position p) => d.TileTextures.GetTileTextureName(p);
        public string GetWallTexture(Dungeon d, Position p, Facing f) => d.GetWallTexture(p, f);

        public void RemoveDungeon(Dungeon dungeon)
        {
            if (Manifest.Dungeons.Remove(dungeon.Name))
            {
                _onManifestLoaded.Invoke(_manifest);
            }
        }
    }

    public class TilesChangedData
    {
        public HashSet<(Dungeon, Position)> Tiles { get; private set; } = new();
        public bool AddChange(Dungeon dungeon, Position position) => Tiles.Add((dungeon, position));
    }

    public class CacheUpdateData
    {
        public Dictionary<string, Material> Cache { get; private set; }
        public IEnumerable<string> Added { get; private set; }
        public readonly bool IsNewCache;

        public CacheUpdateData(Dictionary<string, Material> cache, bool isNew, params string[] added)
        {
            IsNewCache = isNew;
            Cache = cache;
            Added = added;
        }
    }
}