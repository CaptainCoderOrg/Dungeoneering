namespace Tests;

using CaptainCoder.Dungeoneering.DungeonMap;

using Newtonsoft.Json;

using Shouldly;

public class TileEdge_should
{

    [Fact]
    public void not_be_equal_to_null()
    {
        TileEdge underTest = new(new Position(0, 0), Facing.North);
        underTest.Equals(null!).ShouldBeFalse();
    }

    public static IEnumerable<object[]> NormalizeTestData => [
        [new TileEdge(new Position(0, 0), Facing.North), new TileEdge(new Position(0, 0), Facing.North)],
        [new TileEdge(new Position(0, 0), Facing.South), new TileEdge(new Position(0, 1), Facing.North)],
        [new TileEdge(new Position(0, 0), Facing.East), new TileEdge(new Position(0, 0), Facing.East)],
        [new TileEdge(new Position(0, 0), Facing.West), new TileEdge(new Position(-1, 0), Facing.East)],
    ];

    [Theory]
    [MemberData(nameof(NormalizeTestData))]
    public void normalize(TileEdge underTest, TileEdge expected)
    {
        underTest.Normalize().ShouldBe(expected);
    }

    public static IEnumerable<object[]> BeEqualToNormalizedValue => [
        [new TileEdge(new Position(0, 0), Facing.North)],
        [new TileEdge(new Position(0, 0), Facing.South)],
        [new TileEdge(new Position(0, 0), Facing.East)],
        [new TileEdge(new Position(0, 0), Facing.West)],
    ];

    [Theory]
    [MemberData(nameof(BeEqualToNormalizedValue))]
    public void be_equal_to_normalized_value(TileEdge underTest)
    {
        TileEdge normalized = underTest.Normalize();
        Assert.Equal(underTest, normalized);
    }

    public static IEnumerable<object[]> BeEqualToSelf => [
        [new TileEdge(new Position(0, 0), Facing.North)],
        [new TileEdge(new Position(0, 0), Facing.South)],
        [new TileEdge(new Position(0, 0), Facing.East)],
        [new TileEdge(new Position(0, 0), Facing.West)],
    ];

    [Theory]
    [MemberData(nameof(BeEqualToSelf))]
    public void be_equal_to_self(TileEdge underTest)
    {
        underTest.ShouldBe(underTest);
    }

    public static IEnumerable<object[]> BeEqualSharedWall => [
        [new TileEdge(new Position(0, 0), Facing.North), new TileEdge(new Position(0, -1), Facing.South)],
        [new TileEdge(new Position(0, 0), Facing.South), new TileEdge(new Position(0, 1), Facing.North)],
        [new TileEdge(new Position(0, 0), Facing.East), new TileEdge(new Position(1, 0), Facing.West)],
        [new TileEdge(new Position(0, 0), Facing.West), new TileEdge(new Position(-1, 0), Facing.East)],
    ];

    [Theory]
    [MemberData(nameof(BeEqualSharedWall))]
    public void be_equal_to_shared_wall(TileEdge first, TileEdge second)
    {
        Assert.Equal(first, second);
    }

    public static IEnumerable<object[]> NotBeEqualWhenNotSharedEdge => [
        [new TileEdge(new Position(0, 0), Facing.North), new TileEdge(new Position(0, 0), Facing.South)],
        [new TileEdge(new Position(0, 0), Facing.North), new TileEdge(new Position(0, 0), Facing.West)],
        [new TileEdge(new Position(0, 0), Facing.North), new TileEdge(new Position(0, 0), Facing.East)],
        [new TileEdge(new Position(0, 0), Facing.North), new TileEdge(new Position(1, 1), Facing.North)],
        [new TileEdge(new Position(0, 0), Facing.East), new TileEdge(new Position(1, 1), Facing.East)],
        [new TileEdge(new Position(0, 0), Facing.West), new TileEdge(new Position(1, 1), Facing.West)],
        [new TileEdge(new Position(0, 0), Facing.South), new TileEdge(new Position(1, 1), Facing.South)],
    ];
    [Theory]
    [MemberData(nameof(NotBeEqualWhenNotSharedEdge))]
    public void not_be_equal_when_not_shared_edge(TileEdge first, TileEdge second)
    {
        first.ShouldNotBe(second);
    }

    [Theory]
    [InlineData(5, 7, Facing.North)]
    [InlineData(0, 0, Facing.East)]
    public void be_jsonable(int x, int y, Facing facing)
    {
        TileEdge underTest = new(new Position(x, y), facing);
        string json = JsonConvert.SerializeObject(underTest);
        TileEdge? actual = JsonConvert.DeserializeObject<TileEdge>(json);
        actual.ShouldBe(underTest);
    }

}