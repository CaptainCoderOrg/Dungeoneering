namespace CaptainCoder.DungeonCrawler.Combat;
public record AttackResult
{
    public int Damage { get; init; } = 0;
}

public abstract record AttackResultEvent;
public record AttackResultEvents(IEnumerable<AttackResultEvent> Results) : AttackResultEvent;
public record LostGuardEvent(string TargetName) : AttackResultEvent;
public record LostRestEvent(string TargetName) : AttackResultEvent;
public record AttackHitEvent(string TargetName, int Damage, int Blocked) : AttackResultEvent;
public record TargetKilledEvent(string TargetName) : AttackResultEvent;
public record ArmorAbsorbedHitEvent(string TargetName) : AttackResultEvent;
public record EmptyTarget : AttackResultEvent;

public static class AttackResultEventExtensions
{
    public static bool IsTargetKilledEvent(this AttackResultEvent evt)
    {
        if (evt is TargetKilledEvent) { return true; }
        if (evt is AttackResultEvents(var evts)) { evts.Any(IsTargetKilledEvent); }
        return false;
    }

    public static int TotalDamage(this AttackResultEvent evt)
    {
        if (evt is AttackHitEvent hit) { return hit.Damage; }
        else if (evt is AttackResultEvents evts) { return evts.Results.Sum(TotalDamage); }
        return 0;
    }
}