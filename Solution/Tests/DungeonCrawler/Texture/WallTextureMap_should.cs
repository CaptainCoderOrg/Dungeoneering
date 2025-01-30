namespace Tests;

using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.IO;

using Shouldly;

public class WallTextureMap_should
{
    public static WallTextureMap NonDefaults => new()
    {
        DefaultDoor = "wood-door.png",
        DefaultSolid = "stone-wall.png",
        DefaultSecretDoor = "stone-wall-variant.png",
    };

    public static WallTextureMap MakeWithTextures() => new()
    {
        Textures = new Dictionary<(Position, Facing), string>{
            { (new Position(5, 7), Facing.North), "stone-wall.png" },
            { (new Position(5, 6), Facing.South), "stone-wall-variant.png" },
            { (new Position(3, 3), Facing.East), "tavern-wall.png" },
            { (new Position(0, 0), Facing.West), "well-wall.png" },
        },
    };

    public static IEnumerable<object[]> BeJsonableData => [
        [new WallTextureMap()],
        [NonDefaults],
        [MakeWithTextures()],
    ];

    [Theory]
    [MemberData(nameof(BeJsonableData))]
    public void be_jsonable(WallTextureMap underTest)
    {
        string json = JsonExtensions.ToJson(underTest);
        WallTextureMap restored = JsonExtensions.LoadModel<WallTextureMap>(json);
        restored.ShouldBe(underTest);
    }

    [Fact]
    public void be_equal()
    {
        MakeWithTextures().ShouldBe(MakeWithTextures());
    }

    [Fact]
    public void be_not_equal()
    {
        NonDefaults.ShouldNotBe(new WallTextureMap());
    }
}