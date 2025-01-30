namespace CaptainCoder.Dungeoneering.Raylib;

using System.IO.Abstractions;

using CaptainCoder.Dungeoneering.Editor;

using Raylib_cs;

public class ProjectSelectionScreen : IScreen
{
    private readonly MenuScreen _menuScreen;

    public ProjectSelectionScreen()
    {
        _menuScreen = ProjectOptions();
    }
    private MenuScreen ProjectOptions()
    {
        MenuScreen screen = new("Select a Project", [
            .. CreateProjectEntries(),
            new StaticEntry("New Project", CreateNewProject),
        ]);
        return screen;
    }
    private static IEnumerable<MenuEntry> CreateProjectEntries()
    {
        return GetProjectList().Select(path => new StaticEntry(
            Path.GetFileName(path), () => { Program.Screen = new ProjectScreen(Path.GetFileName(path)); }
        ));
    }
    public static List<string> GetProjectList()
    {
        IFileSystem fs = Project.DefaultFileSystem;
        return [.. fs.Directory.EnumerateDirectories(EditorConstants.SaveDir).Where(fs.IsProjectDirectory)];
    }
    public void HandleUserInput()
    {
        _menuScreen.HandleUserInput();
    }

    public void Render()
    {
        Raylib.ClearBackground(Color.Black);
        _menuScreen.Render();
    }

    public void CreateNewProject()
    {
        Program.Screen = new PromptScreen("Name Project", this, OnFinished) { RenderPreviousScreen = false };
        static void OnFinished(string projectName)
        {
            var fs = Project.DefaultFileSystem;
            string path = Path.Combine(EditorConstants.SaveDir, projectName);
            try
            {
                fs.InitializeProjectDirectory(path);
                Program.Screen = new ProjectScreen(projectName);
            }
            catch (IOException e)
            {
                Console.Error.WriteLine(e.StackTrace);
                Console.Error.WriteLine($"Could not initialize project: {e.Message}");
            }
        }
    }
}