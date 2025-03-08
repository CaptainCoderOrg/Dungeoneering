
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
        public MaterialCache MaterialCache { get; private set; } = new();

        private void Init()
        {
            ManifestData.RemoveListener(HandleManifestChanged);
            ManifestData.AddListener(HandleManifestChanged);
        }

        private void HandleManifestChanged(DungeonCrawlerManifest manifest)
        {
            DungeonData.Dungeon = manifest.Dungeons.First().Value.Copy();
        }

        public override void OnEnterPlayMode()
        {
            base.OnEnterPlayMode();
            ForceInitialize();
        }

        protected override void OnExitPlayMode()
        {
            base.OnExitPlayMode();
            ManifestData.RemoveListener(HandleManifestChanged);
        }

        /// <summary>
        /// Forces all DungeonCrawlerData to be reinitialized. This is an expensive operation and is designed for testing.
        /// Note: This clears all listeners on data objects.
        /// </summary>
        public void ForceInitialize()
        {
            MaterialCache.Clear();
            ManifestData.Initialize(MaterialCache);
            DungeonData.Initialize(MaterialCache);
            Init();
        }
    }
}