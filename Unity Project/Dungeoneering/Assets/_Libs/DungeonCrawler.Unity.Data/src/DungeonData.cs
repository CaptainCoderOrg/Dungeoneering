using System.Collections.Generic;

using CaptainCoder.DungeonCrawler.Unity.Data;
using CaptainCoder.Dungeoneering.Unity;
using CaptainCoder.Unity;

using UnityEngine;
using UnityEngine.Events;
namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{
    [CreateAssetMenu(menuName = "DC/DungeonData")]
    public class DungeonData : ObservableSO
    {
        private bool _hasChanged = false;
        public bool HasChanged
        {
            get => _hasChanged;
            private set
            {
                if (value == _hasChanged) { return; }
                _hasChanged = value;
                OnStateChanged.Invoke(_dungeon, _hasChanged);
            }
        }
        [SerializeField]
        private UndoRedoStackData _undoRedoStack;
        [SerializeField]
        private MaterialCacheData _materialCacheData;
        public UnityEvent<Dungeon, bool> OnStateChanged { get; private set; } = new();
        public UnityEvent<DungeonChangedData> OnChange { get; private set; } = new();
        public UnityEvent<TilesChangedData> OnTilesChanged { get; private set; } = new();
        private Dungeon _dungeon;
        private TilesChangedData _changes = new();
        public Dungeon Dungeon
        {
            get => _dungeon;
            set
            {
                if (_dungeon == value) { return; }
                if (_dungeon != null)
                {
                    _dungeon.Walls.OnWallChanged -= HandleWallChanged;
                    _dungeon.WallTextures.OnTextureChange -= HandleWallTextureChanged;
                }
                HasChanged = false;
                DungeonChangedData change = new(_dungeon, value);
                _dungeon = value;
                _dungeon.Walls.OnWallChanged += HandleWallChanged;
                _dungeon.WallTextures.OnTextureChange += HandleWallTextureChanged;
                _undoRedoStack.Clear();
                OnChange.Invoke(change);
            }
        }

        private void HandleWallTextureChanged(Position _, Facing __, string ___) => HasChanged = true;
        private void HandleWallChanged(Position _, Facing __, WallType ___) => HasChanged = true;

        public void SaveToManifest(DungeonManifestData manifest)
        {
            manifest.UpdateDungeon(_dungeon);
            HasChanged = false;
        }

        public void SetFloorTexture(Position position, string textureName)
        {
            _materialCacheData.Cache.SetTexture(new TileReference(Dungeon, position), _materialCacheData.Cache.GetTextureId(textureName));
            _changes.AddChange(Dungeon, position);
            HasChanged = true;
        }

        public void RemoveFloorTexture(Position position)
        {
            Dungeon.TileTextures.Textures.Remove(position);
            _changes.AddChange(Dungeon, position);
            HasChanged = true;
        }

        public void SetWallTexture(Position position, Facing facing, string textureName)
        {
            Dungeon.SetTexture(position, facing, textureName);
            _changes.AddChange(Dungeon, position);
            HasChanged = true;
        }

        public void SetWallType(Position position, Facing facing, WallType type)
        {
            if (Dungeon.Walls[position, facing] == type) { return; }
            Dungeon.WallTextures.Textures.Remove((position, facing));
            Dungeon.WallTextures.Textures.Remove((position.Step(facing), facing.Opposite()));
            Dungeon.Walls.SetWall(position, facing, type);
            _changes.AddChange(Dungeon, position);
        }

        public void Notify()
        {
            if (_changes.Tiles.Count == 0) { return; }
            OnTilesChanged.Invoke(_changes);
            _changes = new();
        }

        public string GetFloorTexture(Position p) => Dungeon.TileTextures.GetTileTextureName(p);
        public string GetWallTexture(Position p, Facing f) => Dungeon.GetWallTexture(p, f);

        private void ConnectToCache()
        {
            _materialCacheData.Cache.DungeonData = this;
        }

        protected override void AfterEnabled()
        {
            base.AfterEnabled();
            // TODO: This feels quite brittle, perhaps a parent object that wires things up for us
            ConnectToCache();
        }

        protected override void OnExitEditMode()
        {
            base.OnExitEditMode();
            Clear();
        }

        protected override void OnEnterPlayMode()
        {
            base.OnEnterPlayMode();
            Debug.Assert(_undoRedoStack != null);
            ConnectToCache();
        }

        protected override void OnExitPlayMode()
        {
            base.OnExitPlayMode();
            Clear();
        }

        public void Clear()
        {
            OnTilesChanged.RemoveAllListeners();
            OnChange.RemoveAllListeners();
            OnStateChanged.RemoveAllListeners();
            _changes = new();
            _dungeon = null;
            _hasChanged = false;
        }


    }

    public record DungeonChangedData(Dungeon Previous, Dungeon New);

    public class TilesChangedData
    {
        public HashSet<(Dungeon, Position)> Tiles { get; private set; } = new();
        public bool AddChange(Dungeon dungeon, Position position) => Tiles.Add((dungeon, position));
    }
}