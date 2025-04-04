using System;
using System.Collections.Generic;

namespace CaptainCoder.Dungeoneering.Encounter;

[Serializable]
public class GainMovementTacticEffect : ITacticEffect
{
    public string Name = "Gain Movement";
    public void Apply(FigureData figure)
    {
        figure.Attacks++;
    }

    public bool TryValidate(FigureData figure, IEnumerable<ITacticEffect> others, out string message)
    {
        message = $"+{figure.EntityData.Speed} Movement";
        return true;
    }
}