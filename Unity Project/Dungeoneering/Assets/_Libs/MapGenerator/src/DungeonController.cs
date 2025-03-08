using System.Collections.Generic;

using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity;

using NaughtyAttributes;

using UnityEngine;
using UnityEngine.Events;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{
    public class DungeonController : MonoBehaviour
    {
        [field: SerializeField]
        public DungeonCrawlerData DungeonCrawlerData { get; private set; }
        [field: SerializeField]
        public Transform TileParent { get; private set; } = null!;
        [field: SerializeField]
        public DungeonTile TilePrefab { get; private set; } = null!;
        [field: SerializeField]
        public UnityEvent<DungeonData> OnDungeonChanged { get; private set; } = new();
        [field: SerializeField]
        public UnityEvent<DungeonTile> OnDungeonTileClicked { get; private set; }
        [field: SerializeField]
        public UnityEvent<DungeonWallController> OnDungeonWallClicked { get; private set; }
        private Dictionary<Position, DungeonTile> _tiles = new();

        void Awake()
        {
            Debug.Log("Controller Awake");
            Assertion.NotNull(this, (DungeonCrawlerData, "DungeonCrawlerData was null"));
        }

        void OnEnable()
        {
            Debug.Log("Controller Enable");
            DungeonCrawlerData.CurrentDungeon.OnTilesChanged.AddListener(UpdateTiles);
            DungeonCrawlerData.CurrentDungeon.AddObserver(HandleDungeonChanged);
        }

        void OnDisable()
        {
            DungeonCrawlerData.CurrentDungeon.OnTilesChanged.RemoveListener(UpdateTiles);
            DungeonCrawlerData.CurrentDungeon.RemoveObserver(HandleDungeonChanged);
        }

        private void HandleDungeonChanged(DungeonChangedData _) => UpdateDungeonTiles();

        [Button]
        public void ClearDungeon() => TileParent.DestroyAllChildren();

        public void UpdateDungeonTiles() => BuildOrUpdateTiles(TileParent, TilePrefab);

        private void BuildOrUpdateTiles(Transform parent, DungeonTile tilePrefab)
        {
            Dictionary<Position, DungeonTile> pooledTiles = _tiles;
            _tiles = new(Mathf.Max(_tiles.Count, DungeonGlobals.DIMENSION * DungeonGlobals.DIMENSION));

            for (int x = 0; x < DungeonGlobals.DIMENSION; x++)
            {
                for (int y = 0; y < DungeonGlobals.DIMENSION; y++)
                {
                    Position position = new(x, y);
                    if (pooledTiles.TryGetValue(position, out DungeonTile tile))
                    {
                        tile.IsSelected = false;
                        tile.SetAllWallsSelected(false);
                        DungeonTile.UpdateTile(this, position, tilePrefab.gameObject.activeSelf, tile);
                        pooledTiles.Remove(position);
                    }
                    else
                    {
                        tile = DungeonTile.Create(tilePrefab, parent, this, position);
                        tile.OnClicked.AddListener(HandleTileClicked);
                        tile.OnWallClicked.AddListener(HandleWallClicked);
                    }

                    _tiles[new Position(x, y)] = tile;
                }
            }

            foreach (var tile in pooledTiles.Values)
            {
                Destroy(tile.gameObject);
            }

            // TODO: We should be able to eliminate this and replace with DungeonData.OnChange
            OnDungeonChanged.Invoke(DungeonCrawlerData.CurrentDungeon);

            // TODO: I suspect there is a memory leak here, we never unregister these
            DungeonCrawlerData.CurrentDungeon.Dungeon.Walls.OnWallChanged += UpdateWalls;
            DungeonCrawlerData.CurrentDungeon.Dungeon.WallTextures.OnTextureChange += UpdateTextures;

            void UpdateWalls(Position position, Facing facing, WallType wall)
            {
                DungeonTile toUpdate = _tiles.GetValueOrDefault(position);
                toUpdate?.UpdateWalls(DungeonCrawlerData.CurrentDungeon.Dungeon.GetTile(position).Walls, DungeonCrawlerData.MaterialCache.GetTileWallMaterials(DungeonCrawlerData.CurrentDungeon.Dungeon, position));
            }

            void UpdateTextures(Position position, Facing facing, string textureName)
            {
                DungeonTile toUpdate = _tiles.GetValueOrDefault(position);
                toUpdate?.UpdateWalls(DungeonCrawlerData.CurrentDungeon.Dungeon.GetTile(position).Walls, DungeonCrawlerData.MaterialCache.GetTileWallMaterials(DungeonCrawlerData.CurrentDungeon.Dungeon, position));
            }
        }

        public bool HasTile(Position position) => _tiles.ContainsKey(position);
        public DungeonTile GetDungeonTile(Position position) => _tiles[position];
        public bool TryGetDungeonTile(Position position, out DungeonTile tile) => _tiles.TryGetValue(position, out tile);

        private void UpdateTiles(TilesChangedData changes)
        {
            foreach ((Dungeon d, Position p) in changes.Tiles)
            {
                UpdateTile(d, p);
            }
        }

        private void UpdateTile(Dungeon dungeon, Position position)
        {
            if (dungeon == DungeonCrawlerData.CurrentDungeon.Dungeon)
            {
                DungeonTile tile = _tiles[position];
                tile.UpdateFloor(DungeonCrawlerData.MaterialCache.GetTexture(dungeon, position));
                tile.UpdateWalls(dungeon.GetTile(position).Walls, DungeonCrawlerData.MaterialCache.GetTileWallMaterials(dungeon, position));
            }
            else
            {
                Debug.Log($"No scene changes made. Current dungeon is {DungeonCrawlerData.CurrentDungeon.Dungeon.Name}");
            }
        }

        private void HandleTileClicked(DungeonTile clicked) => OnDungeonTileClicked.Invoke(clicked);
        private void HandleWallClicked(DungeonWallController clicked) => OnDungeonWallClicked.Invoke(clicked);
    }
}