using System;

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
        internal LoadedManifest ManifestData { get; private set; }
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

        private void HandleDungeonRemoved(Dungeon dungeon) => MaterialCache.RemoveDungeonReferences(dungeon);

        private void HandleDungeonAdded(Dungeon dungeon)
        {
            CurrentDungeonData.Dungeon = dungeon;
            MaterialCache.AddDungeonReferences(dungeon);
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
            if (!this.TryLoadManifest(DefaultManifestJson.text, out _))
            {
                Debug.Log("Manifest could not be loaded");
            }
        }
#endif

        /// <summary>
        /// Forces all DungeonCrawlerData to be reinitialized. This is an expensive operation and is designed for testing.
        /// Note: This clears all listeners on data objects.
        /// </summary>
        public void ForceInitialize()
        {
            MaterialCache = new();
            CurrentDungeonData = new();
            ManifestData = new();
            if (!this.TryLoadManifest(DefaultManifestJson.text, out _))
            {
                Debug.Log("Manifest could not be loaded");
            }
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