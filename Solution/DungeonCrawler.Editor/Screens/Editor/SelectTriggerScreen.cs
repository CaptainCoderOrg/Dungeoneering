namespace CaptainCoder.Dungeoneering.Raylib;

using Raylib_cs;

public class SelectTriggerScreen(IScreen previousScreen, string projectName, Position position, EventMap map) : IScreen
{
    public IScreen PreviousScreen { get; } = previousScreen;
    public string ProjectName { get; } = projectName;
    public Position Position { get; } = position;
    public EventMap Map { get; } = map;
    private MenuScreen? _menuScreen;
    public MenuScreen MenuScreen => _menuScreen ??= InitMenu();
    private MenuScreen InitMenu()
    {
        MenuScreen screen = new($"Select Trigger", [
            .. TriggerOptions,
            new StaticEntry("Cancel", () => Program.Screen = PreviousScreen),
        ]);
        return screen;
    }
    public IEnumerable<MenuEntry> TriggerOptions => Enum.GetValues<EventTrigger>().Select(trigger => new StaticEntry(trigger.ToString(), () => Program.Screen = new SelectScriptScreen(this, trigger)));
    public void HandleUserInput()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Escape))
        {
            Program.Screen = PreviousScreen;
        }
        MenuScreen.HandleUserInput();
    }
    public void Render()
    {
        Raylib.ClearBackground(Color.Black);
        MenuScreen.Render();
    }
}