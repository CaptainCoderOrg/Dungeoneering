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
        [field: SerializeField]
        public DungeonEditorSelectionData SelectionData { get; private set; }
        [field: SerializeField]
        public float DoubleClickTime { get; private set; } = 0.2f;
        [SerializeField]
        private DungeonController _dungeonController;
        private float _lastClick;
        private DungeonTile _lastClicked;

        public void Click(DungeonTile clicked)
        {
            float time = Time.time;
            if (_lastClicked == clicked && time - _lastClick < DoubleClickTime)
            {
                SelectionData.SetWallSelection();
                SelectionData.SetTileSelection(SelectRoom(clicked));
            }
            else if (Keyboard.current.shiftKey.isPressed)
            {
                SelectionData.ToggleTileSelected(clicked);
            }
            else
            {
                SelectionData.SetWallSelection();
                SelectionData.SetTileSelection(clicked);
            }
            _lastClick = time;
            _lastClicked = clicked;
        }

        public void Click(DungeonWallController clicked)
        {
            if (Keyboard.current.shiftKey.isPressed)
            {
                SelectionData.ToggleWallSelected(clicked);
            }
            else
            {
                SelectionData.SetTileSelection();
                SelectionData.SetWallSelection(clicked);
            }

        }

        private IEnumerable<DungeonTile> SelectRoom(DungeonTile start)
        {
            int maxSelect = 1000;
            Facing[] facings = { Facing.North, Facing.East, Facing.South, Facing.West };
            Dungeon d = start.Dungeon;
            HashSet<Position> allTiles = new() { start.Position };
            Queue<Position> queue = new();
            queue.Enqueue(start.Position);
            while (queue.TryDequeue(out Position position) && allTiles.Count < maxSelect)
            {
                var neighbors = facings
                    .Where(f => d.Walls.GetWall(position, f) == WallType.None)
                    .Select(position.Step);

                foreach (Position neighbor in neighbors)
                {
                    if (allTiles.Contains(neighbor)) { continue; }
                    allTiles.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
            return allTiles.Select(_dungeonController.GetDungeonTile);
        }
    }
}