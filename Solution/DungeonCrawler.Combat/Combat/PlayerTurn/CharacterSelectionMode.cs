namespace CaptainCoder.DungeonCrawler.Combat;

public class CharacterSelectionMode(IEnumerable<PlayerCharacter> characters, PlayerCharacter? selected = null)
{
    private readonly PlayerCharacter[] _characters = [.. characters];
    public int SelectedIx { get; private set; } = FirstSelected(characters, selected);
    public event Action<int, PlayerCharacter>? OnSelectionChange;
    public event Action<int, PlayerCharacter>? OnSelected;
    public bool HasSelection => _characters.Any(c => c.ActionPoints > 0);
    public bool IsSelected(int ix) => ix == SelectedIx;
    public bool IsFinished(int ix) => _characters[ix].ActionPoints <= 0;
    public (int, PlayerCharacter) FirstSelected(PlayerCharacter? selected = null)
    {
        int ix = FirstSelected(_characters, selected);
        if (ix >= 0) { return (ix, _characters[ix]); }
        throw new Exception($"No available characters.");
    }

    private static int FirstSelected(IEnumerable<PlayerCharacter> toSearch, PlayerCharacter? selected = null)
    {
        PlayerCharacter[] characters = [.. toSearch];
        for (int ix = 0; ix < characters.Length; ix++)
        {
            if (selected == null && characters[ix].ActionPoints > 0) { return ix; }
            if (selected != null && characters[ix].Card.Name == selected.Card.Name) { return ix; }
        }
        return -1;
    }

    public void HandleInput(CharacterSelectionControl input)
    {
        Action action = input switch
        {
            CharacterSelectionControl.Next => () => Next(1),
            CharacterSelectionControl.Previous => () => Next(-1),
            CharacterSelectionControl.Select => () => OnSelected?.Invoke(SelectedIx, _characters[SelectedIx]),
            _ => throw new NotImplementedException($"Unknown input {input}"),
        };
        action.Invoke();
    }

    private void Next(int delta)
    {
        if (!HasSelection) { return; }
        do
        {
            SelectedIx += delta;
            if (SelectedIx < 0) { SelectedIx = _characters.Length - 1; }
            else if (SelectedIx >= _characters.Length) { SelectedIx = 0; }
        } while (_characters[SelectedIx].ActionPoints <= 0);

        OnSelectionChange?.Invoke(SelectedIx, _characters[SelectedIx]);
    }

}

public enum CharacterSelectionControl
{
    Next,
    Previous,
    Select,
}