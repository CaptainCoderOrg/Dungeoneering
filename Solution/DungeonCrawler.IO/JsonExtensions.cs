namespace CaptainCoder.Dungeoneering.DungeonMap.IO;

using CaptainCoder.Dungeoneering.DungeonCrawler;

using Newtonsoft.Json;

/// <summary>
/// JsonExtensions provides utility classes for serializing and deserializing
/// classes. Creating converter classes is necessary to serialize Dictionaries
/// that use non-string keys. 
/// </summary>
public static class JsonExtensions
{
    public static JsonConverter[] Converters { get; } = [
        new DictionaryJsonConverter<TileEdge, WallType>(),
        new DictionaryJsonConverter<Position, List<TileEvent>>(),
        new DictionaryJsonConverter<(Position, Facing), string>(),
        new DictionaryJsonConverter<Position, string>(),
    ];
    public static string ToJson(this WallMap wallMap) => JsonConvert.SerializeObject(wallMap, Converters);
    public static string ToJson(this EventMap eventMap) => JsonConvert.SerializeObject(eventMap, Converters);
    public static string ToJson(this DungeonCrawlerManifest crawler) => JsonConvert.SerializeObject(crawler, Converters);
    public static string ToJson<T>(T data) => JsonConvert.SerializeObject(data, Converters);

    public static T LoadModel<T>(string json)
    {
        T result = JsonConvert.DeserializeObject<T>(json, Converters) ?? throw new JsonException($"Could not parse DungeonEventMap from \"{json}\"");
        return result;
    }
}