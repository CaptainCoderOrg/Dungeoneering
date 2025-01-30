using System.Collections;

namespace CaptainCoder.DungeonCrawler;

public class Party : IEnumerable<PlayerCharacter>
{
    public Party Copy()
    {
        Party newParty = new();
        newParty._topLeft = _topLeft;
        newParty._topRight = _topRight;
        newParty._bottomLeft = _bottomLeft;
        newParty._bottomRight = _bottomRight;
        return newParty;
    }
    public PlayerCharacter this[int ix]
    {
        get => ToArray[ix];
    }
    private PlayerCharacter _topLeft = new() { Card = Characters.CharacterA };
    private PlayerCharacter _topRight = new() { Card = Characters.NoBody };
    private PlayerCharacter _bottomLeft = new() { Card = Characters.NoBody };
    private PlayerCharacter _bottomRight = new() { Card = Characters.NoBody };
    private PlayerCharacter[] ToArray => [_topLeft, _topRight, _bottomLeft, _bottomRight];

    public PlayerCharacter TopLeft
    {
        get => _topLeft;
        set
        {
            _topLeft = value; OnTopLeftChange?.Invoke(value);
        }
    }
    public PlayerCharacter TopRight
    {
        get => _topRight;
        set
        {
            _topRight = value;
            OnTopRightChange?.Invoke(value);
        }
    }

    public PlayerCharacter BottomLeft
    {
        get => _bottomLeft;
        set
        {
            _bottomLeft = value;
            OnBottomLeftChange?.Invoke(value);
        }
    }

    public PlayerCharacter BottomRight
    {
        get => _bottomRight;
        set
        {
            _bottomRight = value;
            OnBottomRightChange?.Invoke(value);
        }
    }

    public event Action<PlayerCharacter>? OnTopLeftChange;
    public event Action<PlayerCharacter>? OnTopRightChange;
    public event Action<PlayerCharacter>? OnBottomLeftChange;
    public event Action<PlayerCharacter>? OnBottomRightChange;

    public bool IsDead => ToArray.All(pc => pc.IsDead() || pc.Card == Characters.NoBody);

    public void UpdateCharacter(PlayerCharacter toUpdate)
    {
        if (TopLeft.Card.Name == toUpdate.Card.Name) { TopLeft = toUpdate; }
        else if (TopRight.Card.Name == toUpdate.Card.Name) { TopRight = toUpdate; }
        else if (BottomLeft.Card.Name == toUpdate.Card.Name) { BottomLeft = toUpdate; }
        else if (BottomRight.Card.Name == toUpdate.Card.Name) { BottomRight = toUpdate; }
    }

    public IEnumerator<PlayerCharacter> GetEnumerator() => ToArray.AsEnumerable().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void ClearActions()
    {
        TopLeft = TopLeft with { ActionPoints = 0, AttackPoints = 0, MovementPoints = 0, State = CharacterState.Normal };
        TopRight = TopRight with { ActionPoints = 0, AttackPoints = 0, MovementPoints = 0, State = CharacterState.Normal };
        BottomLeft = BottomLeft with { ActionPoints = 0, AttackPoints = 0, MovementPoints = 0, State = CharacterState.Normal };
        BottomRight = BottomRight with { ActionPoints = 0, AttackPoints = 0, MovementPoints = 0, State = CharacterState.Normal };
    }

    public void ApplyValues(Party party)
    {
        foreach (PlayerCharacter character in party)
        {
            UpdateCharacter(character);
        }
    }
}