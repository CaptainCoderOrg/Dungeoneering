
using System;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.IO;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity.Data
{
    [CreateAssetMenu(menuName = "DC/DungeonCrawlerData")]
    public class DungeonCrawlerData : ObservableSO
    {
        [field: SerializeField]
        public TextAsset DefaultManifestJson { get; private set; }
        internal DungeonManifestData ManifestData { get; private set; }
        public DungeonCrawlerManifest Manifest => ManifestData.Manifest;
        internal LoadedDungeon CurrentDungeonData { get; private set; }
        public Dungeon CurrentDungeon => CurrentDungeonData.Dungeon;
        internal MaterialCache MaterialCache { get; private set; }
        public bool PreventNotify
        {
            get => CurrentDungeonData.PreventNotify;
            set => CurrentDungeonData.PreventNotify = value;
        }
        public bool HasChanged => CurrentDungeonData.HasChanged;

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

        private void HandleDungeonRemoved(Dungeon dungeon) => MaterialCache.RemoveDungeonReferences(dungeon);

        private void HandleDungeonAdded(Dungeon dungeon)
        {
            CurrentDungeonData.Dungeon = dungeon;
            MaterialCache.AddDungeonReferences(dungeon);
        }

        private void HandleManifestLoaded(DungeonCrawlerManifest manifest)
        {
            CurrentDungeonData.Dungeon = manifest.Dungeons.First().Value.Copy();
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
            CurrentDungeonData = new();
            ManifestData = new(MaterialCache);
            if (!ManifestData.TryLoadManifest(DefaultManifestJson.text, out _))
            {
                Debug.Log("Manifest could not be loaded");
            }
            Init();
        }
        public void AddObserver(Action<DungeonManifestChanged> handler) => ManifestData.AddObserver(handler);
        public void RemoveObserver(Action<DungeonManifestChanged> handler) => ManifestData.RemoveObserver(handler);
        public void AddObserver(Action<CacheUpdateData> handler) => MaterialCache.AddObserver(handler);
        public void RemoveObserver(Action<CacheUpdateData> handler) => MaterialCache.RemoveObserver(handler);
        public void AddObserver(Action<DungeonChangeEvent> handleDungeonChanged) => CurrentDungeonData.AddObserver(handleDungeonChanged);
        public void RemoveObserver(Action<DungeonChangeEvent> handleDungeonChanged) => CurrentDungeonData.RemoveObserver(handleDungeonChanged);
        public void LoadDungeon(string dungeonJson) => LoadDungeon(JsonExtensions.LoadModel<Dungeon>(dungeonJson));
        public void LoadDungeon(Dungeon dungeon)
        {
            if (CurrentDungeon != null)
            {
                MaterialCache.RemoveDungeonReferences(dungeon);
            }
            CurrentDungeonData.Dungeon = dungeon;
            MaterialCache.AddDungeonReferences(dungeon);
        }
    }
}