namespace CaptainCoder.DungeonCrawler.Combat;


public class Weapon
{
    public string Name { get; init; } = "Unnamed Weapon";
    public IAttackRoll AttackRoll { get; init; } = new SimpleAttack(1, 2);
}