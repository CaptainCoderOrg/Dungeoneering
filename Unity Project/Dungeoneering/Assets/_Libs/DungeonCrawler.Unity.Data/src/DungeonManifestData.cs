using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.IO;

using UnityEngine;
using UnityEngine.Events;

namespace CaptainCoder.Dungeoneering.Unity.Data
{
    [CreateAssetMenu(fileName = "DungeonManifestData", menuName = "DC/Manifest")]
    public class DungeonManifestData : ObservableSO
    {
        private readonly UnityEvent<DungeonCrawlerManifest> _onManifestLoaded = new();
        private DungeonCrawlerManifest _manifest;
        public DungeonCrawlerManifest Manifest => _manifest;
        [SerializeField]
        private MaterialCacheData _materialCache;
        public MaterialCache MaterialCache => _materialCache.Cache;
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
            // TODO: Consider using events to trigger the material cache to be initialized.
            _materialCache.Cache.InitializeMaterialCache(_manifest);
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

        protected override void AfterEnabled()
        {
            base.AfterEnabled();
            ClearListeners();
            InitialLoad();
        }

        private void ClearListeners()
        {
            MaterialCache.Clear();
            _manifest = null;
            _onManifestLoaded.RemoveAllListeners();
        }
        private void InitialLoad()
        {
            Debug.Log("Loading Manifest");
            if (!TryLoadManifest(ManifestJson.text, out _manifest))
            {
                Debug.Log("Manifest could not be loaded");
            }
        }

        protected override void OnExitEditMode()
        {
            base.OnExitEditMode();
            ClearListeners();
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

        public void UpdateDungeon(Dungeon dungeon) => _manifest.Dungeons[dungeon.Name] = dungeon;

        public void RemoveDungeon(Dungeon dungeon)
        {
            if (Manifest.Dungeons.Remove(dungeon.Name))
            {
                _onManifestLoaded.Invoke(_manifest);
            }
        }
    }
}