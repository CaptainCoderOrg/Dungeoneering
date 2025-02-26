using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.Unity;

using NaughtyAttributes;

using UnityEngine;
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
        private DungeonCrawlerManifest _currentManifest;
        [field: SerializeField]
        public string DungeonKey { get; private set; }
        [field: SerializeField]
        public string[] Keys { get; private set; }
        [field: SerializeField]
        public DungeonData DungeonData { get; private set; }
        [field: SerializeField]
        public UnityEvent<DungeonData> OnDungeonChanged { get; private set; } = new();
        [field: SerializeField]
        public UnityEvent<DungeonTile> OnDungeonTileClicked { get; private set; }
        [field: SerializeField]
        public UnityEvent<DungeonWallController> OnDungeonWallClicked { get; private set; }
        private Dictionary<Position, DungeonTile> _tiles = new();

        void OnEnable()
        {
            ManifestData.AddListener(OnManifestLoaded);
            DungeonData.OnTilesChanged.AddListener(UpdateTiles);
        }

        void OnDisable()
        {
            ManifestData.RemoveListener(OnManifestLoaded);
            DungeonData.OnTilesChanged.RemoveListener(UpdateTiles);
        }

        private void OnManifestLoaded(DungeonCrawlerManifest manifest)
        {
            if (_currentManifest == manifest) { return; }

            Keys = ManifestData.Manifest.Dungeons.Keys.ToArray();
            _currentManifest = manifest;
            Dungeon d = manifest.Dungeons.First().Value;
            Build(d);
        }

        [Button]
        public void ClearDungeon() => TileParent.DestroyAllChildren();

        public void Build(Dungeon dungeon) => BuildDungeon(TileParent, TilePrefab, dungeon);

        public void BuildDungeon(Transform parent, DungeonTile tilePrefab, Dungeon d)
        {
            DungeonData.Dungeon = d.Copy();
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

            OnDungeonChanged.Invoke(DungeonData);

            DungeonData.Dungeon.Walls.OnWallChanged += UpdateWalls;
            DungeonData.Dungeon.WallTextures.OnTextureChange += UpdateTextures;

            void UpdateWalls(Position position, Facing facing, WallType wall)
            {
                DungeonTile toUpdate = _tiles.GetValueOrDefault(position);
                toUpdate?.UpdateWalls(DungeonData.Dungeon.GetTile(position).Walls, ManifestData.MaterialCache.GetTileWallMaterials(DungeonData.Dungeon, position));
            }

            void UpdateTextures(Position position, Facing facing, string textureName)
            {
                DungeonTile toUpdate = _tiles.GetValueOrDefault(position);
                toUpdate?.UpdateWalls(DungeonData.Dungeon.GetTile(position).Walls, ManifestData.MaterialCache.GetTileWallMaterials(DungeonData.Dungeon, position));
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
            if (dungeon == DungeonData.Dungeon)
            {
                DungeonTile tile = _tiles[position];
                _ = dungeon.TileTextures.GetTileTextureName(position);

                SelectableMaterial mat = ManifestData.MaterialCache.GetTileMaterial(dungeon, position);
                tile.UpdateFloor(mat);
            }
            else
            {
                Debug.Log($"No scene changes made. Current dungeon is {DungeonData.Dungeon.Name}");
            }
        }

        private void HandleTileClicked(DungeonTile clicked) => OnDungeonTileClicked.Invoke(clicked);
        private void HandleWallClicked(DungeonWallController clicked) => OnDungeonWallClicked.Invoke(clicked);
    }
}