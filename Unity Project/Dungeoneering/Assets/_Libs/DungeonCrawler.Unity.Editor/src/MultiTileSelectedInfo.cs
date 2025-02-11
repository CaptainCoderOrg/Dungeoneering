using System;
using System.Collections.Generic;
using System.Linq;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using CaptainCoder.Unity;
using TMPro;
using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class MultiTileSelectedInfo : MonoBehaviour
    {
        [SerializeField]
        private DungeonEditorSelectionData _selection;
        [SerializeField]
        private UndoRedoStackData _undoRedoStack;
        [field: SerializeField]
        public DungeonTextureSelectorController TextureSelector { get; private set; }
        [SerializeField]
        private GameObject _content;
        [SerializeField]
        private TextMeshProUGUI _tileCountText;
        [SerializeField]
        private TextMeshProUGUI _wallCountText;
        [SerializeField]
        private TextMeshProUGUI _doorCountText;
        [SerializeField]
        private TextMeshProUGUI _secretDoorCountText;
        [SerializeField]
        private TextureLabelController _tileTexture;
        [SerializeField]
        private TextureLabelController _wallTexture;
        [SerializeField]
        private TextureLabelController _doorTexture;
        [SerializeField]
        private TextureLabelController _secretDoorTexture;
        private HashSet<(Position, Facing)> _walls = new();
        private HashSet<(Position, Facing)> _doors = new();
        private HashSet<(Position, Facing)> _secretDoors = new();

        void OnEnable()
        {
            _selection.AddListener(HandleTilesChanged);
        }

        void OnDisable()
        {
            _selection.RemoveListener(HandleTilesChanged);
        }

        void Awake()
        {
            Assertion.NotNull(this, _selection, _undoRedoStack, TextureSelector, _content, _tileCountText, _wallCountText, _doorCountText, _secretDoorCountText, _tileTexture, _wallTexture, _doorTexture, _secretDoorTexture);
        }

        private void HandleTilesChanged(IEnumerable<DungeonTile> tiles)
        {
            if (tiles.Count() <= 1)
            {
                _content.SetActive(false);
                return;
            }
            CountWalls(tiles);
            _content.SetActive(true);
            _wallCountText.text = $"Walls: {_walls.Count}";
            _tileCountText.text = $"Tiles: {tiles.Count()}";
            _doorCountText.text = $"Doors: {_doors.Count}";
            _secretDoorCountText.text = $"Secrets: {_secretDoors.Count}";

        }

        private void CountWalls(IEnumerable<DungeonTile> tiles)
        {
            Facing[] facings = { Facing.North, Facing.East, Facing.South, Facing.West };
            _walls.Clear();
            _doors.Clear();
            _secretDoors.Clear();
            foreach (var tile in tiles)
            {
                foreach (var facing in facings)
                {
                    (Position p, Facing f) key = (tile.Position, facing);
                    WallType wallType = tile.Dungeon.Walls.GetWall(key.p, key.f);
                    if (wallType == WallType.Solid) { _walls.Add((key.p, key.f)); }
                    else if (wallType == WallType.Door) { _doors.Add((key.p, key.f)); }
                    else if (wallType == WallType.SecretDoor) { _secretDoors.Add((key.p, key.f)); }
                }
            }
        }


        private void MultiSetTexture(string newTexture)
        {
            System.Action perform = default;
            System.Action undo = default;
            foreach (DungeonTile tile in _selection.SelectedTiles)
            {
                DungeonManifestData manifest = tile.Manifest;
                Dungeon d = tile.Dungeon;
                Position p = tile.Position;
                string originalTexture = manifest.GetFloorTexture(d, p);
                perform += () => manifest.SetFloorTexture(d, p, newTexture);
                undo += () => manifest.SetFloorTexture(d, p, originalTexture);
            }
            _undoRedoStack.PerformEdit("Set Multiple Textures", perform, undo);
        }

        private void OpenSelector(DungeonTextureButton button)
        {
            TextureSelector.ShowDialogue(MultiSetTexture, null);
        }
    }
}