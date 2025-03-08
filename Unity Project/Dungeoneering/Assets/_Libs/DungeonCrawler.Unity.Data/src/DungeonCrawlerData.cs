
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
        public TextAsset DefaultManifestJson { get; private set; }
        public DungeonManifestData ManifestData { get; private set; }
        public DungeonData CurrentDungeon { get; private set; }
        public MaterialCache MaterialCache { get; private set; }

        private void Init()
        {
            ManifestData.RemoveListener(HandleManifestChanged);
            ManifestData.AddListener(HandleManifestChanged);
        }

        private void HandleManifestChanged(DungeonCrawlerManifest manifest)
        {
            CurrentDungeon.Dungeon = manifest.Dungeons.First().Value.Copy();
        }

        public override void OnBeforeEnterPlayMode()
        {
            base.OnBeforeEnterPlayMode();
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
            MaterialCache = new();
            CurrentDungeon = new(MaterialCache);
            ManifestData = new(MaterialCache);
            MaterialCache.DungeonData = CurrentDungeon;
            if (!ManifestData.TryLoadManifest(DefaultManifestJson.text, out _))
            {
                Debug.Log("Manifest could not be loaded");
            }
            Init();
        }
    }
}