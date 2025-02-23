using UnityEngine;
using UnityEngine.Events;
namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{
    [CreateAssetMenu(menuName = "DC/DungeonData")]
    public class DungeonData : ObservableSO
    {
        public UnityEvent<Dungeon> OnChange { get; private set; } = new();
        public UnityEvent<TilesChangedData> OnTilesChanged { get; private set; } = new();
        private Dungeon _dungeon;
        private TilesChangedData _changes = new();
        public Dungeon Dungeon
        {
            get => _dungeon;
            set
            {
                _dungeon = value;
                OnChange.Invoke(_dungeon);
            }
        }

        public void SetFloorTexture(Position position, string textureName)
        {
            Dungeon.TileTextures.Textures[position] = textureName;
            _changes.AddChange(Dungeon, position);
        }

        public void SetWallTexture(Position position, Facing facing, string textureName)
        {
            Dungeon.SetTexture(position, facing, textureName);
            _changes.AddChange(Dungeon, position);
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

        protected override void OnExitPlayMode()
        {
            base.OnExitPlayMode();
            Clear();
        }

        public void Clear()
        {
            OnTilesChanged.RemoveAllListeners();
            OnChange.RemoveAllListeners();
        }
    }
}