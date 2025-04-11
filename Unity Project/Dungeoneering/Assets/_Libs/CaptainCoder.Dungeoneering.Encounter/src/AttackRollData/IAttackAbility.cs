namespace CaptainCoder.Dungeoneering.Encounter;

public interface IAttackAbility
{
    public int PowerCost { get; }
    public int DamageBonus { get; }
    public int AccuracyBonus { get; }
}