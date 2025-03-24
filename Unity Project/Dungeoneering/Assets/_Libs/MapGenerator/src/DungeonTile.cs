using System;

using CaptainCoder.Dungeoneering.Unity.Data;

using UnityEngine;
using UnityEngine.Events;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{
    public class DungeonTile : MonoBehaviour, ISelectable
    {
        public TileReference TileReference => new(Dungeon, Position);
        public Dungeon Dungeon => DungeonCrawlerData.CurrentDungeon;
        public DungeonCrawlerData DungeonCrawlerData { get; private set; }
        public Position Position { get; private set; }
        [field: SerializeField]
        public UnityEvent<DungeonTile> OnClicked { get; private set; }
        [field: SerializeField]
        public UnityEvent<DungeonWallController> OnWallClicked { get; private set; }
        [field: SerializeField]
        public DungeonWallController NorthWall { get; private set; } = default!;
        [field: SerializeField]
        public DungeonWallController EastWall { get; private set; } = default!;
        [field: SerializeField]
        public DungeonWallController SouthWall { get; private set; } = default!;
        [field: SerializeField]
        public DungeonWallController WestWall { get; private set; } = default!;
        [field: SerializeField]
        public MeshRenderer FloorTile { get; private set; } = default!;
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value)
                    return;
                _isSelected = value;
                IsSelectedChanged(_isSelected);
            }
        }

        public DungeonWallController this[Facing facing]
        {
            get
            {
                return facing switch
                {
                    Facing.North => NorthWall,
                    Facing.East => EastWall,
                    Facing.South => SouthWall,
                    Facing.West => WestWall,
                    _ => throw new ArgumentOutOfRangeException(nameof(facing), facing, null),
                };
            }
        }

        public string FloorTextureName => Dungeon.TileTextures.GetTileTextureName(Position);
        public void Click() => OnClicked.Invoke(this);

        public static DungeonTile Create(DungeonTile prefab, Transform parent, DungeonCrawlerData dungeonCrawlerData, Position position)
        {
            DungeonTile newTile = Instantiate(prefab, parent);
            newTile.DungeonCrawlerData = dungeonCrawlerData;
            newTile.Position = position;
            newTile.name = $"({position.X}, {position.Y})";
            newTile.transform.position = new Vector3(position.Y, 0, position.X);
            UpdateTile(dungeonCrawlerData, position, newTile);
            return newTile;
        }

        public static void UpdateTile(DungeonCrawlerData dungeonCrawlerData, Position position, DungeonTile newTile)
        {
            newTile.UpdateFloor(dungeonCrawlerData.GetTexture(newTile.TileReference));
            newTile.UpdateWalls(newTile.Dungeon.GetTile(position).Walls, dungeonCrawlerData.GetTileWallTextures(newTile.TileReference));
        }

        private void IsSelectedChanged(bool isSelected)
        {
            UpdateFloor(DungeonCrawlerData.GetTexture(new TileReference(Dungeon, Position)), isSelected);
        }

        public void SetAllWallsSelected(bool isSelected)
        {
            NorthWall.IsSelected = isSelected;
            EastWall.IsSelected = isSelected;
            SouthWall.IsSelected = isSelected;
            WestWall.IsSelected = isSelected;
        }

        public void UpdateFloor(TextureReference texture) => UpdateFloor(texture, IsSelected);
        public void UpdateFloor(TextureReference texture, bool isSelected)
        {
            FloorTile.material = texture.Material.GetMaterial(isSelected);
        }

        public void UpdateWalls(TileWalls configuration, TileWallTextures textures)
        {
            NorthWall.Material = textures.North.Material;
            NorthWall.gameObject.SetActive(configuration.North is not WallType.None);

            EastWall.Material = textures.East.Material;
            EastWall.gameObject.SetActive(configuration.East is not WallType.None);

            WestWall.Material = textures.West.Material;
            WestWall.gameObject.SetActive(configuration.West is not WallType.None);

            SouthWall.Material = textures.South.Material;
            SouthWall.gameObject.SetActive(configuration.South is not WallType.None);
        }

        public void SetTexture(TextureReference newTexture) => DungeonCrawlerData.SetTexture(TileReference, newTexture);
    }
}