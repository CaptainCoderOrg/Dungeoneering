using System.Collections.Generic;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{
    public static class DungeonExtensions
    {
        public static SelectableMaterial GetTileMaterial(this Dictionary<string, SelectableMaterial> cache, Dungeon dungeon, Position position) => cache[dungeon.TileTextures.GetTileTextureName(position)];
        public static TileWallMaterials GetTileWallMaterials(this Dictionary<string, SelectableMaterial> cache, Dungeon dungeon, Position position)
        {
            return new TileWallMaterials()
            {
                North = cache.GetValueOrDefault(dungeon.GetWallTexture(position, Facing.North)),
                East = cache.GetValueOrDefault(dungeon.GetWallTexture(position, Facing.East)),
                South = cache.GetValueOrDefault(dungeon.GetWallTexture(position, Facing.South)),
                West = cache.GetValueOrDefault(dungeon.GetWallTexture(position, Facing.West)),
            };
        }
    }
}