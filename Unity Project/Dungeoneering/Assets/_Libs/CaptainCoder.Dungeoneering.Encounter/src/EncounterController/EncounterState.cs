using System.Collections.Generic;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EncounterState
    {
        public Dictionary<Vector2Int, EncounterFigureController> Figures { get; } = new();
    }
}