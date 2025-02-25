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
        public UnityEvent<Dungeon, bool> OnStateChanged { get; private set; } = new();
        public UnityEvent<Dungeon> OnChange { get; private set; } = new();
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
                _dungeon = value;
                _dungeon.Walls.OnWallChanged += HandleWallChanged;
                _dungeon.WallTextures.OnTextureChange += HandleWallTextureChanged;
                _undoRedoStack.Clear();
                OnChange.Invoke(_dungeon);
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
            Dungeon.TileTextures.Textures[position] = textureName;
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

        protected override void OnExitEditMode()
        {
            base.OnExitEditMode();
            Clear();
        }

        protected override void OnEnterPlayMode()
        {
            base.OnEnterPlayMode();
            Debug.Assert(_undoRedoStack != null);
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
}