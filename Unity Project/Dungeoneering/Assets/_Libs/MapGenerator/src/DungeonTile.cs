using System;
using System.Collections.Generic;
using CaptainCoder.Dungeoneering.DungeonCrawler;
using UnityEngine;
using UnityEngine.Events;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{
    public class DungeonTile : MonoBehaviour
    {
        
        public DungeonManifestData Manifest { get; private set; }
        public Dungeon Dungeon { get; private set; }
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

        public string FloorTextureName => Dungeon.TileTextures.GetTileTextureName(Position);
        public void Click() => OnClicked.Invoke(this);

        public static DungeonTile Create(DungeonTile prefab, Transform parent, Dictionary<string, Material> cache, DungeonManifestData manifest, Dungeon dungeon, Position position)
        {
            bool wasActive = prefab.gameObject.activeSelf;
            prefab.gameObject.SetActive(false);
            DungeonTile newTile = Instantiate(prefab, parent);
            newTile.Manifest = manifest;
            newTile.Dungeon = dungeon;
            newTile.Position = position;
            newTile.name = $"({position.X}, {position.Y})";
            newTile.transform.position = new Vector3(position.Y, 0, position.X);
            newTile.UpdateFloor(cache.GetTileMaterial(dungeon, position));
            newTile.UpdateWalls(dungeon.GetTile(position).Walls, cache.GetTileWallMaterials(dungeon, position));
            prefab.gameObject.SetActive(wasActive);
            newTile.gameObject.SetActive(wasActive);
            return newTile;
        }

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

        public void SetTexture(string textureName)
        {
            Manifest.SetFloorTexture(Dungeon, Position, textureName);
        }
    }
}