using System.Collections.Generic;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter;

public abstract record class EncounterEvent;

public sealed record class MoveFigureEvent(EncounterFigureController Controller, IEnumerable<Vector2Int> Path);
public sealed record class FigureTakeDamage();