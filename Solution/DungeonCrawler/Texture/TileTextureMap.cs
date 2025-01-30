using CaptainCoder.Utils.DictionaryExtensions;

namespace CaptainCoder.Dungeoneering.DungeonMap;

public class TileTextureMap : IEquatable<TileTextureMap>
{
    public const string DefaultTexture = "default-tile.png";
    public string Default { get; set; } = DefaultTexture;
    public Dictionary<Position, string> Textures { get; init; } = new();
    public bool Equals(TileTextureMap other) =>
        Default == other.Default &&
        Textures.AllKeyValuesAreEqual(other.Textures);
}

public static class TileTextureMapExtensions
{
    public static string GetTileTextureName(this TileTextureMap map, Position position) => map.Textures.GetValueOrDefault(position, map.Default);
}