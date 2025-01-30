namespace CaptainCoder.Dungeoneering.Raylib;

using Raylib_cs;

public class MenuScreen(string title, IEnumerable<MenuEntry> items, int maxShown = 6) : IScreen
{
    const int SelectedFontSize = 32;
    const int NotSelectedFontSize = 20;
    const int Padding = 16;
    const int TitleFontSize = 40;
    public int MaxItemsShown { get; } = maxShown;
    private readonly MenuEntry[] _items = [.. items];
    private readonly string _title = title;
    private int _selectedIx = 0;
    public void HandleUserInput()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Enter))
        {
            _items[_selectedIx].OnSelect.Invoke();
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Down) || Raylib.IsKeyPressed(KeyboardKey.S))
        {
            UpdateSelected(1);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Up) || Raylib.IsKeyPressed(KeyboardKey.W))
        {
            UpdateSelected(-1);
        }
    }

    private void UpdateSelected(int amount)
    {
        _selectedIx = (_selectedIx + amount) switch
        {
            var ix when ix < 0 => _items.Length - 1,
            var ix when ix >= _items.Length => 0,
            var ix => ix,
        };
    }

    public void Render()
    {
        int menuHeight = NotSelectedFontSize * MaxItemsShown + Padding * 2 + SelectedFontSize + Padding;
        int center = Raylib.GetScreenHeight() / 2;
        int top = center - (menuHeight / 2 + Padding * 3 + TitleFontSize);
        _title.DrawCentered(top, TitleFontSize, Color.White);

        top += TitleFontSize + Padding * 3;
        for (int ix = _selectedIx - 2; ix < _selectedIx + MaxItemsShown; ix++)
        {
            if (_selectedIx == ix)
            {
                if (_selectedIx >= 0 && _selectedIx < _items.Length)
                {
                    _items[_selectedIx].ToString().DrawCentered(top, SelectedFontSize, Color.Yellow);
                }
                top += SelectedFontSize + Padding;
            }
            else
            {
                if (ix >= 0 && ix < _items.Length)
                {
                    _items[ix].ToString().DrawCentered(top, NotSelectedFontSize, Color.White);
                }
                top += NotSelectedFontSize + Padding;
            }
        }
    }
}