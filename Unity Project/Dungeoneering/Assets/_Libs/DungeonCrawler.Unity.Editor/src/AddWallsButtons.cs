using System.Collections.Generic;
using System.Linq;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class AddWallsButtons : MonoBehaviour
    {
        [SerializeField]
        public DungeonEditorSelectionData _selectionData;
        [SerializeField]
        public UndoRedoStackData _undoRedoStackData;

        [field: SerializeField]
        public Button NorthButton { get; private set; }
        [field: SerializeField]
        public Button EastButotn { get; private set; }
        [field: SerializeField]
        public Button SouthButton { get; private set; }
        [field: SerializeField]
        public Button WestButton { get; private set; }
        [field: SerializeField]
        public Button AllButton { get; private set; }

        void Awake()
        {
            Debug.Assert(_selectionData != null, this);
            Debug.Assert(_undoRedoStackData != null, this);
            Debug.Assert(NorthButton != null, this);
            Debug.Assert(EastButotn != null, this);
            Debug.Assert(SouthButton != null, this);
            Debug.Assert(WestButton != null, this);
            Debug.Assert(AllButton != null, this);
        }
        
        void OnEnable()
        {
            NorthButton.onClick.AddListener(SetNorth);
            EastButotn.onClick.AddListener(SetEast);
            SouthButton.onClick.AddListener(SetSouth);
            WestButton.onClick.AddListener(SetWest);
            AllButton.onClick.AddListener(SetAll);
        }

        void OnDisable()
        {
            NorthButton.onClick.RemoveListener(SetNorth);
            EastButotn.onClick.RemoveListener(SetEast);
            SouthButton.onClick.RemoveListener(SetSouth);
            WestButton.onClick.RemoveListener(SetWest);
            AllButton.onClick.RemoveListener(SetAll);
        }

        private void SetNorth() => SetWall(Facing.North);
        private void SetEast() => SetWall(Facing.East);
        private void SetSouth() => SetWall(Facing.South);
        private void SetWest() => SetWall(Facing.West);
        private void SetAll() => SetWall(Facing.North, Facing.East, Facing.South, Facing.West);

        private void SetWall(params Facing[] facings)
        {
            if (_selectionData.SelectedTiles.Count() == 0) { return; }
            HashSet<Position> tiles = _selectionData.SelectedTiles.Select(t => t.Position).ToHashSet();
            System.Action perform = default;
            System.Action undo = default;

            foreach (Facing facing in facings)
            {
                foreach (DungeonTile tile in _selectionData.SelectedTiles)
                {
                    Position neighbor = tile.Position.Step(facing);
                    if (tiles.Contains(neighbor)) { continue; }
                    Dungeon d = tile.Dungeon;
                    Position p = tile.Position;
                    WallType originalWallType = d.Walls[p, facing];
                    if (originalWallType != WallType.None) { continue; }
                    perform += () => d.Walls[p, facing] = WallType.Solid;
                    undo += () => d.Walls[p, facing] = originalWallType;
                }
            }

            _undoRedoStackData.PerformEdit($"Set Wall", perform, undo);
        }

    }
}