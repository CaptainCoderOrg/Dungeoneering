using System;
using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class HeroTurnController : MonoBehaviour
    {
        private static readonly Facing[] Facings = new[] { Facing.North, Facing.East, Facing.South, Facing.West };
        private static readonly Vector2Int[] Directions = new Vector2Int[]
        {
            new(-1, -1), new( 0, -1), new( 1, -1),
            new(-1,  0),              new( 1,  0),
            new(-1,  1), new( 0,  1), new( 1,  1),
         };
        private EncounterController _controller;
        private EncounterController Controller => _controller = (_controller == null ? GetComponentInParent<EncounterController>() : _controller);
        private EncounterState State => Controller.State;
        [AssertIsSet][SerializeField] private AttackConfirmationDialogue _attackConfirmationDialogue;
        [field: SerializeField] public EncounterFigureController FigureController { get; private set; }
        private MoveInfo _currentMoveInfo;
        private HashSet<MoveInfo> _possibleMoves;
        private HashSet<AttackInfo> _possibleAttacks;

        public void ShowPossibleAttacks(FigureData attacker, AttackData attack)
        {
            if (attacker == null || attack == null)
            {
                Debug.LogWarning("Attacker / Attack must be non-null", this);
                Debug.LogWarning($"Attacker: {attacker}", attacker);
                Debug.LogWarning($"Attack: {attack}", attack);
                return;
            }
            _possibleAttacks = FindAttackTargets(attacker, attack, State, Controller.EncounterData);
            HighlightAttacks(_possibleAttacks);
        }

        private void HighlightAttacks(HashSet<AttackInfo> attacks)
        {
            ClearTiles();
            foreach (AttackInfo attack in attacks)
            {
                if (Controller.TileSelectors.TryGetValue(attack.TargetPosition, out EncounterTileSelector tile))
                {
                    if (attack.Target == null) { tile.ShowAttackRange(); }
                    else { tile.ValidAttackTarget(); }
                }
            }
        }

        internal static HashSet<AttackInfo> FindAttackTargets(FigureData attacker, AttackData attackData, EncounterState state, EncounterData data)
        {
            int count = 0;
            HashSet<AttackInfo> possiblePositions = new();
            HashSet<Vector2Int> visited = new() { attacker.Position };
            Queue<AttackInfo> queue = new();
            queue.Enqueue(new AttackInfo(attacker.Position, attacker.Position, null, 0));
            while (queue.TryDequeue(out AttackInfo currentPosition) && count++ < 1000)
            {
                foreach (AttackInfo neighbor in GetNeighbors(currentPosition))
                {
                    if (visited.Contains(neighbor.TargetPosition)) { continue; }
                    visited.Add(neighbor.TargetPosition);
                    possiblePositions.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
            if (count >= 1000)
            {
                Debug.LogWarning($"Search exceeded expected size");
            }
            return possiblePositions;

            IEnumerable<AttackInfo> GetNeighbors(AttackInfo p)
            {
                // If the attack is not ranged, max distance is 1
                if (!attackData.AttackType.IsRanged && p.Distance >= 1) { yield break; }
                int distance = p.Distance + 1;
                foreach (Vector2Int delta in Directions)
                {

                    Vector2Int afterStep = p.TargetPosition + delta;
                    // Cannot pass through walls
                    if (data.DungeonCrawlerData.CurrentDungeon.IntersectsWall(p.StartPosition, afterStep)) { continue; }

                    // If line of site is required, check to see if there is a figure in between
                    if (attackData.AttackType.RequiresLineOfSight && state.IsFigureInBetween(p.StartPosition, afterStep)) { continue; }

                    if (state.Figures.TryGetValue(afterStep, out EncounterFigureController otherfigure))
                    {
                        yield return p with { TargetPosition = afterStep, Distance = distance, Target = otherfigure };
                    }
                    else
                    {
                        yield return p with { TargetPosition = afterStep, Distance = distance, Target = null };
                    }

                }
            }
        }

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
            ClearTiles();
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

        private void ClearTiles()
        {
            if (_currentMoveInfo != null)
            {
                foreach (Vector2Int position in _currentMoveInfo.Path())
                {
                    if (Controller.TileSelectors.TryGetValue(position, out var selector))
                    {
                        selector.ClearEvents();
                        selector.Hide();
                    }
                }
            }
            if (_possibleMoves != null)
            {
                foreach (MoveInfo info in _possibleMoves)
                {
                    if (Controller.TileSelectors.TryGetValue(info.Position, out var selector))
                    {
                        selector.ClearEvents();
                        selector.Hide();
                    }
                }
            }
            if (_possibleAttacks != null)
            {
                foreach (AttackInfo info in _possibleAttacks)
                {
                    if (Controller.TileSelectors.TryGetValue(info.TargetPosition, out var selector))
                    {
                        selector.ClearEvents();
                        selector.Hide();
                    }
                }
            }
        }

        private void PerformMove(MoveInfo moveInfo)
        {
            FigureController.Figure.Movement -= moveInfo.Distance;
            ClearTiles();
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

        internal void EndTurn()
        {
            ClearTiles();
            FigureController.Figure.Movement = 0;
            FigureController.Figure.Attacks = 0;
            FigureController.Figure.HasTakenTurn = true;
            foreach (var panel in Controller.HeroPanels)
            {
                panel.TurnEnded();
            }
            FigureController = null;
        }

        internal void StartAttack()
        {
            _attackConfirmationDialogue.Attacker = FigureController.Figure;
            _attackConfirmationDialogue.ClearTarget();
            _attackConfirmationDialogue.Show();
        }
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

    record class AttackInfo(Vector2Int StartPosition, Vector2Int TargetPosition, EncounterFigureController Target, int Distance);

    public static class DungeonExtensions
    {
        const float FigureRadius = 0.500f;
        public static bool IsPassable(this Dungeon dungeon, Vector2Int position, Facing facing) => dungeon.IsPassable(new Position(position.x, position.y), facing);

        /// <summary>
        /// Calculates if two tiles have a wall obscuring sight. If any corner on the first tile has no wall to any corner on the second tile, then the tile is not obscured.
        /// </summary>
        /// <param name="dungeon"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool IntersectsWall(this Dungeon dungeon, Vector2Int start, Vector2Int end)
        {
            IEnumerable<LineSegment> cornerToCornerPermuations = start.CornersToCenter(end);
            foreach (LineSegment cornerToCorner in cornerToCornerPermuations)
            {
                IEnumerable<LineSegment> wallSegements = dungeon.GetWallSegments(cornerToCorner.GetGridPositions());
                // If there are no wall segements between these two corners, the tiles are visible to each other
                if (!wallSegements.Any(seg => cornerToCorner.Intersects(seg))) { return false; }
            }
            return true;
        }

        public static bool IntersectsCircle(this LineSegment segment, Vector2Int center, float radius)
        {
            Vector2 d = segment.End - segment.Start;
            Vector2 f = segment.Start - center;

            float a = Vector2.Dot(d, d);
            float b = 2 * Vector2.Dot(f, d);
            float c = Vector2.Dot(f, f) - radius * radius;

            float discriminant = b * b - 4 * a * c;
            if (discriminant < 0)
            {
                // No intersection
                return false;
            }

            discriminant = MathF.Sqrt(discriminant);

            float t1 = (-b - discriminant) / (2 * a);
            float t2 = (-b + discriminant) / (2 * a);

            // Check if either intersection point lies on the segment
            return (t1 >= 0 && t1 <= 1) || (t2 >= 0 && t2 <= 1);
        }

        public static bool IsFigureInBetween(this EncounterState state, Vector2Int start, Vector2Int end)
        {
            LineSegment segment = new(start, end);
            return segment.GetGridPositions().Where(p => p != start && p != end).Where(state.Figures.ContainsKey).Any(c => segment.IntersectsCircle(c, FigureRadius));
        }

        public static Vector2Int Step(this Vector2Int position, Facing f) => f switch
        {
            Facing.North => new(position.x, position.y - 1),
            Facing.South => new(position.x, position.y + 1),
            Facing.East => new(position.x + 1, position.y),
            Facing.West => new(position.x - 1, position.y),
            _ => throw new Exception($"Unexpected facing: {f}"),
        };
    }

    public record struct LineSegment(Vector2 Start, Vector2 End);

    public static class GeometryUtils
    {
        const float EdgeDelta = 0.500f;
        const float CenterOffsetDelta = 0.0001f; // We step slightly to the side to allow attacking around corners

        /// <summary>
        /// Get a list of line segments between all corners of two positions
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static IEnumerable<LineSegment> CornersToCenter(this Vector2Int start, Vector2Int end)
        {
            foreach (var startCorner in start.Corners())
            {
                yield return new LineSegment(startCorner, end);
            }
        }

        public static IEnumerable<Vector2> Corners(this Vector2Int center)
        {
            yield return new(center.x - CenterOffsetDelta, center.y);
            yield return new(center.x + CenterOffsetDelta, center.y);
            yield return new(center.x, center.y - CenterOffsetDelta);
            yield return new(center.x, center.y + CenterOffsetDelta);
        }
        public static IEnumerable<Vector2Int> GetGridPositions(this LineSegment segment)
        {
            int minX = Mathf.FloorToInt(Mathf.Min(segment.Start.x, segment.End.x));
            int maxX = Mathf.FloorToInt(Mathf.Max(segment.Start.x, segment.End.x));
            int minY = Mathf.FloorToInt(Mathf.Min(segment.Start.y, segment.End.y));
            int maxY = Mathf.FloorToInt(Mathf.Max(segment.Start.y, segment.End.y));
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    yield return new Vector2Int(x, y);
                }
            }
        }

        public static IEnumerable<LineSegment> GetWallSegments(this Dungeon dungeon, IEnumerable<Vector2Int> positions)
        {
            return positions
                .Select(p => (p, new Position(p.x, p.y)))
                .Select(pair => (pair.p, dungeon.GetTile(pair.Item2).Walls))
                .SelectMany(GetWallSegments);

            static IEnumerable<LineSegment> GetWallSegments((Vector2Int, TileWalls) pair)
            {
                (Vector2Int centerP, TileWalls walls) = pair;
                if (walls.North is WallType.Solid)
                {
                    yield return new LineSegment(new Vector2(centerP.x - EdgeDelta, centerP.y - EdgeDelta),
                                                 new Vector2(centerP.x + EdgeDelta, centerP.y - EdgeDelta));
                }
                if (walls.South is WallType.Solid)
                {
                    yield return new LineSegment(new Vector2(centerP.x - EdgeDelta, centerP.y + EdgeDelta),
                                                 new Vector2(centerP.x + EdgeDelta, centerP.y + EdgeDelta));
                }

                if (walls.East is WallType.Solid)
                {
                    yield return new LineSegment(new Vector2(centerP.x + EdgeDelta, centerP.y - EdgeDelta),
                                                 new Vector2(centerP.x + EdgeDelta, centerP.y + EdgeDelta));
                }

                if (walls.West is WallType.Solid)
                {
                    yield return new LineSegment(new Vector2(centerP.x - EdgeDelta, centerP.y - EdgeDelta),
                                                 new Vector2(centerP.x - EdgeDelta, centerP.y + EdgeDelta));
                }
            }
        }

        public static bool Intersects(this LineSegment a, LineSegment b)
        {
            int Orientation(Vector2 p, Vector2 q, Vector2 r)
            {
                float val = (q.y - p.y) * (r.x - q.x) -
                            (q.x - p.x) * (r.y - q.y);
                if (Mathf.Abs(val) == 0) return 0; // colinear
                return (val > 0) ? 1 : 2; // clockwise or counterclockwise
            }

            bool OnSegment(Vector2 p, Vector2 q, Vector2 r)
            {
                return Mathf.Min(p.x, q.x) <= r.x && r.x <= Mathf.Max(p.x, q.x) &&
                       Mathf.Min(p.y, q.y) <= r.y && r.y <= Mathf.Max(p.y, q.y);
            }

            Vector2 p1 = a.Start, p2 = a.End;
            Vector2 q1 = b.Start, q2 = b.End;

            int o1 = Orientation(p1, p2, q1);
            int o2 = Orientation(p1, p2, q2);
            int o3 = Orientation(q1, q2, p1);
            int o4 = Orientation(q1, q2, p2);

            if (o1 != o2 && o3 != o4) return true;

            if (o1 == 0 && OnSegment(p1, p2, q1)) return true;
            if (o2 == 0 && OnSegment(p1, p2, q2)) return true;
            if (o3 == 0 && OnSegment(q1, q2, p1)) return true;
            if (o4 == 0 && OnSegment(q1, q2, p2)) return true;

            return false;
        }
    }

}