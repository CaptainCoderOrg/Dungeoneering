namespace Tests;

using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.IO;

using Shouldly;

public class Dungeon_should
{
    public static Dungeon SimpleSquareDungeon
    {
        get
        {
            Dungeon dungeon = new() { Name = "Simple" };
            dungeon.Walls.SetWall(new Position(0, 0), Facing.North, WallType.Solid);
            dungeon.Walls.SetWall(new Position(0, 0), Facing.South, WallType.Solid);
            dungeon.Walls.SetWall(new Position(0, 0), Facing.East, WallType.Solid);
            dungeon.Walls.SetWall(new Position(0, 0), Facing.West, WallType.Solid);
            return dungeon;
        }
    }

    public static Dungeon TwoByTwoRoom
    {
        get
        {
            Dungeon dungeon = new();
            dungeon.Walls.SetWall(new Position(0, 0), Facing.North, WallType.Solid);
            dungeon.Walls.SetWall(new Position(0, 0), Facing.West, WallType.Solid);

            dungeon.Walls.SetWall(new Position(1, 0), Facing.North, WallType.Solid);
            dungeon.Walls.SetWall(new Position(1, 0), Facing.East, WallType.Solid);

            dungeon.Walls.SetWall(new Position(1, 1), Facing.South, WallType.Solid);
            dungeon.Walls.SetWall(new Position(1, 1), Facing.East, WallType.Solid);

            dungeon.Walls.SetWall(new Position(0, 1), Facing.South, WallType.Solid);
            dungeon.Walls.SetWall(new Position(0, 1), Facing.West, WallType.Solid);
            return dungeon;
        }
    }

    public static IEnumerable<object[]> ReturnTilesWithWallsData => [
        [SimpleSquareDungeon, new Position(0, 0), new Tile(new Position(0, 0), new TileWalls(WallType.Solid, WallType.Solid, WallType.Solid, WallType.Solid))],
        [TwoByTwoRoom, new Position(0, 0), new Tile(new Position(0, 0), new TileWalls(WallType.Solid, WallType.None, WallType.None, WallType.Solid))],
        [TwoByTwoRoom, new Position(1, 0), new Tile(new Position(1, 0), new TileWalls(WallType.Solid, WallType.Solid, WallType.None, WallType.None))],
        [TwoByTwoRoom, new Position(1, 1), new Tile(new Position(1, 1), new TileWalls(WallType.None, WallType.Solid, WallType.Solid, WallType.None))],
        [TwoByTwoRoom, new Position(0, 1), new Tile(new Position(0, 1), new TileWalls(WallType.None, WallType.None, WallType.Solid, WallType.Solid))],
    ];

    [Theory]
    [MemberData(nameof(ReturnTilesWithWallsData))]
    public void return_tiles_with_walls(Dungeon dungeon, Position toCheck, Tile expectedTile)
    {
        Tile actual = dungeon.GetTile(toCheck);
        actual.ShouldBe(expectedTile);
    }

    [Fact]
    public void be_equals()
    {
        Dungeon first = MakeDungeon();
        first.ShouldBe(MakeDungeon());
        static Dungeon MakeDungeon()
        {
            Dungeon dungeon = new();
            dungeon.Walls.SetWall(new Position(5, 7), Facing.East, WallType.Door);
            dungeon.Walls.SetWall(new Position(5, 7), Facing.North, WallType.Solid);
            dungeon.Walls.SetWall(new Position(3, 7), Facing.North, WallType.Solid);
            dungeon.EventMap.AddEvent(new Position(3, 2), new TileEvent(EventTrigger.OnEnter, "Test script"));
            dungeon.EventMap.AddEvent(new Position(1, 2), new TileEvent(EventTrigger.OnExit, "Another Test script"));
            dungeon.TileTextures.Textures[new Position(0, 0)] = "dirt";
            return dungeon;
        }
    }

