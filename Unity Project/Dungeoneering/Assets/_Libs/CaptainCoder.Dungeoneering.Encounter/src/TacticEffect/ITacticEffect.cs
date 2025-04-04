
using System.Collections.Generic;

namespace CaptainCoder.Dungeoneering.Encounter;
public interface ITacticEffect
{
    public void Apply(FigureData figure);
    public bool TryValidate(FigureData figure, IEnumerable<ITacticEffect> others, out string message);
}