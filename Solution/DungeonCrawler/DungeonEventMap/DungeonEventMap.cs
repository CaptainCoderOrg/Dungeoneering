using System.Diagnostics.CodeAnalysis;

using CaptainCoder.Utils.DictionaryExtensions;

namespace CaptainCoder.Dungeoneering.DungeonMap;

public class EventMap() : IEquatable<EventMap>
{
    public Dictionary<Position, List<TileEvent>> Events { get; set; } = [];

    public EventMap(IEnumerable<(Position, IEnumerable<TileEvent>)> events) : this()
    {
        Events = new();
        foreach ((Position pos, IEnumerable<TileEvent> eventsAtPosition) in events)
        {
            Events[pos] = [.. eventsAtPosition];
        }
    }
    public void AddEvent(Position position, TileEvent toAdd)
    {
        if (!Events.TryGetValue(position, out List<TileEvent> tileEvents))
        {
            tileEvents = new();
            Events[position] = tileEvents;
        }
        tileEvents.Add(toAdd);
    }

    public bool TryRemoveEvent(Position position, int ix, [NotNullWhen(true)] out TileEvent? removed)
    {
        if (Events.TryGetValue(position, out List<TileEvent> eventsAtPosition) && eventsAtPosition.Count > ix)
        {
            removed = eventsAtPosition[ix];
            eventsAtPosition.RemoveAt(ix);
            if (eventsAtPosition.Count == 0)
            {
                Events.Remove(position);
            }
            return true;
        }
        removed = default;
        return false;
    }
    public void RemoveAllEventsAt(Position position) => Events.Remove(position);

    public List<TileEvent> EventsAt(Position position) => Events.GetValueOrDefault(position, []);

    public bool Equals(EventMap other)
    {
        return Events.AllKeyValuesAreEqual(other.Events, ListEquality);
        static bool ListEquality(List<TileEvent> list0, List<TileEvent> list1)
        {
            if (list0.Count != list1.Count) { return false; }
            return list0.ToHashSet().SetEquals(list1);
        }
    }
}