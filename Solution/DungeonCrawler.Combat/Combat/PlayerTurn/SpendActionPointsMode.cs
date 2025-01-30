namespace CaptainCoder.DungeonCrawler.Combat;

public class SpendActionPointsMode(PlayerCharacter character)
{
    private readonly PlayerCharacter _originalCharacter = character;
    public PlayerCharacter Character { get; private set; } = character;
    private readonly SpendActionMenuItem[] _actions = (SpendActionMenuItem[])Enum.GetValues(typeof(SpendActionMenuItem));
    private int _selectedIx = 0;
    public event Action<SpendActionMenuItem>? OnSelectionChange;
    public event Action<PlayerCharacter>? OnCancel;
    public event Action<SpendPointResult>? OnSelected;
    public event Action? OnToggleHelp;

    public void HandleInput(SpendActionPointsControls input)
    {
        Action action = input switch
        {
            SpendActionPointsControls.Next => () => Next(1),
            SpendActionPointsControls.Previous => () => Next(-1),
            SpendActionPointsControls.Cancel => () => OnCancel?.Invoke(_originalCharacter),
            SpendActionPointsControls.Select => () => Select(_actions[_selectedIx]),
            _ => throw new NotImplementedException($"Unknown input {input}"),
        };
        action.Invoke();
    }

    private void Select(SpendActionMenuItem menuItem)
    {
        if (menuItem is SpendActionMenuItem.Cancel)
        {
            OnCancel?.Invoke(_originalCharacter);
            return;
        }

        if (menuItem is SpendActionMenuItem.ToggleHelp)
        {
            OnToggleHelp?.Invoke();
            return;
        }

        if (Character.ActionPoints <= 0)
        {
            OnSelected?.Invoke(new SpendPointResult("Not enough points", menuItem, Character));
            return;
        }

        SpendAction spendAction = menuItem switch
        {
            SpendActionMenuItem.Move => SpendAction.BuyMovement,
            SpendActionMenuItem.Attack => SpendAction.BuyAttack,
            SpendActionMenuItem.Guard => SpendAction.BuyGuard,
            SpendActionMenuItem.Rest => SpendAction.BuyRest,
            _ => throw new NotImplementedException($"Unexpected MenuItem {menuItem}"),
        };
        var orig = Character;
        Character = Character.SpendActionPoint(spendAction);
        string message = spendAction switch
        {
            SpendAction.BuyMovement => $"{Character.Card.Name} prepares to move. Total moves: {Character.MovementPoints}.",
            SpendAction.BuyAttack => $"{Character.Card.Name} prepares to attack. Total attacks: {Character.AttackPoints}",
            SpendAction.BuyGuard => $"{Character.Card.Name} prepares to Guard",
            SpendAction.BuyRest => $"{Character.Card.Name} prepares to Rest",
            _ => throw new NotImplementedException($"Unknown action {spendAction}"),
        };
        OnSelected?.Invoke(new SpendPointResult(message, menuItem, Character) { IsCharacterChanged = Character != orig });
    }

    private void Next(int delta)
    {
        _selectedIx += delta;
        if (_selectedIx < 0) { _selectedIx = _actions.Length - 1; }
        else if (_selectedIx >= _actions.Length) { _selectedIx = 0; }
        OnSelectionChange?.Invoke(_actions[_selectedIx]);
    }
}

public record SpendPointResult(string Message, SpendActionMenuItem SelectedAction, PlayerCharacter Character)
{
    public bool IsCharacterChanged { get; init; } = true;
}

public enum SpendActionMenuItem
{
    Move,
    Attack,
    Guard,
    Rest,
    Cancel,
    ToggleHelp,
}

public enum SpendActionPointsControls
{
    Next,
    Previous,
    Select,
    Cancel,
}