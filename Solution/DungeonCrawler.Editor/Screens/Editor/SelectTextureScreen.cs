using CaptainCoder.Dungeoneering.Editor;


namespace CaptainCoder.Dungeoneering.Raylib;
using Raylib_cs;
public class SelectTextureScreen : IScreen
{
    public required IScreen PreviousScreen { get; init; }
    public required string ProjectName { get; init; }
    public Action<TextureResult>? OnFinished { get; init; }
    private MenuScreen? _menu;
    public MenuScreen Menu => _menu ??= InitMenu();

    public void HandleUserInput()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Escape)) { Finish(); }
        Menu.HandleUserInput();
    }

    public void Render()
    {
        Raylib.ClearBackground(Color.Black);
        Menu.Render();
    }

    private MenuScreen InitMenu()
    {
        IEnumerable<MenuEntry> options = [
            new StaticEntry("Default", () => { Finish(DefaultTexture.Shared); }),
            .. Project.DefaultFileSystem.GetTextureNames(Path.Combine(EditorConstants.SaveDir, ProjectName)).Select(MakeEntry),
        ];
        return new MenuScreen("Select Texture", options);
        MenuEntry MakeEntry(string name) => new StaticEntry(name, () => { Finish(new TextureReference(name)); });
    }

    private void Finish(TextureResult? result = null)
    {
        Program.Screen = PreviousScreen;
        if (result is not null)
        {
            OnFinished?.Invoke(result);
        }
    }
}

public abstract record TextureResult;
public record DefaultTexture : TextureResult
{
    public static DefaultTexture Shared { get; } = new DefaultTexture();
}
public record TextureReference(string Name) : TextureResult;