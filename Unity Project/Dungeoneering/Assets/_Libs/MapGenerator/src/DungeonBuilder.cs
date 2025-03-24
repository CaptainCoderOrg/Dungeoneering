using System.Collections.Generic;

using CaptainCoder.Dungeoneering.Unity.Data;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{
    public class DungeonBuilder
    {
        public int MinX { get; private set; } = 0;
        public int MaxX { get; private set; } = DungeonGlobals.DIMENSION;
        public int MinY { get; private set; } = 0;
        public int MaxY { get; private set; } = DungeonGlobals.DIMENSION;
        private int CellCount => (MaxX - MinX + 1) * (MaxY - MinY + 1);
        private Dictionary<Position, DungeonTile> _tiles = new();
        public bool HasTile(Position position) => _tiles.ContainsKey(position);
        public DungeonTile GetDungeonTile(Position position) => _tiles[position];
        public bool TryGetDungeonTile(TileReference tileRef, out DungeonTile tile) => _tiles.TryGetValue(tileRef.Position, out tile);
        public bool TryGetDungeonTile(Position position, out DungeonTile tile) => _tiles.TryGetValue(position, out tile);
        public void BuildOrUpdateTiles(Transform parent, DungeonTile tilePrefab, System.Action<DungeonTile, Position> updateTile, System.Func<DungeonTile, Transform, Position, DungeonTile> createTile)
        {
            Dictionary<Position, DungeonTile> pooledTiles = _tiles;
            _tiles = new(Mathf.Max(_tiles.Count, CellCount));

            for (int x = MinX; x < MaxX; x++)
            {
                for (int y = MinY; y < MaxY; y++)
                {
                    Position position = new(x, y);
                    if (pooledTiles.TryGetValue(position, out DungeonTile tile))
                    {
                        updateTile.Invoke(tile, position);
                        pooledTiles.Remove(position);
                    }
                    else
                    {
                        tile = createTile.Invoke(tilePrefab, parent, position);
                    }

                    _tiles[new Position(x, y)] = tile;
                }
            }

            foreach (var tile in pooledTiles.Values)
            {
                Object.Destroy(tile.gameObject);
            }
        }
    }
}