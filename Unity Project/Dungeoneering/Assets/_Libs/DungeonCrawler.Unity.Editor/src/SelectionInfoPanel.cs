
using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class SelectionInfoPanel : MonoBehaviour
    {
        [SerializeField]
        private DungeonCrawlerData _dungeonCrawlerData;
        private static readonly Facing[] Facings = { Facing.North, Facing.East, Facing.South, Facing.West };
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
        private readonly HashSet<(Position, Facing)> _walls = new();
        private readonly HashSet<(Position, Facing)> _doors = new();
        private readonly HashSet<(Position, Facing)> _secretDoors = new();

        private void HandleTilesChanged(TilesChangedData _) => RenderInfo(_selection.Tiles, _selection.Walls);

        void OnEnable()
        {
            _dungeonCrawlerData.DungeonData.OnTilesChanged.AddListener(HandleTilesChanged);
            _selection.AddListener(HandleSelectionChanged);
            RenderInfo(_selection.Tiles, _selection.Walls);
            _tilesLabel.Button.OnClick.AddListener(OpenTileTextureSelector);
            _wallsLabel.Button.OnClick.AddListener(OpenWallTextureSelector);
            _doorsLabel.Button.OnClick.AddListener(OpenDoorsTextureSelector);
            _secretDoorLabel.Button.OnClick.AddListener(OpenSecretDoorsTextureSelector);
        }

        void OnDisable()
        {
            _dungeonCrawlerData.DungeonData.OnTilesChanged.RemoveListener(HandleTilesChanged);
            _selection.RemoveListener(HandleSelectionChanged);
            _tilesLabel.Button.OnClick.RemoveListener(OpenTileTextureSelector);
            _wallsLabel.Button.OnClick.RemoveListener(OpenWallTextureSelector);
            _doorsLabel.Button.OnClick.RemoveListener(OpenDoorsTextureSelector);
            _secretDoorLabel.Button.OnClick.RemoveListener(OpenSecretDoorsTextureSelector);
        }

        void Awake()
        {
            Assertion.NotNull(this, _selection, _undoRedoStack, TextureSelector, _content, _tilesLabel, _wallsLabel, _doorsLabel, _secretDoorLabel, _dungeonCrawlerData);
        }

        private void HandleSelectionChanged(SelectionChangedData changes) => RenderInfo(changes.SelectedTiles, changes.SelectedWalls);

        private void RenderInfo(ISet<DungeonTile> tiles, ISet<DungeonWallController> walls)
        {
            CountWalls(tiles, walls);
            _content.SetActive(true);

            (string tileTextureName, UnityEngine.Texture tileTexture) = TextureLabel(tiles);
            _tilesLabel.Label.text = $"{tiles.Count()} Tiles: {tileTextureName}";
            _tilesLabel.Button.Image.texture = tileTexture;

            UpdateLabel(_wallsLabel, "Walls", _walls);
            UpdateLabel(_doorsLabel, "Doors", _doors);
            UpdateLabel(_secretDoorLabel, "Secret Doors", _secretDoors);
        }

        private void UpdateLabel(TextureLabelController label, string name, ISet<(Position, Facing)> walls)
        {
            (string wallTextureName, UnityEngine.Texture wallTexture) = TextureLabel(walls);
            label.Label.text = $"{walls.Count} {name}: {wallTextureName}";
            label.Button.Image.texture = wallTexture;
        }

        private (string, UnityEngine.Texture) TextureLabel(ISet<(Position p, Facing f)> walls)
        {
            if (walls.Count() < 1) { return ("No Selection", null); }
            string textureName = GetTextureName(walls.First());
            if (walls.All(w => GetTextureName(w) == textureName))
            {
                return (textureName, _dungeonCrawlerData.MaterialCache.GetTexture(textureName).Material.Unselected.mainTexture);
            }
            return ("Multiple textures", null);
            string GetTextureName((Position p, Facing f) wall) => _dungeonCrawlerData.DungeonData.GetWallTextureName(wall.p, wall.f);
        }

        private (string, UnityEngine.Texture) TextureLabel(ISet<DungeonTile> tiles)
        {
            if (tiles.Count() < 1) { return ("No Selection", null); }
            TextureReference textureRef = _dungeonCrawlerData.MaterialCache.GetTexture(tiles.First().TileReference);
            if (tiles.All(t => _dungeonCrawlerData.MaterialCache.GetTexture(t.TileReference) == textureRef))
            {
                return (textureRef.TextureName, textureRef.Material.Unselected.mainTexture);
            }
            return ("Multiple textures", null);
        }

        private void CountWalls(ISet<DungeonTile> tiles, ISet<DungeonWallController> walls)
        {
            _walls.Clear();
            _doors.Clear();
            _secretDoors.Clear();
            foreach (var wall in walls)
            {
                HashSet<(Position, Facing)> wallSet = wall.WallType switch
                {
                    WallType.Solid => _walls,
                    WallType.Door => _doors,
                    WallType.SecretDoor => _secretDoors,
                    _ => null,
                };
                if (wallSet == null) { continue; }
                wallSet.Add((wall.Parent.Position, wall.Facing));
            }
            foreach (var tile in tiles)
            {
                foreach (var facing in Facings)
                {
                    (Position p, Facing f) key = (tile.Position, facing);
                    WallType wallType = tile.Dungeon.Walls.GetWall(key.p, key.f);
                    if (wallType == WallType.Solid) { _walls.Add((key.p, key.f)); }
                    else if (wallType == WallType.Door) { _doors.Add((key.p, key.f)); }
                    else if (wallType == WallType.SecretDoor) { _secretDoors.Add((key.p, key.f)); }
                }
            }
        }

        private void SetTileTexture(TextureId newTexture)
        {
            if (!_selection.Tiles.Any()) { return; }
            System.Action perform = default;
            System.Action undo = default;
            foreach (DungeonTile tile in _selection.Tiles)
            {
                Dungeon d = tile.Dungeon;
                Position p = tile.Position;
                TextureId originalTexture = _dungeonCrawlerData.DungeonData.GetFloorTexture(p);
                perform += () => _dungeonCrawlerData.DungeonData.SetFloorTexture(p, newTexture);
                undo += () => _dungeonCrawlerData.DungeonData.SetFloorTexture(p, originalTexture);
            }
            _undoRedoStack.PerformEdit("Set Multiple Textures", perform, undo, _dungeonCrawlerData.DungeonData);
        }

        private void SetSolidTextures(TextureId tId) => SetWallTextures(tId, _walls);
        private void SetDoorTextures(TextureId tId) => SetWallTextures(tId, _doors);
        private void SetSecretTextures(TextureId tId) => SetWallTextures(tId, _secretDoors);

        private void SetWallTextures(TextureId newTexture, HashSet<(Position, Facing)> walls)
        {
            DungeonManifestData manifest = _dungeonCrawlerData.ManifestData;
            Dungeon d = _dungeonCrawlerData.DungeonData.Dungeon;
            System.Action perform = default;
            System.Action undo = default;
            foreach ((Position p, Facing f) in walls)
            {
                TextureId originalTexture = _dungeonCrawlerData.DungeonData.GetWallTexture(p, f);
                perform += () => _dungeonCrawlerData.DungeonData.SetWallTexture(p, f, newTexture);
                undo += () => _dungeonCrawlerData.DungeonData.SetWallTexture(p, f, originalTexture);
            }
            _undoRedoStack.PerformEdit("Set Multiple Wall Textures", perform, undo, _dungeonCrawlerData.DungeonData);
        }

        private void OpenTileTextureSelector(DungeonTextureButton _) => TextureSelector.ShowDialogue(SetTileTexture, null);
        private void OpenWallTextureSelector(DungeonTextureButton _) => TextureSelector.ShowDialogue(SetSolidTextures, null);
        private void OpenDoorsTextureSelector(DungeonTextureButton _) => TextureSelector.ShowDialogue(SetDoorTextures, null);
        private void OpenSecretDoorsTextureSelector(DungeonTextureButton _) => TextureSelector.ShowDialogue(SetSecretTextures, null);
    }
}