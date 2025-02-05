
using System.Collections.Generic;
using System.Linq;
using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap.IO;
using UnityEngine;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{

    [CreateAssetMenu(fileName = "DungeonManifestData", menuName = "DC/Manifest")]
    public class DungeonManifestData : ObservableSO
    {
        private DungeonCrawlerManifest _manifest;
        public DungeonCrawlerManifest Manifest => _manifest ??= LoadManifest();
        private Dictionary<string, Material> _materialCache;
        public Dictionary<string, Material> MaterialCache => _materialCache ??= InitializeMaterialCache(Manifest);
        
        [field: SerializeField] 
        public TextAsset ManifestJson { get; private set; }
        public DungeonCrawlerManifest LoadManifest() => JsonExtensions.LoadModel<DungeonCrawlerManifest>(ManifestJson.text);

        // public DungeonCrawlerManifest LoadFromFile(string path) => JsonExtensions.LoadModel<DungeonCrawlerManifest>(File.ReadAllText(path));

        private Dictionary<string, Material> InitializeMaterialCache(DungeonCrawlerManifest manifest)
        {
            Debug.Log("Initializing Cache");
            _materialCache = manifest.Textures.Values.ToDictionary(t => t.Name, t => t.ToMaterial());
            return _materialCache;
        }

        protected override void AfterEnabled()
        {
            base.AfterEnabled();
            _materialCache = null;
            _manifest = null;
        }

        protected override void OnEnterPlayMode()
        {
            base.OnEnterPlayMode();
            _materialCache = null;
            _manifest = null;
        }

        protected override void OnExitPlayMode()
        {
            base.OnExitPlayMode();
            _materialCache = null;
            _manifest = null;
        }
    }
}