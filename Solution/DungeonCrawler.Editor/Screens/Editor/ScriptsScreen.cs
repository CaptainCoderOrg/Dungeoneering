namespace CaptainCoder.Dungeoneering.Raylib;
using Raylib_cs;

public class ScriptsScreen(IScreen previousScreen, string projectName, Position position, EventMap map) : IScreen
{
    public IScreen PreviousScreen { get; } = previousScreen;
    public string ProjectName { get; } = projectName;
    public Position Position { get; } = position;
    public EventMap Map { get; } = map;
    private MenuScreen? _menuScreen;
    public MenuScreen MenuScreen => _menuScreen ??= InitMenu();
    private MenuScreen InitMenu()
    {
        MenuScreen screen = new($"Events at ({Position.X}, {Position.Y})", [
            new StaticEntry("New Event", () => Program.Screen = new SelectTriggerScreen(PreviousScreen, ProjectName, Position, Map)),
            .. CurrentEvents,
            new StaticEntry("Return to Editor", () => Program.Screen = PreviousScreen),
        ]);
        return screen;
    }
    public IEnumerable<MenuEntry> CurrentEvents => Map.EventsAt(Position).Select((evt, ix) => new StaticEntry($"Remove {evt.Trigger}: {evt.ScriptName}", () => Remove(ix)));
    public void Remove(int ix)
    {
        Map.TryRemoveEvent(Position, ix, out _);
        _menuScreen = InitMenu();
    }
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