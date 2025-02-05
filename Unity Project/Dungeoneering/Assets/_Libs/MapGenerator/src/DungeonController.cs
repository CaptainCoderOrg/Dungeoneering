using UnityEngine;

using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonCrawler;

using NaughtyAttributes;
using UnityEngine.Events;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{
    public class DungeonController : MonoBehaviour
    {
        
        [field: SerializeField]
        public Transform TileParent { get; private set; } = null!;
        [field: SerializeField]
        public DungeonTile TilePrefab { get; private set; } = null!;
        [field: SerializeField]
        public DungeonManifestData ManifestData { get; private set; } = null!;
        [field: SerializeField]
        public string DungeonKey { get; private set; }
        [field: SerializeField]
        public string[] Keys { get; private set; }
        [field: SerializeField]
        public DungeonData DungeonData { get; private set; }
        [field: SerializeField]
        public UnityEvent<DungeonTile> OnDungeonTileClicked { get; private set; }

        public void Start()
        {
            BuildSelectedDungeon();
        }

        [Button]
        public void BuildSelectedDungeon()
        {
            Keys = ManifestData.Manifest.Dungeons.Keys.ToArray();
            if (!ManifestData.Manifest.Dungeons.TryGetValue(DungeonKey, out Dungeon d))
            {
                d = ManifestData.Manifest.Dungeons[ManifestData.Manifest.Dungeons.Keys.First()];
            }
            DungeonData.Dungeon = d;
            
            Build(d, ManifestData.Manifest);
        }
        [Button]
        public void ClearDungeon() => TileParent.DestroyAllChildren();
        public void Build(Dungeon dungeon, DungeonCrawlerManifest manifest) => BuildDungeon(TileParent, TilePrefab, dungeon, manifest);

        public void BuildDungeon(Transform parent, DungeonTile tilePrefab, Dungeon dungeon, DungeonCrawlerManifest manifest)
        {
            Dictionary<Position, DungeonTile> allTiles = new();
            parent.DestroyAllChildren();

            for (int x = 0; x < 24; x++)
            {
                for (int y = 0; y < 24; y++)
                {
                    Position position = new(x, y);
                    DungeonTile newTile = DungeonTile.Create(tilePrefab, parent, ManifestData.MaterialCache, ManifestData, dungeon, position);
                    newTile.OnClicked.AddListener(HandleTileClicked);
                    allTiles[new Position(x, y)] = newTile;
                }
            }

            dungeon.Walls.OnWallChanged += UpdateWalls;
            dungeon.WallTextures.OnTextureChange += UpdateTextures;
            void UpdateWalls(Position position, Facing facing, WallType wall)
            {
                DungeonTile toUpdate = allTiles[position];
                toUpdate.UpdateWalls(dungeon.GetTile(position).Walls, ManifestData.MaterialCache.GetTileWallMaterials(dungeon, position));
            }
            void UpdateTextures(Position position, Facing facing, string textureName)
            {
                DungeonTile toUpdate = allTiles[position];
                toUpdate.UpdateWalls(dungeon.GetTile(position).Walls, ManifestData.MaterialCache.GetTileWallMaterials(dungeon, position));
            }
        }

        private void HandleTileClicked(DungeonTile clicked) => OnDungeonTileClicked.Invoke(clicked);
    }
}