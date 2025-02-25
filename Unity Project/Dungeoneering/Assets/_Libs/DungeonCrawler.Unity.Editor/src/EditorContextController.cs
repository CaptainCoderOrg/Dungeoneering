using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;

using UnityEngine;
using UnityEngine.InputSystem;
namespace CaptainCoder.Dungeoneering.Unity
{
    public class EditorContextController : MonoBehaviour
    {
        private const int FACING_ORIENTATION_BIT = 1;
        private const int FACING_DIRECTION_BIT = 2;

        [field: SerializeField]
        public DungeonEditorSelectionData SelectionData { get; private set; }
        [field: SerializeField]
        public float DoubleClickTime { get; private set; } = 0.2f;
        [SerializeField]
        private DungeonController _dungeonController;
        private float _lastClick;
        private DungeonTile _lastClicked;
        private DungeonWallController _lastClickedWall;
        private static readonly Facing[] Facings = { Facing.North, Facing.East, Facing.South, Facing.West };

        public void Click(DungeonTile clicked)
        {
            float time = Time.time;
            bool isDoubleClick = _lastClicked == clicked && time - _lastClick < DoubleClickTime;
            if (isDoubleClick && Keyboard.current.shiftKey.isPressed)
            {
                SelectionData.AddTileSelection(SelectRoom(clicked));
            }
            else if (isDoubleClick)
            {
                SelectionData.SetTileSelection(SelectRoom(clicked));
            }
            else if (Keyboard.current.shiftKey.isPressed)
            {
                SelectionData.ToggleTileSelected(clicked);
            }
            else
            {
                SelectionData.SetTileSelection(clicked);
            }
            _lastClick = time;
            _lastClicked = clicked;
            _lastClickedWall = null;
        }

        public void Click(DungeonWallController clicked)
        {
            float time = Time.time;
            bool isDoubleClick = _lastClickedWall == clicked && time - _lastClick <= DoubleClickTime;
            if (isDoubleClick && Keyboard.current.shiftKey.isPressed)
            {
                SelectionData.AddWallSelection(GetConnectedWalls(clicked));
            }
            else if (isDoubleClick)
            {
                SelectionData.SetWallSelection(GetConnectedWalls(clicked));
            }
            else if (Keyboard.current.shiftKey.isPressed)
            {
                SelectionData.ToggleWallSelected(clicked);
            }
            else
            {
                SelectionData.SetWallSelection(clicked);
            }
            _lastClick = time;
            _lastClicked = null;
            _lastClickedWall = clicked;
        }

        private IEnumerable<DungeonTile> SelectRoom(DungeonTile start)
        {
            int maxSelect = 1000;
            Dungeon d = start.Dungeon;
            HashSet<Position> allTiles = new() { start.Position };
            Queue<Position> queue = new();
            queue.Enqueue(start.Position);
            while (queue.TryDequeue(out Position position) && allTiles.Count < maxSelect)
            {
                var neighbors = Facings
                    .Where(f => d.Walls.GetWall(position, f) == WallType.None)
                    .Select(position.Step)
                    .Where(_dungeonController.HasTile);

                foreach (Position neighbor in neighbors)
                {
                    if (allTiles.Add(neighbor))
                    {
                        queue.Enqueue(neighbor);
                    }
                }
            }
            return allTiles.Select(_dungeonController.GetDungeonTile);
        }

        private (DungeonWallController wall, Facing newIncomingSide) GetNextWall(DungeonWallController wall, Facing incomingSide)
        {
#if UNITY_EDITOR
            Debug.Assert(wall.enabled);
            // For vertical walls, the incoming side should be horizontal, and vice versa
            Debug.Assert(((int)(wall.Facing ^ incomingSide) & FACING_ORIENTATION_BIT) == FACING_ORIENTATION_BIT);
#endif

            DungeonTile tile = wall.Parent;
            if (tile == null)
            {
                Debug.LogError("No parent tile found!");
                return default;
            }

            Facing exitSide = (Facing)((int)incomingSide ^ FACING_DIRECTION_BIT);

            // First check for the next wall as a turn in the current tile
            DungeonWallController candidate = tile[exitSide];

            if (candidate.isActiveAndEnabled)
                return (candidate, wall.Facing);

            // Next check for a matching continuation in the neighboring tile
            Position exitSidePosition = wall.Parent.Position.Step(exitSide);
            if (_dungeonController.TryGetDungeonTile(exitSidePosition, out DungeonTile neighborTile))
            {
                candidate = neighborTile[wall.Facing];
                if (candidate.isActiveAndEnabled)
                    return (candidate, incomingSide);
            }
            else
            {
                // There was no neighbor on the exit side, implying we hit the end of the map,
                // so we can't continue and check for neighbors through the sibling or the
                // invariant that walls are made in sibling pairs and we'll always complete a
                // cycle can't be relied upon
                return default;
            }



            // Next check for a connection through the sibling wall
            if (_dungeonController.TryGetDungeonTile(exitSidePosition.Step(wall.Facing), out neighborTile))
            {
                candidate = neighborTile[incomingSide];
                if (candidate.isActiveAndEnabled)
                    return (candidate, (Facing)((int)wall.Facing ^ FACING_DIRECTION_BIT));
            }

            // Finally, check for this wall's sibling
            if (_dungeonController.TryGetDungeonTile(wall.Parent.Position.Step(wall.Facing), out neighborTile))
            {
                Facing reversedFacing = (Facing)((int)wall.Facing ^ FACING_DIRECTION_BIT);
                candidate = neighborTile[reversedFacing];
                if (candidate.isActiveAndEnabled)
                    return (candidate, exitSide);
            }

            // This appears to be a terminal node, so a wall segment on the map border with no connection
            return default;
        }

        private IEnumerable<DungeonWallController> GetConnectedWalls(DungeonWallController start)
        {
            yield return start;

            Facing initialIncoming = (Facing)((int)start.Facing ^ FACING_ORIENTATION_BIT);
            var (curr, dir) = GetNextWall(start, initialIncoming);
            while (curr != null && curr != start)
            {
                yield return curr;
                (curr, dir) = GetNextWall(curr, dir);
            }

            if (curr != null)
                yield break;

            // If curr became null, that should mean that we ran into a terminal node (or some unknown condition),
            // meaning there's a map edge tile without a wall at the border of the map. If that happens, continue the
            // search in the opposite direction from the start
            Facing reversedInitialIncoming = (Facing)((int)initialIncoming ^ FACING_DIRECTION_BIT);
            (curr, dir) = GetNextWall(start, reversedInitialIncoming);
            while (curr != null && curr != start)
            {
                yield return curr;
                (curr, dir) = GetNextWall(curr, dir);
            }
        }
    }
}