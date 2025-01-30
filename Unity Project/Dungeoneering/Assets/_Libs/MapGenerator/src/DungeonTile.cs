using UnityEngine;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{
    public class DungeonTile : MonoBehaviour
    {
        [field: SerializeField]
        public MeshRenderer NorthWall { get; private set; } = default!;
        [field: SerializeField]
        public MeshRenderer EastWall { get; private set; } = default!;
        [field: SerializeField]
        public MeshRenderer SouthWall { get; private set; } = default!;
        [field: SerializeField]
        public MeshRenderer WestWall { get; private set; } = default!;
        [field: SerializeField]
        public MeshRenderer FloorTile { get; private set; } = default!;

        public void UpdateFloor(Material material)
        {
            FloorTile.material = material;
        }

        public void UpdateWalls(TileWalls configuration, TileWallMaterials materials)
        {
            NorthWall.material = materials.North;
            NorthWall.gameObject.SetActive(configuration.North is not WallType.None);

            EastWall.material = materials.East;
            EastWall.gameObject.SetActive(configuration.East is not WallType.None);

            WestWall.material = materials.West;
            WestWall.gameObject.SetActive(configuration.West is not WallType.None);

            SouthWall.material = materials.South;
            SouthWall.gameObject.SetActive(configuration.South is not WallType.None);
        }
    }
}