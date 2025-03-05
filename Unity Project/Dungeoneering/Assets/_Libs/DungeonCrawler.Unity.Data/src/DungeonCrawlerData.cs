
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity.Data
{
    [CreateAssetMenu(menuName = "DC/DungeonCrawlerData")]
    public class DungeonCrawlerData : ObservableSO
    {
        [field: SerializeField]
        public DungeonManifestData ManifestData { get; private set; }
        [field: SerializeField]
        public DungeonData DungeonData { get; private set; }
        [field: SerializeField]
        public MaterialCacheData CacheData { get; private set; }
        public MaterialCache MaterialCache => CacheData.Cache;

        private void Init()
        {
            ManifestData.RemoveListener(HandleManifestChanged);
            ManifestData.AddListener(HandleManifestChanged);
        }

        private void HandleManifestChanged(DungeonCrawlerManifest manifest)
        {
            DungeonData.Dungeon = manifest.Dungeons.First().Value.Copy();
        }

        protected override void AfterEnabled()
        {
            base.AfterEnabled();
            Init();
        }

        protected override void OnEnterPlayMode()
        {
            base.OnEnterPlayMode();
            Init();
        }

        protected override void OnExitPlayMode()
        {
            base.OnExitPlayMode();
            ManifestData.RemoveListener(HandleManifestChanged);
        }
    }
}