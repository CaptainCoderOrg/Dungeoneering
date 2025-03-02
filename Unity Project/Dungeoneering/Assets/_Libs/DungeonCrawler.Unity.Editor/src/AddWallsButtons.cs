using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class AddWallsButtons : MonoBehaviour
    {
        [SerializeField]
        private DungeonEditorSelectionData _selectionData;
        [SerializeField]
        private UndoRedoStackData _undoRedoStackData;

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
        [field: SerializeField]
        public Button NoneButton { get; private set; }

        void Awake()
        {
            Debug.Assert(_selectionData != null, this);
            Debug.Assert(_undoRedoStackData != null, this);
            Debug.Assert(NorthButton != null, this);
            Debug.Assert(EastButotn != null, this);
            Debug.Assert(SouthButton != null, this);
            Debug.Assert(WestButton != null, this);
            Debug.Assert(AllButton != null, this);
            Debug.Assert(NoneButton != null, this);
        }

        void OnEnable()
        {
            NorthButton.onClick.AddListener(SetNorth);
            EastButotn.onClick.AddListener(SetEast);
            SouthButton.onClick.AddListener(SetSouth);
            WestButton.onClick.AddListener(SetWest);
            AllButton.onClick.AddListener(SetAll);
            NoneButton.onClick.AddListener(SetNone);
        }

        void OnDisable()
        {
            NorthButton.onClick.RemoveListener(SetNorth);
            EastButotn.onClick.RemoveListener(SetEast);
            SouthButton.onClick.RemoveListener(SetSouth);
            WestButton.onClick.RemoveListener(SetWest);
            AllButton.onClick.RemoveListener(SetAll);
            NoneButton.onClick.RemoveListener(SetNone);
        }

        private void SetNorth() => SetWall(WallType.Solid, Facing.North);
        private void SetEast() => SetWall(WallType.Solid, Facing.East);
        private void SetSouth() => SetWall(WallType.Solid, Facing.South);
        private void SetWest() => SetWall(WallType.Solid, Facing.West);
        private void SetAll() => SetWall(WallType.Solid, Facing.North, Facing.East, Facing.South, Facing.West);
        private void SetNone() => SetWall(WallType.None, Facing.North, Facing.East, Facing.South, Facing.West);

        private void SetWall(WallType wallType, params Facing[] facings)
        {
            if (_selectionData.Tiles.Count() == 0) { return; }
            HashSet<Position> tiles = _selectionData.Tiles.Select(t => t.Position).ToHashSet();
            DungeonData dungeonData = _selectionData.Tiles.First().DungeonController.DungeonData;
            Dungeon d = dungeonData.Dungeon;
            IEnumerable<WallChangeData> changes = d.FindWallChanges(tiles, wallType, facings);
            WallData[] newWalls = changes.Select(c => c.Changed).ToArray();
            WallData[] originalWalls = changes.Select(c => c.Original).ToArray();
            void Perform() => d.SetWalls(newWalls);
            void Undo() => d.SetWalls(originalWalls);
            _undoRedoStackData.PerformEdit($"Set Wall", Perform, Undo, dungeonData);
        }

    }
}