namespace Tests;

using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.IO;

using Shouldly;

public class WallMap_should
{
    [Fact]
    public void not_update_count_when_setting_matching_shared_wall()
    {
        WallMap underTest = new();
        underTest.SetWall(new Position(5, 5), Facing.North, WallType.Solid);
        underTest.SetWall(new Position(5, 4), Facing.South, WallType.Solid);

        underTest.Count.ShouldBe(1);
        underTest[new Position(5, 5), Facing.North].ShouldBe(WallType.Solid);
        underTest[new Position(5, 4), Facing.South].ShouldBe(WallType.Solid);
    }

    public static IEnumerable<object[]> SaveAsJsonAndRestorFromJsonData()
    {
        WallMap empty = new();
        WallMap oneWall = new();
        oneWall.SetWall(new Position(2, 5), Facing.North, WallType.Solid);

        WallMap threeWall = new();
        threeWall.SetWall(new Position(3, 3), Facing.South, WallType.Door);
        threeWall.SetWall(new Position(3, 3), Facing.East, WallType.Solid);
        threeWall.SetWall(new Position(3, 3), Facing.West, WallType.SecretDoor);
        return [
            [empty],
            [oneWall],
            [threeWall]
        ];
    }

    [Theory]
    [MemberData(nameof(SaveAsJsonAndRestorFromJsonData))]
    public void save_as_json_and_restore_from_json(WallMap underTest)
    {
        string json = underTest.ToJson();
        WallMap actual = JsonExtensions.LoadModel<WallMap>(json);
        underTest.Map.Count.ShouldBe(actual.Map.Count);
        underTest.Map.Keys.ShouldBeSubsetOf(actual.Map.Keys);
        foreach (var key in underTest.Map.Keys)
        {
            underTest.Map[key].ShouldBe(actual.Map[key]);
        }
    }

    [Theory]
    [InlineData(5, 5, Facing.North, WallType.Solid)]
    [InlineData(7, 4, Facing.East, WallType.Solid)]
    [InlineData(3, 8, Facing.South, WallType.Solid)]
    [InlineData(14, 17, Facing.West, WallType.Solid)]
    public void notify_observer_on_set_wall(int x, int y, Facing facing, WallType wall)
    {
        WallMap underTest = new();
        List<(Position, Facing, WallType)> notifications = new();
        underTest.OnWallChanged += (p, f, w) => notifications.Add((p, f, w));
        underTest.SetWall(new Position(x, y), facing, wall);

        notifications.Count.ShouldBe(2);
        notifications.ShouldContain((new Position(x, y), facing, wall));
        notifications.ShouldContain((new Position(x, y).Step(facing), facing.Opposite(), wall));
    }

    [Theory]
    [InlineData(5, 5, Facing.North)]
    [InlineData(7, 4, Facing.East)]
    [InlineData(3, 8, Facing.South)]
    [InlineData(14, 17, Facing.West)]
    public void notify_observer_on_wall_removed(int x, int y, Facing facing)
    {
        WallMap underTest = new();
        List<(Position, Facing, WallType)> notifications = new();
        underTest.OnWallChanged += (p, f, w) => notifications.Add((p, f, w));
        underTest.RemoveWall(new Position(x, y), facing);

        notifications.Count.ShouldBe(2);
        notifications.ShouldContain((new Position(x, y), facing, WallType.None));
        notifications.ShouldContain((new Position(x, y).Step(facing), facing.Opposite(), WallType.None));
    }

    [Fact]
    public void be_equals()
    {
        WallMap first = MakeMap();
        first.ShouldBe(MakeMap());

        static WallMap MakeMap()
        {
            WallMap map = new();
            map.SetWall(new Position(5, 7), Facing.East, WallType.Solid);
            map.SetWall(new Position(5, 2), Facing.South, WallType.Door);
            map.SetWall(new Position(2, 7), Facing.North, WallType.SecretDoor);
            return map;
        }
    }
}