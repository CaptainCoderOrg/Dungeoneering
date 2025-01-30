using CaptainCoder.Dungeoneering.Game.Unity;

namespace CaptainCoder.DungeonCrawler.Combat.Unity;
public static class CombatExtensions
{
    public static AttackResultEvent DoAttack(this CombatMap map, CharacterCard card, IAttackRoll roll, Position target)
    {
        PlayerCharacter character = map.GetCharacter(card);
        AttackResult result = roll.GetRoll(IRandom.Default);
        AttackResultEvent eventResult = map.ApplyAttack(target, result);

        string message = eventResult switch
        {
            AttackHitEvent hit => $"{hit.TargetName} takes {hit.Damage} damage.",
            TargetKilledEvent killed => $"{killed.TargetName} was slain!",
            ArmorAbsorbedHitEvent hit => $"{hit.TargetName}'s armor absorbs the blow.",
            EmptyTarget => $"{card.Name} attacks but nothing is there.",
            _ => throw new NotImplementedException($"Unknown event: {eventResult}"),
        };

        MessageRenderer.Shared.AddMessage(message);

        PlayerCharacter updated = character with { AttackPoints = Math.Max(0, character.AttackPoints - 1) };
        map.UpdateCharacter(updated);
        return eventResult;
    }
}