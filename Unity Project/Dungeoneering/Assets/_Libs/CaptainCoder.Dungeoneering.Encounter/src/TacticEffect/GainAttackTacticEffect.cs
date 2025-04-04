using System;
using System.Collections.Generic;

namespace CaptainCoder.Dungeoneering.Encounter;

[Serializable]
public class GainAttackTacticEffect : ITacticEffect
{
    const string Effect = "+1 Attack";
    public string Name = "Gain Attack";
    public void Apply(FigureData figure)
    {
        figure.Attacks++;
    }

    public bool TryValidate(FigureData figure, IEnumerable<ITacticEffect> others, out string message)
    {
        message = Effect;
        return true;
    }
}