    [Fact]
    public void not_be_equals_with_different_tile_textures()
    {
        Dungeon first = MakeDungeon();
        Dungeon modified = MakeDungeon();
        modified.TileTextures.Textures[new Position(0, 0)] = "floor";
        first.ShouldNotBe(modified);
        static Dungeon MakeDungeon()
        {
            Dungeon dungeon = new();
            dungeon.Walls.SetWall(new Position(5, 7), Facing.East, WallType.Door);
            dungeon.Walls.SetWall(new Position(5, 7), Facing.North, WallType.Solid);
            dungeon.Walls.SetWall(new Position(3, 7), Facing.North, WallType.Solid);
            dungeon.EventMap.AddEvent(new Position(3, 2), new TileEvent(EventTrigger.OnEnter, "Test script"));
            dungeon.EventMap.AddEvent(new Position(1, 2), new TileEvent(EventTrigger.OnExit, "Another Test script"));
            return dungeon;
        }
    }

    [Theory]
    [InlineData(WallType.Solid, WallTextureMap.DefaultSolidTexture)]
    [InlineData(WallType.SecretDoor, WallTextureMap.DefaultSecretDoorTexture)]
    [InlineData(WallType.Door, WallTextureMap.DefaultDoorTexture)]
    public void get_default_wall_texture(WallType type, string expected)
    {
        Dungeon dungeon = new()
        {
            Walls = new WallMap([(new TileEdge(new Position(0, 0), Facing.North), type)]),
        };
        string actual = dungeon.GetWallTexture(new Position(0, 0), Facing.North);
        actual.ShouldBe(expected);
    }

    [Theory]
    [InlineData(WallType.Solid, "wall-variant.png")]
    [InlineData(WallType.SecretDoor, "wall-door-variant.png")]
    [InlineData(WallType.Door, "door-variant.png")]
    public void get_wall_texture(WallType type, string expected)
    {
        Dungeon dungeon = new()
        {
            Walls = new WallMap([(new TileEdge(new Position(0, 0), Facing.North), type)]),
            WallTextures = new WallTextureMap()
            {
                Textures = new Dictionary<(Position, Facing), string>()
                {
                    {(new Position(0, 0), Facing.North), expected},
                },
            }
        };
        string actual = dungeon.GetWallTexture(new Position(0, 0), Facing.North);
        actual.ShouldBe(expected);
    }

    [Theory]
    [InlineData(5, 7, Facing.North, "wood.png")]
    [InlineData(0, 17, Facing.East, "brick.png")]
    [InlineData(3, 1, Facing.South, "stone.png")]
    [InlineData(9, 2, Facing.West, "dirt.png")]
    public void set_wall_texture(int x, int y, Facing facing, string textureName)
    {
        Dungeon dungeon = new();
        dungeon.SetTexture(new Position(x, y), facing, textureName);

        string actual = dungeon.WallTextures.Textures[(new Position(x, y), facing)];
        actual.ShouldBe(textureName);
    }

    [Theory]
    [InlineData(5, 7, Facing.North, "wood.png")]
    [InlineData(0, 17, Facing.East, "brick.png")]
    [InlineData(3, 1, Facing.South, "stone.png")]
    [InlineData(9, 2, Facing.West, "dirt.png")]
    public void notify_observer_on_wall_texture_change(int x, int y, Facing facing, string textureName)
    {
        // Arrange
        Dungeon dungeon = new();
        (Position pos, Facing facing, string texture)? actual = null;
        dungeon.WallTextures.OnTextureChange += (p, f, t) => actual = (p, f, t);

        // Act
        dungeon.SetTexture(new Position(x, y), facing, textureName);

        // Assert
        actual.ShouldBe((new Position(x, y), facing, textureName));
    }

    [Fact]
    public void be_jsonable()
    {
        string json = JsonExtensions.ToJson(SimpleSquareDungeon);
        Dungeon loaded = JsonExtensions.LoadModel<Dungeon>(json);
        loaded.ShouldBe(SimpleSquareDungeon);
    }
}