namespace Tests;

using CaptainCoder.Dungeoneering.DungeonCrawler.Scripting;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.IO;

using Shouldly;

public class DungeonEventMap_should
{
    static readonly EventScript TeleportPlayerMock = new("Teleport Player");
    static readonly TileEvent TeleportEvent = new(EventTrigger.OnEnter, "teleport.lua");
    static readonly EventScript DamagePlayerMock = new("Damage Player Mock");
    static readonly TileEvent DamageEvent = new(EventTrigger.OnExit, "damage.lua");

    public static IEnumerable<object[]> AddEventsAtPositionData => [
        [new Position(5, 5), TeleportEvent],
        [new Position(2, 7), DamageEvent],
    ];

    [Theory]
    [MemberData(nameof(AddEventsAtPositionData))]
    public void add_event_at_position(Position position, TileEvent eventToAdd)
    {
        EventMap map = new();
        map.AddEvent(position, eventToAdd);
        map.EventsAt(position).Count.ShouldBe(1);
        map.EventsAt(position).ShouldContain(eventToAdd);
    }

    public static IEnumerable<object[]> AddMultipleEventsAtPositionData => [
        [new Position(5, 5), TeleportEvent, DamageEvent],
        [new Position(2, 7), DamageEvent, TeleportEvent],
    ];

    [Theory]
    [MemberData(nameof(AddMultipleEventsAtPositionData))]

    public void add_multiple_events_at_position(Position position, TileEvent firstEvent, TileEvent secondEvent)
    {
        EventMap map = new();
        map.AddEvent(position, firstEvent);
        map.AddEvent(position, secondEvent);
        map.EventsAt(position).Count.ShouldBe(2);
        map.EventsAt(position).ShouldContain(firstEvent);
        map.EventsAt(position).ShouldContain(secondEvent);
    }

    [Fact]
    public void not_have_key_when_last_event_is_removed_from_position()
    {
        EventMap map = new();
        map.AddEvent(new Position(0, 0), new TileEvent(EventTrigger.OnEnter, "scriptName"));
        map.TryRemoveEvent(new Position(0, 0), 0, out _);
        map.Events.Keys.Count.ShouldBe(0);
    }

    [Fact]
    public void provide_view_of_events()
    {
        EventMap map = new();
        Position pos = new(0, 0);
        map.AddEvent(pos, DamageEvent);

        IReadOnlyDictionary<Position, List<TileEvent>> events = map.Events;

        events[pos].Count.ShouldBe(1);
        events[pos].ShouldContain(DamageEvent);

        map.AddEvent(pos, TeleportEvent);

        events[pos].Count.ShouldBe(2);
        events[pos].ShouldContain(TeleportEvent);
    }

    [Fact]
    public void remove_event_by_index()
    {
        EventMap map = new();
        Position pos = new(0, 0);
        map.AddEvent(pos, DamageEvent);
        map.AddEvent(pos, TeleportEvent);
        List<TileEvent> events = map.EventsAt(pos);

        int ix = events.IndexOf(DamageEvent);
        bool wasSuccessful = map.TryRemoveEvent(pos, ix, out TileEvent? removed);

        wasSuccessful.ShouldBeTrue();
        removed.ShouldBe(DamageEvent);
        map.EventsAt(pos).Count.ShouldBe(1);
    }

    [Fact]
    public void remove_event_by_index_returns_false_on_no_entry()
    {
        EventMap map = new();
        bool actual = map.TryRemoveEvent(new Position(0, 0), 0, out _);
        actual.ShouldBeFalse();
    }

    [Fact]
    public void remove_all_events_at_position()
    {
        EventMap map = new();
        Position pos = new(0, 0);
        map.AddEvent(pos, DamageEvent);
        map.AddEvent(pos, TeleportEvent);

        map.RemoveAllEventsAt(pos);

        map.EventsAt(pos).Count.ShouldBe(0);
    }

    [Fact]
    public void be_jsonable()
    {
        EventMap map = new();
        Position pos0 = new(0, 0);
        Position pos1 = new(0, 0);
        map.AddEvent(pos0, new TileEvent(EventTrigger.OnEnter, "event0"));
        map.AddEvent(pos0, new TileEvent(EventTrigger.OnExit, "event1"));
        map.AddEvent(pos1, new TileEvent(EventTrigger.OnEnter, "event2"));

        string json = map.ToJson();
        EventMap restored = JsonExtensions.LoadModel<EventMap>(json);

        List<TileEvent> expectedEventsAtPos0 = map.EventsAt(pos0);
        restored.EventsAt(pos0).Count.ShouldBe(expectedEventsAtPos0.Count);
        restored.EventsAt(pos0).ShouldBeSubsetOf(expectedEventsAtPos0);

        List<TileEvent> expectedEventsAtPos1 = map.EventsAt(pos1);
        restored.EventsAt(pos1).Count.ShouldBe(expectedEventsAtPos1.Count);
        restored.EventsAt(pos1).ShouldBeSubsetOf(expectedEventsAtPos1);
    }

    [Fact]
    public void be_equals()
    {
        EventMap first = MakeMap();
        first.ShouldBe(MakeMap());

        static EventMap MakeMap()
        {
            EventMap map = new();
            Position pos0 = new(0, 0);
            Position pos1 = new(0, 0);
            map.AddEvent(pos0, new TileEvent(EventTrigger.OnEnter, "event0"));
            map.AddEvent(pos0, new TileEvent(EventTrigger.OnExit, "event1"));
            map.AddEvent(pos1, new TileEvent(EventTrigger.OnEnter, "event2"));
            return map;
        }
    }

}