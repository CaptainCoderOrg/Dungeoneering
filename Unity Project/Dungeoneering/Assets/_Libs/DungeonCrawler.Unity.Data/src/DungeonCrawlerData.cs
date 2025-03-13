using System;
using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Unity;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity.Data
{
    [CreateAssetMenu(menuName = "DC/DungeonCrawlerData")]
    public class DungeonCrawlerData : ObservableSO
    {
        private bool _preventNotify = false;
        internal bool PreventNotify
        {
            get => _preventNotify;
            set
            {
                _preventNotify = value;
                NotifyTileChanges();
            }
        }

        [field: SerializeField]
        public TextAsset DefaultManifestJson { get; private set; }
        internal readonly UndoRedoStack UndoRedoStack = new();
        public DungeonCrawlerManifest Manifest { get; internal set; }
        private Action<DungeonManifestChanged> _onManifestChanged;

        public Dungeon CurrentDungeon { get; internal set; }
        private Action<DungeonChangeEvent> _onDungeonChanged;
        private readonly HashSet<TileReference> _cachedTileChanges = new();

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

        internal MaterialCache MaterialCache { get; private set; }

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

        public void AddObserver(Action<DungeonChangeEvent> handle)
        {
            _onDungeonChanged += handle;
            if (CurrentDungeon != null)
            {
                handle.Invoke(new DungeonLoaded(CurrentDungeon));
            }
        }
        public void RemoveObserver(Action<DungeonChangeEvent> handle) => _onDungeonChanged -= handle;
        internal void NotifyObservers(DungeonChangeEvent change) => _onDungeonChanged?.Invoke(change);
        internal void NotifyTileChanges()
        {
            if (_preventNotify) { return; }
            if (!_cachedTileChanges.Any()) { return; }
            _onDungeonChanged?.Invoke(new TilesChanged(_cachedTileChanges.AsEnumerable()));
            _cachedTileChanges.Clear();
        }
        internal void AddTileChange(TileReference tile) => _cachedTileChanges.Add(tile);


        public void AddObserver(Action<CacheUpdateData> handler) => MaterialCache.AddObserver(handler);
        public void RemoveObserver(Action<CacheUpdateData> handler) => MaterialCache.RemoveObserver(handler);

        public void Undo() => UndoRedoStack.Undo();
        public void Redo() => UndoRedoStack.Redo();
    }
}