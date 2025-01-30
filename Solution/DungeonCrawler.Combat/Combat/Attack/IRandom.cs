namespace CaptainCoder.DungeonCrawler.Combat;
public interface IRandom
{
    public static IRandom Default { get; } = new DefaultRandom();
    public int Next(int min, int max);
}

internal class DefaultRandom : IRandom
{
    private static readonly Random RNG = new();
    public int Next(int min, int max) => RNG.Next(min, max);
}