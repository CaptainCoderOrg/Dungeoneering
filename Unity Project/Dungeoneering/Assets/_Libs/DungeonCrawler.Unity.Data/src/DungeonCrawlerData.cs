
using System;
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
            ManifestData.RemoveObserver(HandleManifestChanged);
            ManifestData.AddObserver(HandleManifestChanged);
        }

        private void HandleManifestChanged(DungeonManifestChanged changeEvent)
        {
            switch (changeEvent)
            {
                case ManifestLoadedEvent(DungeonCrawlerManifest manifest):
                    HandleManifestLoaded(manifest);
                    break;
                case DungeonAddedEvent(Dungeon added):
                    HandleDungeonAdded(added);
                    break;
                case DungeonRemovedEvent(Dungeon removed):
                    HandleDungeonRemoved(removed);
                    break;
            }
        }

        private void HandleDungeonRemoved(Dungeon dungeon)
        {
            MaterialCache.RemoveDungeonReferences(dungeon);
        }

        private void HandleDungeonAdded(Dungeon dungeon)
        {
            CurrentDungeon.Dungeon = dungeon;
            MaterialCache.AddDungeonReferences(dungeon);
        }

        private void HandleManifestLoaded(DungeonCrawlerManifest manifest)
        {
            CurrentDungeon.Dungeon = manifest.Dungeons.First().Value.Copy();
        }

        public override void OnBeforeEnterPlayMode()
        {
            base.OnBeforeEnterPlayMode();
            ForceInitialize();
        }

#if UNITY_EDITOR
        public override void OnAfterEnterPlayMode()
        {
            base.OnAfterEnterPlayMode();
            // Required to correctly load SelectableMaterial in PlayMode
            if (!ManifestData.TryLoadManifest(DefaultManifestJson.text, out _))
            {
                Debug.Log("Manifest could not be loaded");
            }
        }
#endif

        protected override void OnExitPlayMode()
        {
            base.OnExitPlayMode();
            ManifestData.RemoveObserver(HandleManifestChanged);
        }

        /// <summary>
        /// Forces all DungeonCrawlerData to be reinitialized. This is an expensive operation and is designed for testing.
        /// Note: This clears all listeners on data objects.
        /// </summary>
        public void ForceInitialize()
        {
            MaterialCache = new();
            CurrentDungeon = new();
            ManifestData = new(MaterialCache);
            CurrentDungeon.OnChange += HandleDungeonChanged;
            if (!ManifestData.TryLoadManifest(DefaultManifestJson.text, out _))
            {
                Debug.Log("Manifest could not be loaded");
            }
            Init();
        }

        private void HandleDungeonChanged(DungeonChangedData changes)
        {
            MaterialCache.RemoveDungeonReferences(changes.Previous);
            MaterialCache.AddDungeonReferences(changes.New);
        }
    }
}