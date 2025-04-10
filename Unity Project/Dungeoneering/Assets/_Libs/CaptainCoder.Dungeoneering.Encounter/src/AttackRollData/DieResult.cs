namespace CaptainCoder.Dungeoneering.Encounter;
public record struct DieResult(DieData Die, int Face)
{
    public readonly int Damage => Die.Faces[Face].Damage;
    public readonly int Accuracy => Die.Faces[Face].Aim;
    public readonly int Power => Die.Faces[Face].Power;
    public readonly int Split => Die.Faces[Face].IsSplit ? 1 : 0;
}