using UnityEngine;

using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Unity;

using NaughtyAttributes;
using UnityEngine.Events;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{
    public class DungeonBuilder : MonoBehaviour
    {
        private Dictionary<string, Material> s_materialCache;
        public Dictionary<string, Material> MaterialCache => s_materialCache ?? throw new System.Exception("Material Cache has not yet been initialized.");
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
        [field: SerializeField]
        public DungeonData DungeonData { get; private set; }
        [field: SerializeField]
        public UnityEvent<DungeonTile> OnDungeonTileClicked { get; private set; }

        public void Start()
        {
            BuildManifest();
        }

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
            DungeonData.Dungeon = d;
            
            Build(d, manifest);
        }
        [Button]
        public void ClearDungeon() => TileParent.DestroyAllChildren();
        public Dictionary<string, Material> InitializeMaterialCache(DungeonCrawlerManifest manifest)
        {
            s_materialCache = manifest.Textures.Values.ToDictionary(t => t.Name, t => t.ToMaterial());
            return s_materialCache;
        }
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
                    DungeonTile newTile = DungeonTile.Create(tilePrefab, parent, MaterialCache, manifest, dungeon, position);
                    newTile.OnClicked.AddListener(HandleTileClicked);
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

        private void HandleTileClicked(DungeonTile clicked) => OnDungeonTileClicked.Invoke(clicked);
    }
}