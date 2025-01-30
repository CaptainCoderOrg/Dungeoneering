namespace CaptainCoder.Dungeoneering.Raylib;

using System.Text.Json;

using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap.IO;
using CaptainCoder.Dungeoneering.Editor;

using CommandLine;

using Raylib_cs;

public class Program
{
    public const string ConfigFile = "dungeoneering.config";
    public static IScreen Screen { get; set; } = new ProjectSelectionScreen();
    public static ScreenConfig Config { get; set; } = InitScreenConfig();
    private static bool s_isRunning = true;

    public class Options
    {
        [Option('o', "out", Required = false, HelpText = "The output file when building")]
        public string? OutPath { get; set; } = null;
        [Option('b', "build", Required = false, HelpText = "Build the specified project and exit")]
        public string? BuildProject { get; set; } = null;
    }

    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(o =>
        {
            if (o.BuildProject is null)
            {
                RunEditor();
            }
            else
            {
                Build(o.BuildProject, o.OutPath);
            }
        });
    }

    public static void Build(string projectName, string? outDir)
    {
        outDir ??= Path.Combine(EditorConstants.SaveDir, $"{projectName}.json");
        Console.WriteLine($"Building project {projectName}");
        DungeonCrawlerManifest manifest = Project.DefaultFileSystem.Build(Path.Combine(EditorConstants.SaveDir, projectName));
        Console.WriteLine($"Success! Writing manifest to {outDir}");
        File.WriteAllText(outDir, manifest.ToJson());
    }

    public static void RunEditor()
    {
        InitWindow();
        Raylib.InitAudioDevice();

        Raylib.SetTargetFPS(60);
        Raylib.SetExitKey(0);
        // Main game loop
        while (!Raylib.WindowShouldClose() && s_isRunning)
        {
            Screen.HandleUserInput();
            Raylib.BeginDrawing();
            Screen.Render();
            Raylib.EndDrawing();
        }
        SaveScreenConfig(new ScreenConfig(Raylib.GetCurrentMonitor()));
        Raylib.CloseWindow();
    }

    public static void Exit() => s_isRunning = false;

    private static void SaveScreenConfig(ScreenConfig config)
    {
        try
        {
            string json = JsonSerializer.Serialize(config);
            File.WriteAllText(ConfigFile, json);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Failed to save {ConfigFile}. Stack Trace below:");
            Console.Error.WriteLine(e.StackTrace);
        }
    }

    private static ScreenConfig InitScreenConfig()
    {
        if (File.Exists(ConfigFile))
        {
            try
            {
                string json = File.ReadAllText(ConfigFile);
                return JsonSerializer.Deserialize<ScreenConfig>(json);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Failed to load {ConfigFile}. Stack trace below:");
                Console.Error.WriteLine(e.StackTrace);
            }
        }
        return EditorConstants.Default;
    }

    /// <summary>
    /// Initializes the game window on to the specified monitor and centers
    /// it on the screen
    /// </summary>
    private static void InitWindow()
    {
        Raylib.SetWindowState(ConfigFlags.HiddenWindow);
        Raylib.InitWindow(EditorConstants.MinScreenWidth, EditorConstants.MinScreenHeight, "Dungeon Editor | Main Window");
        Raylib.SetWindowState(ConfigFlags.ResizableWindow);
        Raylib.SetWindowMinSize(EditorConstants.MinScreenWidth, EditorConstants.MinScreenHeight);
        CenterWindow();

        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);
        Raylib.ClearWindowState(ConfigFlags.HiddenWindow);
        Raylib.EndDrawing();
    }

    private static void CenterWindow()
    {
        (int mWidth, int mHeight) = (Raylib.GetMonitorWidth(Config.Monitor), Raylib.GetMonitorHeight(Config.Monitor));
        (int wWidth, int wHeight) = (Raylib.GetScreenWidth(), Raylib.GetScreenHeight());

        Raylib.SetWindowPosition((mWidth - wWidth) / 2, (mHeight - wHeight) / 2);
        Raylib.SetWindowMonitor(Config.Monitor);
    }

}