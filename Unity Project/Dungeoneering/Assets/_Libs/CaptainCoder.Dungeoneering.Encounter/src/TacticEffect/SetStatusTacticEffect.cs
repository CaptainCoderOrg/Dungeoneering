using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter;

[Serializable]
public class SetStatusTacticEffect : ITacticEffect, IOnTurnStart
{
    public string Name = "Set Status";
    [field: SerializeField] public StatusData Status { get; private set; }
    public void Apply(FigureData figure)
    {
        figure.Attacks++;
    }

    public void OnTurnStart(FigureData figure) => figure.EntityData.AddEffect(Status.Effect);

    public bool TryValidate(FigureData figure, IEnumerable<ITacticEffect> others, out string message)
    {
        ITacticEffect other = others.FirstOrDefault(o => o is SetStatusTacticEffect);
        if (other is SetStatusTacticEffect otherStatus)
        {
            message = $"<color=red>Conflict: {otherStatus.Status.Name}</color>";
            return false;
        }
        message = Status.Name;
        return true;
    }
}