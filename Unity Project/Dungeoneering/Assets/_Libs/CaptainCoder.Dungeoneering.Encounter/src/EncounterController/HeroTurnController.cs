using System;
using System.Collections.Generic;
using System.Linq;

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
        private MoveInfo _currentMoveInfo;
        private HashSet<MoveInfo> _possibleMoves;

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
            _currentMoveInfo = null;
            _possibleMoves = FindMoves(FigureController.Figure, State, Controller.EncounterData);
            HashSet<Vector2Int> positions = _possibleMoves.Select(p => p.Position).ToHashSet();
            foreach (MoveInfo moveInfo in _possibleMoves)
            {
                if (Controller.TileSelectors.TryGetValue(moveInfo.Position, out var selector))
                {
                    selector.ClearEvents();
                    selector.Highlight();
                    selector.OnMouseEntered += () => ShowMoveInfo(moveInfo);
                    selector.OnMouseExited += () => ClearMoveInfo(positions);
                    selector.OnClicked += () => PerformMove(moveInfo);
                }
            }
        }

        private void PerformMove(MoveInfo moveInfo)
        {
            FigureController.Figure.Movement -= moveInfo.Distance;
            foreach (Vector2Int position in moveInfo.Path())
            {
                if (Controller.TileSelectors.TryGetValue(position, out var selector))
                {
                    selector.ClearEvents();
                    selector.Hide();
                }
            }
            foreach (MoveInfo info in _possibleMoves)
            {
                if (Controller.TileSelectors.TryGetValue(info.Position, out var selector))
                {
                    selector.ClearEvents();
                    selector.Hide();
                }
            }
            Controller.HandleMovementEvent(new MoveFigureEvent(FigureController, moveInfo.Path().Reverse()));
        }

        private void ClearMoveInfo(HashSet<Vector2Int> possibleMoves)
        {
            if (_currentMoveInfo == null) { return; }
            foreach (Vector2Int position in _currentMoveInfo.Path())
            {
                EncounterTileSelector selector = Controller.TileSelectors[position];
                if (!possibleMoves.Contains(position)) { selector.Hide(); }
                else { selector.Highlight(); }
            }
        }
        private void ShowMoveInfo(MoveInfo moveInfo)
        {
            _currentMoveInfo = moveInfo;
            foreach (Vector2Int position in moveInfo.Path())
            {
                Controller.TileSelectors[position].Selected();
            }
        }

        internal static HashSet<MoveInfo> FindMoves(FigureData figure, EncounterState state, EncounterData data)
        {
            HashSet<MoveInfo> validMoves = new();
            HashSet<Vector2Int> visited = new() { figure.Position };
            Queue<MoveInfo> queue = new();
            queue.Enqueue(new MoveInfo(figure.Position, null, 0));
            while (queue.TryDequeue(out MoveInfo currentPosition))
            {
                foreach (MoveInfo neighbor in GetNeighbors(currentPosition))
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

            IEnumerable<MoveInfo> GetNeighbors(MoveInfo p)
            {
                if (p.Distance >= figure.Movement) { yield break; }
                int distance = p.Distance + 1;
                foreach (Facing f in Facings)
                {
                    // Cannot pass through walls
                    if (data.DungeonCrawlerData.CurrentDungeon.IsPassable(p.Position, f))
                    {
                        Vector2Int afterStep = p.Position.Step(f);
                        // Cannot move into space with enemy
                        if (state.Figures.TryGetValue(afterStep, out EncounterFigureController otherfigure) && otherfigure.Figure.EntityData is EnemyEntityData)
                        {
                            continue;
                        }
                        yield return p with { Position = afterStep, Distance = distance, PreviousSpace = p };
                    }
                }
            }
        }
        private static readonly Facing[] Facings = new[] { Facing.North, Facing.East, Facing.South, Facing.West };
    }

    record class MoveInfo(Vector2Int Position, MoveInfo PreviousSpace, int Distance)
    {
        public IEnumerable<Vector2Int> Path()
        {
            MoveInfo current = this;
            while (current != null)
            {
                yield return current.Position;
                current = current.PreviousSpace;
            }
        }
    }

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