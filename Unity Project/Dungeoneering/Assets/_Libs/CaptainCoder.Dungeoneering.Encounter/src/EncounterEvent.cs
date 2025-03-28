using System.Collections.Generic;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter;

public abstract record class EncounterEvent;

public sealed record class MoveFigureEvent(FigureData Figure, IEnumerable<Vector2Int> Path);