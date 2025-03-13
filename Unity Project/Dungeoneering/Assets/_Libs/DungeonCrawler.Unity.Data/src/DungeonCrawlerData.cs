using System;
using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity.Data
{
    [CreateAssetMenu(menuName = "DC/DungeonCrawlerData")]
    public class DungeonCrawlerData : ObservableSO
    {
        private Action<DungeonManifestChanged> _onManifestChanged;
        private Action<DungeonChangeEvent> _onDungeonChanged;

        [field: SerializeField]
        public TextAsset DefaultManifestJson { get; private set; }
        public DungeonCrawlerManifest Manifest { get; internal set; }
        public Dungeon CurrentDungeon { get; internal set; }
        internal MaterialCache MaterialCache { get; private set; }
        private bool _preventNotify = false;
        public bool PreventNotify
        {
            get => _preventNotify;
            set
            {
                _preventNotify = value;
                NotifyTileChanges();
            }
        }
        private bool _hasChanged = false;
        public bool HasChanged
        {
            get => _hasChanged;
            internal set
            {
                if (value == _hasChanged) { return; }
                _hasChanged = value;
                _onDungeonChanged?.Invoke(new SyncedStateChange(CurrentDungeon, !_hasChanged));
            }
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
            if (!this.TryLoadManifest(DefaultManifestJson.text, out _))
            {
                Debug.Log("Manifest could not be loaded");
            }
        }

        public void AddObserver(Action<DungeonManifestChanged> onChange)
        {
            _onManifestChanged += onChange;
            if (Manifest != null)
            {
                onChange.Invoke(new ManifestInitialized(Manifest));
            }
        }
        public void RemoveObserver(Action<DungeonManifestChanged> onChange) => _onManifestChanged -= onChange;
        internal void NotifyObservers(DungeonManifestChanged change) => _onManifestChanged?.Invoke(change);
        public void AddObserver(Action<CacheUpdateData> handler) => MaterialCache.AddObserver(handler);
        public void RemoveObserver(Action<CacheUpdateData> handler) => MaterialCache.RemoveObserver(handler);
        internal void NotifyObservers(DungeonChangeEvent change) => _onDungeonChanged?.Invoke(change);
        private readonly HashSet<TileReference> _cachedTileChanges = new();
        internal void NotifyTileChanges()
        {
            if (_preventNotify) { return; }
            if (!_cachedTileChanges.Any()) { return; }
            _onDungeonChanged?.Invoke(new TilesChanged(_cachedTileChanges.AsEnumerable()));
            _cachedTileChanges.Clear();
        }
        internal void AddTileChange(TileReference tile) => _cachedTileChanges.Add(tile);
        public void AddObserver(Action<DungeonChangeEvent> handle)
        {
            _onDungeonChanged += handle;
            if (CurrentDungeon != null)
            {
                handle.Invoke(new DungeonLoaded(CurrentDungeon));
            }
        }
        public void RemoveObserver(Action<DungeonChangeEvent> handle) => _onDungeonChanged -= handle;
    }
}