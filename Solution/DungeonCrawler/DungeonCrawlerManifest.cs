using CaptainCoder.Dungeoneering.DungeonCrawler.Scripting;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Utils.DictionaryExtensions;

namespace CaptainCoder.Dungeoneering.DungeonCrawler;

public class DungeonCrawlerManifest : IEquatable<DungeonCrawlerManifest>
{
    public Dictionary<string, Dungeon> Dungeons { get; set; } = new();
    public Dictionary<string, EventScript> Scripts { get; set; } = new();
    public Dictionary<string, Texture> Textures { get; set; } = new();

    public bool Equals(DungeonCrawlerManifest other)
    {
        return Dungeons.AllKeyValuesAreEqual(other.Dungeons, DungeonEquality) &&
               Scripts.AllKeyValuesAreEqual(other.Scripts) &&
               Textures.AllKeyValuesAreEqual(other.Textures, TextureEquality);
        static bool TextureEquality(Texture t0, Texture t1) => t0.Equals(t1);
        static bool DungeonEquality(Dungeon d0, Dungeon d1) => d0.Equals(d1);
    }
}

public static class ManifestExtensions
{
    public static void AddDungeon(this DungeonCrawlerManifest manifest, string name, Dungeon toAdd)
    {
        if (!manifest.Dungeons.TryAdd(name, toAdd))
        {
            throw new InvalidOperationException($"A dungeon with the name '{name}' already exists.");
        }
    }

    public static void AddScript(this DungeonCrawlerManifest manifest, string name, EventScript toAdd)
    {
        if (!manifest.Scripts.TryAdd(name, toAdd))
        {
            throw new InvalidOperationException($"A script with the name '{name}' already exists.");
        }
    }

    public static void AddTexture(this DungeonCrawlerManifest manifest, Texture toAdd)
    {
        if (!manifest.Textures.TryAdd(toAdd.Name, toAdd))
        {
            throw new InvalidOperationException($"A texture with the name '{toAdd.Name}' already exists.");
        }
    }
}