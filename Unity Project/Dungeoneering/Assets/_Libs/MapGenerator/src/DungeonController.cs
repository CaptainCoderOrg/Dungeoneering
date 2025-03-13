using System.Collections.Generic;

using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity;
using CaptainCoder.Unity.Assertions;

using NaughtyAttributes;

using UnityEngine;
using UnityEngine.Events;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{
    public class DungeonController : MonoBehaviour
    {
        [AssertIsSet][field: SerializeField] public DungeonCrawlerData DungeonCrawlerData { get; private set; }
        [AssertIsSet][field: SerializeField] public Transform TileParent { get; private set; } = null!;
        [AssertIsSet][field: SerializeField] public DungeonTile TilePrefab { get; private set; } = null!;
        [AssertIsSet][field: SerializeField] public UnityEvent<DungeonTile> OnDungeonTileClicked { get; private set; }
        [AssertIsSet][field: SerializeField] public UnityEvent<DungeonWallController> OnDungeonWallClicked { get; private set; }
        private Dictionary<Position, DungeonTile> _tiles = new();

        void Awake()
        {
            Assertion.NotNull(this, (DungeonCrawlerData, "DungeonCrawlerData was null"));
        }

        void OnEnable()
        {
            DungeonCrawlerData.AddObserver(HandleDungeonChanged);
        }

        void OnDisable()
        {
            DungeonCrawlerData.RemoveObserver(HandleDungeonChanged);
        }

        private void HandleDungeonChanged(DungeonChangeEvent change)
        {
            switch (change)
            {
                case DungeonLoaded(Dungeon _):
                    UpdateDungeonTiles();
                    break;
                case TilesChanged(IEnumerable<TileReference> tiles):
                    UpdateTiles(tiles);
                    break;
                case DefaultTileTextureChanged(Dungeon dungeon, TextureReference _):
                    if (dungeon == DungeonCrawlerData.CurrentDungeon)
                    {
                        UpdateDungeonTiles();
                    }
                    break;
                case DefaultWallTextureChanged(Dungeon dungeon, WallType _, TextureReference __):
                    if (dungeon == DungeonCrawlerData.CurrentDungeon)
                    {
                        UpdateDungeonTiles();
                    }
                    break;
            }
        }

        [Button]
        public void ClearDungeon() => TileParent.DestroyAllChildren();

        private void UpdateDungeonTiles() => BuildOrUpdateTiles(TileParent, TilePrefab);

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
                        DungeonTile.UpdateTile(this, position, tile);
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
        }

        public bool HasTile(Position position) => _tiles.ContainsKey(position);
        public DungeonTile GetDungeonTile(Position position) => _tiles[position];
        public bool TryGetDungeonTile(Position position, out DungeonTile tile) => _tiles.TryGetValue(position, out tile);

        private void UpdateTiles(IEnumerable<TileReference> tiles)
        {
            foreach (TileReference tileRef in tiles)
            {
                UpdateTile(tileRef);
            }
        }

        private void UpdateTile(TileReference tileRef)
        {
            if (tileRef.Dungeon == DungeonCrawlerData.CurrentDungeon)
            {
                if (_tiles.TryGetValue(tileRef.Position, out DungeonTile tile))
                {
                    tile.UpdateFloor(DungeonCrawlerData.GetTexture(tileRef));
                    tile.UpdateWalls(tileRef.Dungeon.GetTile(tileRef.Position).Walls, DungeonCrawlerData.GetTileWallTextures(tileRef));
                }
            }
            else
            {
                Debug.Log($"No scene changes made. Current dungeon is {DungeonCrawlerData.CurrentDungeon.Name}");
            }
        }

        private void HandleTileClicked(DungeonTile clicked) => OnDungeonTileClicked.Invoke(clicked);
        private void HandleWallClicked(DungeonWallController clicked) => OnDungeonWallClicked.Invoke(clicked);
    }
}