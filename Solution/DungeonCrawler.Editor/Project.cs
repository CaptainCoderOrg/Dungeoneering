using System.IO.Abstractions;

using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonCrawler.Scripting;
using CaptainCoder.Dungeoneering.DungeonMap.IO;

namespace CaptainCoder.Dungeoneering.Editor;

public static class Project
{
    public static IFileSystem DefaultFileSystem = new FileSystem();
    // private readonly IFileSystem _fileSystem = fileSystem;
    public const string ScriptDir = "scripts";
    public const string DungeonDir = "dungeons";
    public const string TextureDir = "textures";
    // public string RootDirectory { get; set; } = rootDirectory;
    /// <summary>
    /// A dictionary from dungeon names to dungeon file paths
    /// </summary>
    public static IDictionary<string, string> GetDungeonPaths(this IFileSystem fileSystem, string root)
    {
        return fileSystem.Directory.EnumerateFiles(Path.Combine(root, DungeonDir)).ToDictionary(GetName, path => path);
        static string GetName(string path) => Path.GetFileNameWithoutExtension(path);

    }
    /// <summary>
    /// A collection of all of the names of all of the scripts in this project
    /// </summary>
    public static IEnumerable<string> GetScriptNames(this IFileSystem fileSystem, string root)
    {
        return fileSystem.Directory.EnumerateFiles(Path.Combine(root, ScriptDir)).Select(GetName);
        static string GetName(string path) => Path.GetFileName(path);
    }

    public static IEnumerable<string> GetTextureNames(this IFileSystem fileSystem, string root)
    {
        return fileSystem.Directory.EnumerateFiles(Path.Combine(root, TextureDir)).Select(GetName);
        static string GetName(string path) => Path.GetFileName(path);
    }

    /// <summary>
    /// Compile all of the resources of this project into a single DungeonCrawlerManifest
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static DungeonCrawlerManifest Build(this IFileSystem fileSystem, string root)
    {
        DungeonCrawlerManifest manifest = new();

        foreach ((string name, string filepath) in fileSystem.GetDungeonPaths(root))
        {
            Dungeon dungeon = JsonExtensions.LoadModel<Dungeon>(fileSystem.File.ReadAllText(filepath));
            manifest.AddDungeon(name, dungeon);
        }

        foreach (string name in fileSystem.GetScriptNames(root))
        {
            string scriptText = fileSystem.File.ReadAllText(Path.Combine(root, ScriptDir, name));
            manifest.AddScript(name, new EventScript(scriptText));
        }

        foreach (string fileName in fileSystem.GetTextureNames(root))
        {
            byte[] bytes = fileSystem.File.ReadAllBytes(Path.Combine(root, TextureDir, fileName));
            string textureName = Path.GetFileName(fileName);
            manifest.AddTexture(new Texture(textureName, bytes));
        }

        return manifest;
    }
    public static bool IsProjectDirectory(this IFileSystem fileSystem, string rootPath)
    {
        return fileSystem.Directory.Exists(rootPath) &&
               fileSystem.Directory.Exists(Path.Combine(rootPath, DungeonDir)) &&
               fileSystem.Directory.Exists(Path.Combine(rootPath, ScriptDir)) &&
               fileSystem.Directory.Exists(Path.Combine(rootPath, TextureDir));
    }

    public static void InitializeProjectDirectory(this IFileSystem fileSystem, string rootPath)
    {
        if (fileSystem.Directory.Exists(rootPath)) { throw new IOException($"Cannot initialize project at '{rootPath}'. Path already exists."); }
        fileSystem.Directory.CreateDirectory(rootPath);
        fileSystem.Directory.CreateDirectory(Path.Combine(rootPath, DungeonDir));
        fileSystem.Directory.CreateDirectory(Path.Combine(rootPath, ScriptDir));
        fileSystem.Directory.CreateDirectory(Path.Combine(rootPath, TextureDir));
    }
}