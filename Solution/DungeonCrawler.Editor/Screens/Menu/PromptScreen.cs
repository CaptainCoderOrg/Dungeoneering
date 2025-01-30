namespace CaptainCoder.Dungeoneering.Raylib;

using System.Text;


using Raylib_cs;

public class PromptScreen(string prompt, IScreen previousScreen, Action<string> onFinish, Action? onCancel = null) : IScreen
{
    public string Prompt { get; } = prompt;
    public Action<string> OnFinish { get; } = onFinish;
    public Action? OnCancel { get; } = onCancel;
    public string UserInput { get; private set; } = string.Empty;
    private readonly StringBuilder _builder = new();
    public IScreen PreviousScreen { get; } = previousScreen;
    public bool RenderPreviousScreen { get; set; } = true;

    public void HandleUserInput()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Escape))
        {
            Program.Screen = PreviousScreen;
            OnCancel?.Invoke();
            return;
        }
        if (Raylib.IsKeyPressed(KeyboardKey.Backspace) && _builder.Length > 0)
        {
            _builder.Remove(_builder.Length - 1, 1);
            return;
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Enter) && _builder.Length > 0)
        {
            Program.Screen = PreviousScreen;
            OnFinish.Invoke(_builder.ToString());
            return;
        }

        char key;
        do
        {
            key = (char)Raylib.GetCharPressed();
            if (char.IsAsciiLetterOrDigit(key))
            {
                _builder.Append(key);
            }
        } while (key != 0);

    }

    public void Render()
    {
        if (RenderPreviousScreen) { PreviousScreen.Render(); }
        else { Raylib.ClearBackground(Color.Black); }
        Prompt.DrawCentered(50, 48, Color.White);
        string userInput = _builder.ToString();
        userInput = userInput != string.Empty ? userInput : "Enter a Name";
        if (userInput == string.Empty)
        {
            "Enter a Response".DrawCentered(100, 24, Color.Yellow);
        }
        else
        {
            userInput.ToString().DrawCentered(100, 36, Color.White);
        }
    }
}