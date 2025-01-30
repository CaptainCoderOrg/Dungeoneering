using CaptainCoder.DungeonCrawler.Combat;

namespace CaptainCoder.DungeonCrawler;

public record CharacterCard(string Name)
{
    public int BaseHealth { get; init; } = 10;
    public int BaseEnergy { get; init; } = 4;
    public int BaseArmor { get; init; } = 0;
    public int BaseSpeed { get; init; } = 4;
    public CharacterCard() : this("Unnamed") { }
}
public record PlayerCharacter(CharacterCard Card)
{
    public Weapon Weapon { get; init; } = Weapons.Hands;
    public int Wounds { get; init; } = 0;
    public int Exertion { get; init; } = 0;
    public int ActionPoints { get; init; } = 0;
    public int MovementPoints { get; init; } = 0;
    public int AttackPoints { get; init; } = 0;
    public CharacterState State { get; init; } = CharacterState.Normal;
    public PlayerCharacter() : this(new CharacterCard()) { }
}

public static class CharacterExtensions
{
    public static PlayerCharacter AddWounds(this PlayerCharacter pc, int wounds) => pc with { Wounds = pc.Wounds + wounds };
    public static int Health(this PlayerCharacter pc) => pc.Card.BaseHealth - pc.Wounds;
    public static bool IsDead(this PlayerCharacter pc) => pc.Health() <= 0;
    public static PlayerCharacter AddExertion(this PlayerCharacter pc, int exertion) => pc with { Exertion = pc.Exertion + exertion };
    public static int Energy(this PlayerCharacter pc) => pc.Card.BaseEnergy - pc.Exertion;
    public static PlayerCharacter SpendActionPoint(this PlayerCharacter pc, SpendAction action) => action switch
    {
        _ when pc.ActionPoints == 0 => pc,
        SpendAction.BuyMovement => pc with { ActionPoints = pc.ActionPoints - 1, MovementPoints = pc.MovementPoints + pc.Card.BaseSpeed },

        SpendAction.BuyAttack => pc with { ActionPoints = pc.ActionPoints - 1, AttackPoints = pc.AttackPoints + 1 },

        SpendAction.BuyRest when pc.State is CharacterState.Rest => pc,
        SpendAction.BuyRest => pc with { ActionPoints = pc.ActionPoints - 1, State = CharacterState.Rest },

        SpendAction.BuyGuard when pc.State is CharacterState.Guard => pc,
        SpendAction.BuyGuard => pc with { ActionPoints = pc.ActionPoints - 1, State = CharacterState.Guard },

        _ => throw new NotImplementedException($"Not Implemented for {action.GetType()}: {action}"),
    };
}

public enum CharacterState
{
    Normal,
    Guard,
    Rest
}

public enum SpendAction
{
    BuyMovement,
    BuyAttack,
    BuyRest,
    BuyGuard,
}