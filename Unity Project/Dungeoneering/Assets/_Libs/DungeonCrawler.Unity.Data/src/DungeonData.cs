using System.Collections.Generic;

using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Unity;

using UnityEngine;
using UnityEngine.Events;
namespace CaptainCoder.Dungeoneering.Unity.Data
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
        private MaterialCache _materialCache;
        public UnityEvent<Dungeon, bool> OnStateChanged { get; private set; } = new();
        public event System.Action<DungeonChangedData> OnChange;
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
                OnChange?.Invoke(change);
            }
        }
        public void AddObserver(System.Action<DungeonChangedData> handle)
        {
            OnChange += handle;
            if (_dungeon != null)
            {
                handle.Invoke(new DungeonChangedData(null, _dungeon));
            }
        }

        public void RemoveObserver(System.Action<DungeonChangedData> handle)
        {
            OnChange -= handle;
        }

        private void HandleWallTextureChanged(Position _, Facing __, string ___) => HasChanged = true;
        private void HandleWallChanged(Position _, Facing __, WallType ___) => HasChanged = true;

        public void SaveToManifest(DungeonManifestData manifest)
        {
            manifest.UpdateDungeon(_dungeon);
            HasChanged = false;
        }

        public void SetFloorTexture(Position position, TextureId tId)
        {
            _materialCache.SetTexture(new TileReference(Dungeon, position), tId);
            _changes.AddChange(Dungeon, position);
            HasChanged = true;
        }

        public void RemoveFloorTexture(Position position)
        {
            Dungeon.TileTextures.Textures.Remove(position);
            _changes.AddChange(Dungeon, position);
            HasChanged = true;
        }

        public void SetWallTexture(Position position, Facing facing, TextureId textureId)
        {
            _materialCache.SetTexture(new WallReference(Dungeon, position, facing), textureId);
            _changes.AddChange(Dungeon, position);
            HasChanged = true;
        }

        internal void RemoveWallTexture(Position position, Facing facing)
        {
            Dungeon.WallTextures.Textures.Remove((position, facing));
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
            HasChanged = true;
        }

        public void Notify()
        {
            if (_changes.Tiles.Count == 0) { return; }
            OnTilesChanged.Invoke(_changes);
            _changes = new();
        }

        public TextureId GetFloorTexture(Position p) => _materialCache.GetTextureId(Dungeon, p);
        public TextureId GetWallTexture(Position p, Facing f) => _materialCache.GetWallTexture(new WallReference(Dungeon, p, f));
        public string GetWallTextureName(Position p, Facing f) => Dungeon.GetWallTexture(p, f);

        public void Initialize(MaterialCache materialCache)
        {
            Debug.Assert(_undoRedoStack != null);
            _materialCache = materialCache;
            // _materialCacheData.Cache.DungeonData = this;
        }

        protected override void OnExitPlayMode()
        {
            base.OnExitPlayMode();
            Clear();
        }

        public void Clear()
        {
            OnTilesChanged.RemoveAllListeners();
            OnChange = null;
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