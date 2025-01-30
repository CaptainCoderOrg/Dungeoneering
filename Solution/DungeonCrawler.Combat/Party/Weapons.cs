namespace CaptainCoder.DungeonCrawler.Combat;


public static class Weapons
{
    public static Weapon Hands { get; } = new() { Name = "Hands", AttackRoll = new SimpleAttack(1, 2) };
    public static Weapon Dagger { get; } = new() { Name = "Dagger", AttackRoll = new SimpleAttack(2, 4) };
    public static Weapon Sword { get; } = new() { Name = "Sword", AttackRoll = new SimpleAttack(3, 6) };
}