namespace Tests;

using CaptainCoder.DungeonCrawler;

using Shouldly;

public class PlayerCharacter_should
{

    [Theory]
    [InlineData(10, 4, 6)]
    [InlineData(12, 2, 10)]
    [InlineData(7, 3, 4)]
    public void add_wounds(int baseHealth, int woundsToAdd, int expectedHealth)
    {
        CharacterCard card = new() { BaseHealth = baseHealth };
        PlayerCharacter playerCharacter = new() { Card = card };

        PlayerCharacter actual = playerCharacter.AddWounds(woundsToAdd);
        actual.Health().ShouldBe(expectedHealth);
    }

    [Theory]
    [InlineData(4, 1, 3)]
    [InlineData(3, 3, 0)]
    [InlineData(7, 5, 2)]
    public void add_exertion(int baseEnergy, int exertionToAdd, int expectedEnergy)
    {
        CharacterCard card = new() { BaseEnergy = baseEnergy };
        PlayerCharacter playerCharacter = new() { Card = card };

        PlayerCharacter actual = playerCharacter.AddExertion(exertionToAdd);
        actual.Energy().ShouldBe(expectedEnergy);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(8)]
    [InlineData(9)]
    public void be_dead(int baseHealth)
    {
        CharacterCard card = new() { BaseHealth = baseHealth };
        PlayerCharacter underTest = new() { Card = card, Wounds = baseHealth };
        underTest.IsDead().ShouldBeTrue();
    }

    [Theory]
    [InlineData(5, 4)]
    [InlineData(8, 0)]
    [InlineData(9, 2)]
    public void not_be_dead(int baseHealth, int wounds)
    {
        CharacterCard card = new() { BaseHealth = baseHealth };
        PlayerCharacter underTest = new() { Card = card, Wounds = wounds };
        underTest.IsDead().ShouldBeFalse();
    }

    [Theory]
    [InlineData(4, 0, 2, 4, 1)]
    [InlineData(3, 2, 1, 5, 0)]
    [InlineData(6, 0, 0, 0, 0)]
    [InlineData(4, 3, 0, 3, 0)]
    public void spend_point_on_move(int speed, int movementPoints, int actionPoints, int expectedMovementPoints, int expectedActionPoints)
    {
        PlayerCharacter underTest = new()
        {
            Card = new CharacterCard() { BaseSpeed = speed },
            ActionPoints = actionPoints,
            MovementPoints = movementPoints,
        };
        PlayerCharacter actual = underTest.SpendActionPoint(SpendAction.BuyMovement);

        actual.ActionPoints.ShouldBe(expectedActionPoints);
        actual.MovementPoints.ShouldBe(expectedMovementPoints);
    }

    [Theory]
    [InlineData(0, 2, 1, 1)]
    [InlineData(1, 1, 2, 0)]
    [InlineData(2, 0, 2, 0)]
    [InlineData(1, 0, 1, 0)]
    public void spend_point_on_attack(int attackPoints, int actionPoints, int expectedAttackPoints, int expectedActionPoints)
    {
        PlayerCharacter underTest = new()
        {
            Card = new CharacterCard(),
            ActionPoints = actionPoints,
            AttackPoints = attackPoints,
        };
        PlayerCharacter actual = underTest.SpendActionPoint(SpendAction.BuyAttack);

        actual.ActionPoints.ShouldBe(expectedActionPoints);
        actual.AttackPoints.ShouldBe(expectedAttackPoints);
    }

    [Theory]
    [InlineData(CharacterState.Normal, 2, CharacterState.Rest, 1)]
    [InlineData(CharacterState.Normal, 0, CharacterState.Normal, 0)]
    [InlineData(CharacterState.Normal, 1, CharacterState.Rest, 0)]
    [InlineData(CharacterState.Rest, 1, CharacterState.Rest, 1)]
    [InlineData(CharacterState.Guard, 1, CharacterState.Rest, 0)]
    public void spend_point_on_rest(CharacterState state, int actionPoints, CharacterState expectedState, int expectedActionPoints)
    {
        PlayerCharacter underTest = new()
        {
            Card = new CharacterCard(),
            ActionPoints = actionPoints,
            State = state,
        };
        PlayerCharacter actual = underTest.SpendActionPoint(SpendAction.BuyRest);

        actual.ActionPoints.ShouldBe(expectedActionPoints);
        actual.State.ShouldBe(expectedState);
    }

    [Theory]
    [InlineData(CharacterState.Normal, 2, CharacterState.Guard, 1)]
    [InlineData(CharacterState.Normal, 0, CharacterState.Normal, 0)]
    [InlineData(CharacterState.Normal, 1, CharacterState.Guard, 0)]
    [InlineData(CharacterState.Rest, 1, CharacterState.Guard, 0)]
    [InlineData(CharacterState.Guard, 1, CharacterState.Guard, 1)]
    public void spend_point_on_guard(CharacterState state, int actionPoints, CharacterState expectedState, int expectedActionPoints)
    {
        PlayerCharacter underTest = new()
        {
            Card = new CharacterCard(),
            ActionPoints = actionPoints,
            State = state,
        };
        PlayerCharacter actual = underTest.SpendActionPoint(SpendAction.BuyGuard);

        actual.ActionPoints.ShouldBe(expectedActionPoints);
        actual.State.ShouldBe(expectedState);
    }
}