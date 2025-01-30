using UnityEngine;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity;

using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap.IO;
using CaptainCoder.Unity;

public class DungeonBuilder : MonoBehaviour
{
    private static Dictionary<string, Material>? s_materialCache;
    public static Dictionary<string, Material> MaterialCache => s_materialCache ?? throw new Exception("Material Cache has not yet been initialized.");
    [field: SerializeField]
    public Transform TileParent { get; private set; } = null!;
    [field: SerializeField]
    public DungeonTile TilePrefab { get; private set; } = null!;
    public static Dictionary<string, Material> InitializeMaterialCache(DungeonCrawlerManifest manifest)
    {
        s_materialCache = manifest.Textures.Values.ToDictionary(t => t.Name, t => t.ToMaterial());
        return s_materialCache;
    }
    public void Build(Dungeon dungeon) => BuildDungeon(TileParent, TilePrefab, Destroy, dungeon);

    public static void BuildDungeon(Transform parent, DungeonTile tilePrefab, Action<GameObject> destroy, Dungeon dungeon)
    {
        Dictionary<Position, DungeonTile> allTiles = new();
        parent.DestroyAllChildren(destroy);

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

public static class DungeonExtensions
{
    public static Material GetTileMaterial(this Dictionary<string, Material> cache, Dungeon dungeon, Position position) => cache[dungeon.TileTextures.GetTileTextureName(position)];
    public static TileWallMaterials GetTileWallMaterials(this Dictionary<string, Material> cache, Dungeon dungeon, Position position)
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

[CreateAssetMenu(fileName = "DungeonData", menuName = "Data/Manifest")]
public class DungeonManifestData : ScriptableObject
{
    [field: SerializeField]
    public TextAsset? ManifestJson { get; private set; }

    public DungeonCrawlerManifest LoadManifest() => JsonExtensions.LoadModel<DungeonCrawlerManifest>(ManifestJson!.text);
    public DungeonCrawlerManifest LoadFromFile(string path) => JsonExtensions.LoadModel<DungeonCrawlerManifest>(File.ReadAllText(path));
}

public static class TextureExtensions
{
    public static Material ToMaterial(this Texture texture)
    {
        Texture2D t2d = new(2, 2) { filterMode = FilterMode.Point };
        ImageConversion.LoadImage(t2d, texture.Data);
        Material material = new(Shader.Find("Standard"));
        material.SetFloat("_Glossiness", 0); // Smoothness
        material.SetFloat("_SpecularHighlights", 0); // Specular Highlights
        material.SetFloat("_GlossyReflections", 0); // Reflections
        material.mainTexture = t2d;
        return material;
    }
}