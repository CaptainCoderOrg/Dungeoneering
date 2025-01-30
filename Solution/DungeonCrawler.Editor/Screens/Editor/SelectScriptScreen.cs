namespace CaptainCoder.Dungeoneering.Raylib;

using CaptainCoder.Dungeoneering.Editor;

using Raylib_cs;

public class SelectScriptScreen(SelectTriggerScreen triggerScreen, EventTrigger trigger) : IScreen
{
    public SelectTriggerScreen TriggerScreen { get; } = triggerScreen;
    public EventTrigger Trigger { get; } = trigger;
    private MenuScreen? _menuScreen;
    public MenuScreen MenuScreen => _menuScreen ??= InitMenu();
    private MenuScreen InitMenu()
    {
        MenuScreen screen = new($"Trigger: {Trigger} - Select Script", [
            .. ScriptOptions,
            new StaticEntry("Back", () => Program.Screen = TriggerScreen),
            new StaticEntry("Cancel", () => Program.Screen = TriggerScreen.PreviousScreen),
        ]);
        return screen;
    }
    public IEnumerable<MenuEntry> ScriptOptions => Project.DefaultFileSystem.GetScriptNames(Path.Combine(EditorConstants.SaveDir, TriggerScreen.ProjectName)).Select(name => new StaticEntry(name, () => AddEvent(name)));
    public void AddEvent(string scriptName)
    {
        TriggerScreen.Map.AddEvent(TriggerScreen.Position, new TileEvent(Trigger, scriptName));
        Program.Screen = TriggerScreen.PreviousScreen;
    }
    public void HandleUserInput()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Escape))
        {
            Program.Screen = TriggerScreen;
        }
        MenuScreen.HandleUserInput();
    }
    public void Render()
    {
        Raylib.ClearBackground(Color.Black);
        MenuScreen.Render();
    }
}