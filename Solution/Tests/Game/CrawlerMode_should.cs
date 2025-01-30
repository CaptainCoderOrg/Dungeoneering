namespace Tests;

using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.Game;
using CaptainCoder.Dungeoneering.Player;

using Shouldly;

public class CrawlerMode_should
{
    [Theory]
    [InlineData(5, 5, Facing.East)]
    [InlineData(7, 2, Facing.West)]
    [InlineData(1, 9, Facing.North)]
    [InlineData(3, 18, Facing.South)]
    public void notify_observer_when_view_changes(int x, int y, Facing facing)
    {
        // Arrange
        CrawlerMode underTest = new(new DungeonCrawlerManifest(), new Dungeon(), new PlayerView(new Position(0, 0), Facing.North));
        ViewChangeEvent actual = null!;
        underTest.OnViewChange += (newView) => actual = newView;
        ViewChangeEvent expected = new(new PlayerView(new Position(0, 0), Facing.North), new(x, y, facing));

        // Act
        underTest.CurrentView = new PlayerView(x, y, facing);

        // Assert
        actual.ShouldBe(expected);
    }

    [Theory]
    [InlineData(MessageType.Info, "Information")]
    [InlineData(MessageType.Debug, "Debug Message")]
    public void notify_observer_on_message(MessageType type, string message)
    {
        CrawlerMode underTest = new(new DungeonCrawlerManifest(), new Dungeon(), new PlayerView(new Position(0, 0), Facing.North));
        Message actual = null!;
        underTest.OnMessageAdded += (message) => actual = message;
        Message expected = new(type, message);
        underTest.AddMessage(expected);
    }

    [Fact]
    public void notify_observer_on_dungeon_change()
    {
        Dungeon startDungeon = Dungeon_should.SimpleSquareDungeon;
        Dungeon nextDungeon = Dungeon_should.TwoByTwoRoom;
        DungeonCrawlerManifest manifest = new();
        CrawlerMode underTest = new(manifest, startDungeon, new PlayerView(0, 0, Facing.North));
        Dungeon? exited = default;
        Dungeon? entered = default;
        underTest.OnDungeonChange += evt => (exited, entered) = (evt.Exited, evt.Entered);

        underTest.CurrentDungeon = nextDungeon;

        exited.ShouldBe(startDungeon);
        entered.ShouldBe(nextDungeon);
    }
}