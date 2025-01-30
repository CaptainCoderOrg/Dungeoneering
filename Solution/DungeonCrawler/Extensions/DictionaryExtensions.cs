namespace CaptainCoder.Utils.DictionaryExtensions;

public static class DictionaryExtensions
{
    public static bool AllKeyValuesAreEqual<TKey, TValue>(this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second, Func<TValue, TValue, bool>? equals = null)
    {
        equals ??= (v, o) => v == null || v.Equals(o);
        if (first.Count != second.Count) { return false; }
        foreach ((TKey key, TValue value) in first)
        {
            if (!second.TryGetValue(key, out TValue otherValue)) { return false; }
            if (!equals.Invoke(value, otherValue)) { return false; }
        }
        return true;
    }
}