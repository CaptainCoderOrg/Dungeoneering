namespace CaptainCoder.Dungeoneering.DungeonMap;
using static WallType;
public enum WallType
{
    None,
    Solid,
    Door,
    SecretDoor,
}

public static class WallTypeExtensions
{
    public static WallType Next(this WallType current) => current switch
    {
        Solid => Door,
        Door => SecretDoor,
        SecretDoor => Solid,
        _ => throw new Exception($"Unknown wall type {current}"),
    };

    public static bool IsPassable(this WallType wall) => wall is not Solid;
}