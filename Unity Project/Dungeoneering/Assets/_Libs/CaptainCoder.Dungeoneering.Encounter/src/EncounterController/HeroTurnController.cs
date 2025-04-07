using System;
using System.Collections.Generic;

using CaptainCoder.Dungeoneering.DungeonMap;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class HeroTurnController : MonoBehaviour
    {
        private EncounterController _controller;
        private EncounterController Controller => _controller = (_controller == null ? GetComponentInParent<EncounterController>() : _controller);
        private EncounterState State => Controller.State;
        [field: SerializeField] public EncounterFigureController FigureController { get; private set; }

        internal void BeginTurn(EncounterFigureController figureController, IEnumerable<TacticData> tactics)
        {
            Controller.TacticsMenu.Cancel();
            FigureController = figureController;
            foreach (var tactic in tactics)
            {
                tactic.Effect.OnTurnStart(FigureController.Figure);
            }
            foreach (var panel in Controller.HeroPanels)
            {
                panel.StartTurn(figureController);
            }
        }

        internal void ShowMove()
        {
            HashSet<SearchPosition> positions = FindMoves(FigureController.Figure, State, Controller.EncounterData);
            foreach (SearchPosition position in positions)
            {
                Controller.TileSelectors[position.Position].ShowHighlight();
            }
        }

        internal static HashSet<SearchPosition> FindMoves(FigureData figure, EncounterState state, EncounterData data)
        {
            HashSet<SearchPosition> validMoves = new();
            HashSet<Vector2Int> visited = new() { figure.Position };
            Queue<SearchPosition> queue = new();
            queue.Enqueue(new SearchPosition(figure.Position, figure.Position, 0));
            while (queue.TryDequeue(out SearchPosition currentPosition))
            {
                foreach (SearchPosition neighbor in GetNeighbors(currentPosition))
                {
                    if (visited.Contains(neighbor.Position)) { continue; }
                    visited.Add(neighbor.Position);
                    if (!state.Figures.ContainsKey(neighbor.Position))
                    {
                        // If there is no figure in this space, we can end our movement here
                        validMoves.Add(neighbor);
                    }
                    queue.Enqueue(neighbor);
                }
            }

            return validMoves;

            IEnumerable<SearchPosition> GetNeighbors(SearchPosition p)
            {
                if (p.Distance >= figure.Movement) { yield break; }
                int distance = p.Distance + 1;
                foreach (Facing f in Facings)
                {
                    // Cannot pass through walls
                    if (data.DungeonCrawlerData.CurrentDungeon.IsPassable(p.Position, f))
                    {
                        Debug.Log($"Was passable: {p.Position}, {f}");
                        Vector2Int afterStep = p.Position.Step(f);
                        // Cannot move into space with enemy
                        if (state.Figures.TryGetValue(afterStep, out EncounterFigureController otherfigure) && otherfigure.Figure.EntityData is EnemyEntityData)
                        {
                            continue;
                        }
                        yield return p with { Position = afterStep, Distance = distance, PreviousSpace = p.Position };
                    }
                    else
                    {
                        Debug.Log($"Was not passable: {p.Position}, {f}");
                    }
                }
            }
        }
        private static readonly Facing[] Facings = new[] { Facing.North, Facing.East, Facing.South, Facing.West };
    }

    record struct SearchPosition(Vector2Int Position, Vector2Int PreviousSpace, int Distance);

    public static class DungeonExtensions
    {
        public static bool IsPassable(this Dungeon dungeon, Vector2Int position, Facing facing) => dungeon.IsPassable(new Position(position.x, position.y), facing);

        public static Vector2Int Step(this Vector2Int position, Facing f) => f switch
        {
            Facing.North => new(position.x, position.y - 1),
            Facing.South => new(position.x, position.y + 1),
            Facing.East => new(position.x + 1, position.y),
            Facing.West => new(position.x - 1, position.y),
            _ => throw new Exception($"Unexpected facing: {f}"),
        };
    }

}