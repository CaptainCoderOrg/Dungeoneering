using UnityEngine;

using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Unity;

using NaughtyAttributes;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{
    public class DungeonBuilder : MonoBehaviour
    {
        private static Dictionary<string, Material> s_materialCache;
        public static Dictionary<string, Material> MaterialCache => s_materialCache ?? throw new System.Exception("Material Cache has not yet been initialized.");
        [field: SerializeField]
        public Transform TileParent { get; private set; } = null!;
        [field: SerializeField]
        public DungeonTile TilePrefab { get; private set; } = null!;
        [field: SerializeField]
        public DungeonManifestData Manifest { get; private set; } = null!;
        [field: SerializeField]
        public string DungeonKey { get; private set; }
        [field: SerializeField]
        public string[] Keys { get; private set; }
        [Button]
        public void BuildManifest()
        {
            DungeonCrawlerManifest manifest = Manifest.LoadManifest();
            InitializeMaterialCache(manifest);
            Keys = manifest.Dungeons.Keys.ToArray();
            if (!manifest.Dungeons.TryGetValue(DungeonKey, out Dungeon d))
            {
                d = manifest.Dungeons[manifest.Dungeons.Keys.First()];
            }
            
            Build(d);
        }
        public static Dictionary<string, Material> InitializeMaterialCache(DungeonCrawlerManifest manifest)
        {
            s_materialCache = manifest.Textures.Values.ToDictionary(t => t.Name, t => t.ToMaterial());
            return s_materialCache;
        }
        public void Build(Dungeon dungeon) => BuildDungeon(TileParent, TilePrefab, dungeon);

        public static void BuildDungeon(Transform parent, DungeonTile tilePrefab, Dungeon dungeon)
        {
            Dictionary<Position, DungeonTile> allTiles = new();
            parent.DestroyAllChildren();

            for (int x = 0; x < 24; x++)
            {
                for (int y = 0; y < 24; y++)
                {
                    DungeonTile newTile = Instantiate(tilePrefab, parent);
                    newTile.name = $"({x}, {y})";
                    newTile.transform.position = new Vector3(y, 0, x);
                    Position position = new(x, y);
                    newTile.UpdateFloor(MaterialCache.GetTileMaterial(dungeon, position));
                    newTile.UpdateWalls(dungeon.GetTile(position).Walls, MaterialCache.GetTileWallMaterials(dungeon, position));
                    allTiles[new Position(x, y)] = newTile;
                }
            }

            dungeon.Walls.OnWallChanged += UpdateWalls;
            dungeon.WallTextures.OnTextureChange += UpdateTextures;
            void UpdateWalls(Position position, Facing facing, WallType wall)
            {
                DungeonTile toUpdate = allTiles[position];
                toUpdate.UpdateWalls(dungeon.GetTile(position).Walls, MaterialCache.GetTileWallMaterials(dungeon, position));
            }
            void UpdateTextures(Position position, Facing facing, string textureName)
            {
                DungeonTile toUpdate = allTiles[position];
                toUpdate.UpdateWalls(dungeon.GetTile(position).Walls, MaterialCache.GetTileWallMaterials(dungeon, position));
            }
        }
    }
}