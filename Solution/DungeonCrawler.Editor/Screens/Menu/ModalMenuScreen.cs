namespace CaptainCoder.Dungeoneering.Raylib;

using Raylib_cs;

public class ModalMenuScreen(IScreen parent, MenuScreen menu) : IScreen
{
    public void HandleUserInput()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Escape))
        {
            Program.Screen = parent;
        }
        menu.HandleUserInput();
    }

    public void Render()
    {
        parent.Render();
        menu.Render();
    }
}