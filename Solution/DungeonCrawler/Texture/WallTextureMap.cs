using CaptainCoder.Utils.DictionaryExtensions;

namespace CaptainCoder.Dungeoneering.DungeonMap;

public class WallTextureMap : IEquatable<WallTextureMap>
{
    public const string DefaultSolidTexture = "default-wall.png";
    public const string DefaultDoorTexture = "default-door.png";
    public const string DefaultSecretDoorTexture = "default-secret-door.png";
    public string DefaultSolid { get; set; } = DefaultSolidTexture;
    public string DefaultDoor { get; set; } = DefaultDoorTexture;
    public string DefaultSecretDoor { get; set; } = DefaultSecretDoorTexture;
    public Dictionary<(Position, Facing), string> Textures { get; init; } = new();
    public event Action<Position, Facing, string>? OnTextureChange;
    internal void Notify(Position position, Facing facing, string newTextureName) => OnTextureChange?.Invoke(position, facing, newTextureName);
    public void ClearListeners() => OnTextureChange = null;
    public bool Equals(WallTextureMap other) =>
        DefaultSolid == other.DefaultSolid &&
        DefaultDoor == other.DefaultDoor &&
        DefaultSecretDoor == other.DefaultSecretDoor &&
        Textures.AllKeyValuesAreEqual(other.Textures);
}

public static class WallTextureMapExtensions
{
    public static string GetWallTexture(this Dungeon dungeon, Position position, Facing facing) =>
    dungeon.Walls.GetWall(position, facing) switch
    {
        _ when dungeon.WallTextures.Textures.TryGetValue((position, facing), out string texture) is true => texture,
        WallType.Solid => dungeon.WallTextures.DefaultSolid,
        WallType.Door => dungeon.WallTextures.DefaultDoor,
        WallType.SecretDoor => dungeon.WallTextures.DefaultSecretDoor,
        _ => "No Texture",
    };

    /// <summary>
    /// Sets the texture at the specified position and facing. Then, notifies any observers of the change.
    /// </summary>
    public static void SetTexture(this Dungeon dungeon, Position position, Facing facing, string textureName)
    {
        dungeon.WallTextures.Textures[(position, facing)] = textureName;
        dungeon.WallTextures.Notify(position, facing, textureName);
    }
}