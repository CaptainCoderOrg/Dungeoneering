
using System.Collections.Generic;

namespace CaptainCoder.Dungeoneering.Encounter;
public interface ITacticEffect : IOnTurnStart
{
    public void Apply(FigureData figure);
    public bool TryValidate(FigureData figure, IEnumerable<ITacticEffect> others, out string message);
}

public interface IOnTurnStart
{
    public void OnTurnStart(FigureData figure);
}