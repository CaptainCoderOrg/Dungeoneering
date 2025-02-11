
using System;
using System.Collections.Generic;
using System.Linq;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using CaptainCoder.Unity;
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
        private TextureLabelController _tilesLabel;
        [SerializeField]
        private TextureLabelController _wallsLabel;
        [SerializeField]
        private TextureLabelController _doorsLabel;
        [SerializeField]
        private TextureLabelController _secretDoorLabel;
        private HashSet<(Position, Facing)> _walls = new();
        private HashSet<(Position, Facing)> _doors = new();
        private HashSet<(Position, Facing)> _secretDoors = new();
        private DungeonController _dungeonController;

        void OnEnable()
        {
            _selection.AddListener(HandleTilesChanged);
            _tilesLabel.Button.OnClick.AddListener(OpenTileTextureSelector);
            _wallsLabel.Button.OnClick.AddListener(OpenWallTextureSelector);
            _doorsLabel.Button.OnClick.AddListener(OpenDoorsTextureSelector);
            _secretDoorLabel.Button.OnClick.AddListener(OpenSecretDoorsTextureSelector);
        }


        void OnDisable()
        {
            _selection.RemoveListener(HandleTilesChanged);
            _tilesLabel.Button.OnClick.RemoveListener(OpenTileTextureSelector);
            _wallsLabel.Button.OnClick.RemoveListener(OpenWallTextureSelector);
            _doorsLabel.Button.OnClick.RemoveListener(OpenDoorsTextureSelector);
            _secretDoorLabel.Button.OnClick.RemoveListener(OpenSecretDoorsTextureSelector);
        }

        void Awake()
        {
            Assertion.NotNull(this, _selection, _undoRedoStack, TextureSelector, _content, _tilesLabel, _wallsLabel, _doorsLabel, _secretDoorLabel);
        }

        private void HandleTilesChanged(IEnumerable<DungeonTile> tiles)
        {
            if (tiles.Count() <= 1)
            {
                _content.SetActive(false);
                return;
            }
            _dungeonController = tiles.First().DungeonController;
            CountWalls(tiles);
            _content.SetActive(true);
            
            (string tileTextureName, UnityEngine.Texture tileTexture) = TextureLabel(tiles);
            _tilesLabel.Label.text = $"{tiles.Count()} Tiles: {tileTextureName}";
            _tilesLabel.Button.Image.texture = tileTexture;

            UpdateLabel(_wallsLabel, "Walls", _walls);
            UpdateLabel(_doorsLabel, "Doors", _doors);
            UpdateLabel(_secretDoorLabel, "Secret Doors", _secretDoors);
        }

        private void UpdateLabel(TextureLabelController label, string name, HashSet<(Position, Facing)> walls)
        {
            (string wallTextureName, UnityEngine.Texture wallTexture) = TextureLabel(walls);
            label.Label.text = $"{walls.Count} {name}: {wallTextureName}";
            label.Button.Image.texture = wallTexture;
        }

        private (string, UnityEngine.Texture) TextureLabel(HashSet<(Position p, Facing f)> walls)
        {
            if (walls.Count() < 1) { return ("No Selection", null); }
            string textureName = GetTextureName(walls.First());
            if (walls.All(w => GetTextureName(w) == textureName))
            {
                return (textureName, _dungeonController.ManifestData.MaterialCache[textureName].mainTexture);
            }
            return ("Multiple textures", null);
            string GetTextureName((Position p, Facing f) wall) => _dungeonController.ManifestData.GetWallTexture(_dungeonController.DungeonData.Dungeon, wall.p, wall.f);
        }

        private (string, UnityEngine.Texture) TextureLabel(IEnumerable<DungeonTile> tiles)
        {
            if (tiles.Count() < 1) { return ("No Selection", null); }
            string textureName = tiles.First().FloorTextureName;
            if (tiles.All(t => t.FloorTextureName == textureName))
            {
                return (textureName, tiles.First().FloorTile.material.mainTexture);
            }
            return ("Multiple textures", null);
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


        private void SetTileTexture(string newTexture)
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

        private void SetSolidTextures(string textureName) =>  SetWallTextures(textureName, _walls);
        private void SetDoorTextures(string textureName) => SetWallTextures(textureName, _doors); 
        private void SetSecretTextures(string textureName) => SetWallTextures(textureName, _secretDoors); 

        private void SetWallTextures(string newTexture, HashSet<(Position, Facing)> walls)
        {
            DungeonManifestData manifest = _dungeonController.ManifestData;
            Dungeon d = _dungeonController.DungeonData.Dungeon;
            System.Action perform = default;
            System.Action undo = default;
            foreach ((Position p, Facing f) in walls)
            {
                string originalTexture = manifest.GetWallTexture(d, p, f);
                perform += () => manifest.SetWallTexture(d, p, f, newTexture);
                undo += () => manifest.SetWallTexture(d, p, f, originalTexture);
            }
            _undoRedoStack.PerformEdit("Set Multiple Wall Textures", perform, undo);
        }

        private void OpenTileTextureSelector(DungeonTextureButton _) => TextureSelector.ShowDialogue(SetTileTexture, null);
        private void OpenWallTextureSelector(DungeonTextureButton _) => TextureSelector.ShowDialogue(SetSolidTextures, null);
        private void OpenDoorsTextureSelector(DungeonTextureButton _) => TextureSelector.ShowDialogue(SetDoorTextures, null);
        private void OpenSecretDoorsTextureSelector(DungeonTextureButton _) => TextureSelector.ShowDialogue(SetSecretTextures, null);
    }
}