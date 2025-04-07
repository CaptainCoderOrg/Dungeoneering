using System;
using System.Collections.Generic;

namespace CaptainCoder.Dungeoneering.Encounter;

[Serializable]
public class GainMovementTacticEffect : ITacticEffect, IOnTurnStart
{
    public string Name = "Gain Movement";
    public void Apply(FigureData figure)
    {
        figure.Attacks++;
    }

    public void OnTurnStart(FigureData figure) => figure.Movement += figure.EntityData.Speed;

    public bool TryValidate(FigureData figure, IEnumerable<ITacticEffect> others, out string message)
    {
        message = $"+{figure.EntityData.Speed} Movement";
        return true;
    }
}