using UnityEngine;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{
    public class DungeonTile : MonoBehaviour
    {
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

        public void UpdateFloor(Material material)
        {
            FloorTile.material = material;
        }

        public void UpdateWalls(TileWalls configuration, TileWallMaterials materials)
        {
            NorthWall.Material = materials.North;
            NorthWall.gameObject.SetActive(configuration.North is not WallType.None);

            EastWall.Material = materials.East;
            EastWall.gameObject.SetActive(configuration.East is not WallType.None);

            WestWall.Material = materials.West;
            WestWall.gameObject.SetActive(configuration.West is not WallType.None);

            SouthWall.Material = materials.South;
            SouthWall.gameObject.SetActive(configuration.South is not WallType.None);
        }
    }
}