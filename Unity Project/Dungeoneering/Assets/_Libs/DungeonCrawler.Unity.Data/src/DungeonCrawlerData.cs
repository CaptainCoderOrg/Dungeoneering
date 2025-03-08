
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
        public DungeonData CurrentDungeon { get; private set; } = new();
        public MaterialCache MaterialCache { get; private set; } = new();

        private void Init()
        {
            ManifestData.RemoveListener(HandleManifestChanged);
            ManifestData.AddListener(HandleManifestChanged);
        }

        private void HandleManifestChanged(DungeonCrawlerManifest manifest)
        {
            CurrentDungeon.Dungeon = manifest.Dungeons.First().Value.Copy();
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
            CurrentDungeon = new();
            MaterialCache = new();
        }

        /// <summary>
        /// Forces all DungeonCrawlerData to be reinitialized. This is an expensive operation and is designed for testing.
        /// Note: This clears all listeners on data objects.
        /// </summary>
        public void ForceInitialize()
        {
            MaterialCache.Clear();
            ManifestData.Initialize(MaterialCache);
            CurrentDungeon.Initialize(MaterialCache);
            MaterialCache.DungeonData = CurrentDungeon;
            Init();
        }
    }
}