namespace CaptainCoder.Dungeoneering.DungeonMap.IO;

using Newtonsoft.Json;

/// <summary>
/// JsonConverter for DungeonCrawlerManifest. This is required to correctly serialize and
/// deserialize the underlying dictionary which uses non-string as a Key. The
/// default serializer for Dictionary converts the keys to a string which cannot
/// be correctly deserialized.
/// </summary>
public class DictionaryJsonConverter<TKey, TValue> : JsonConverter<Dictionary<TKey, TValue>>
{
    public override Dictionary<TKey, TValue>? ReadJson(JsonReader reader, Type objectType, Dictionary<TKey, TValue>? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        (TKey, TValue)[] elems = serializer.Deserialize<(TKey, TValue)[]>(reader) ?? throw new Exception($"Could not parse json to Dictionary<{typeof(TKey)}, {typeof(TValue)}.");
        return new Dictionary<TKey, TValue>(elems.ToKevValuePairs());
    }

    public override void WriteJson(JsonWriter writer, Dictionary<TKey, TValue>? value, JsonSerializer serializer)
    {
        if (value is null) { throw new Exception($"Could not write null to dictionary."); }
        serializer.Serialize(writer, value.ToSerializableArray());
    }
}

public static class DictionaryExtensions
{
    public static (TKey, TValue)[] ToSerializableArray<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> toSerialize) =>
        [.. toSerialize.Select(kvp => (kvp.Key, kvp.Value))];

    public static IEnumerable<KeyValuePair<TKey, TValue>> ToKevValuePairs<TKey, TValue>(this (TKey, TValue)[] toDeserialize) =>
        toDeserialize.Select(pair => new KeyValuePair<TKey, TValue>(pair.Item1, pair.Item2));
}