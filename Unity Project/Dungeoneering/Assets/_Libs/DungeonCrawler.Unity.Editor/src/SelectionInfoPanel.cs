
using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity;
using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class SelectionInfoPanel : MonoBehaviour
    {
        private static readonly Facing[] Facings = { Facing.North, Facing.East, Facing.South, Facing.West };
        [AssertIsSet][SerializeField] private DungeonCrawlerData _dungeonCrawlerData;
        [AssertIsSet][SerializeField] private DungeonEditorSelectionData _selection;
        [AssertIsSet][SerializeField] private UndoRedoStackData _undoRedoStack;
        [AssertIsSet][SerializeField] private TileTextureSelectorPanel _tileTextureSelector;
        [AssertIsSet][SerializeField] private GameObject _content;
        [AssertIsSet][SerializeField] private TextureLabelController _tilesLabel;
        [AssertIsSet][SerializeField] private TextureLabelController _wallsLabel;
        [AssertIsSet][SerializeField] private TextureLabelController _doorsLabel;
        [AssertIsSet][SerializeField] private TextureLabelController _secretDoorLabel;
        private WallSelectionData _wallSelectionData = new();

        private void HandleTilesChanged(TilesChangedData _) => RenderInfo(_selection.Tiles, _selection.Walls);

        void OnEnable()
        {
            _dungeonCrawlerData.CurrentDungeon.OnTilesChanged.AddListener(HandleTilesChanged);
            _selection.AddListener(HandleSelectionChanged);
            RenderInfo(_selection.Tiles, _selection.Walls);
        }

        void OnDisable()
        {
            _dungeonCrawlerData.CurrentDungeon.OnTilesChanged.RemoveListener(HandleTilesChanged);
            _selection.RemoveListener(HandleSelectionChanged);
        }

        private void HandleSelectionChanged(SelectionChangedData changes) => RenderInfo(changes.SelectedTiles, changes.SelectedWalls);

        private void RenderInfo(ISet<DungeonTile> tiles, ISet<DungeonWallController> walls)
        {
            _wallSelectionData.CountWalls(tiles, walls);
            _content.SetActive(true);

            (string tileTextureName, TextureReference tileTexture) = TextureLabel(tiles);
            _tilesLabel.Label.text = $"{tiles.Count()} Tiles: {tileTextureName}";
            _tilesLabel.Button.Texture = tileTexture;

            UpdateLabel(_wallsLabel, "Walls", _wallSelectionData.Solid);
            UpdateLabel(_doorsLabel, "Doors", _wallSelectionData.Doors);
            UpdateLabel(_secretDoorLabel, "Secret Doors", _wallSelectionData.SecretDoors);
        }

        private void UpdateLabel(TextureLabelController label, string name, ISet<(Position, Facing)> walls)
        {
            (string wallTextureName, TextureReference wallTexture) = TextureLabel(walls);
            label.Label.text = $"{walls.Count} {name}: {wallTextureName}";
            label.Button.Texture = wallTexture;
        }

        private (string, TextureReference) TextureLabel(ISet<(Position p, Facing f)> walls)
        {
            if (walls.Count() < 1) { return ("No Selection", null); }
            string textureName = GetTextureName(walls.First());
            if (walls.All(w => GetTextureName(w) == textureName))
            {
                return (textureName, _dungeonCrawlerData.MaterialCache.GetTexture(textureName));
            }
            return ("Multiple textures", null);
            string GetTextureName((Position p, Facing f) wall) => _dungeonCrawlerData.CurrentDungeon.GetWallTextureName(wall.p, wall.f);
        }

        private (string, TextureReference) TextureLabel(ISet<DungeonTile> tiles)
        {
            if (tiles.Count() < 1) { return ("No Selection", null); }
            TextureReference textureRef = _dungeonCrawlerData.MaterialCache.GetTexture(tiles.First().TileReference);
            if (tiles.All(t => _dungeonCrawlerData.MaterialCache.GetTexture(t.TileReference) == textureRef))
            {
                return (textureRef.TextureName, textureRef);
            }
            return ("Multiple textures", null);
        }

        private void SetTileTexture(TextureReference newTexture)
        {
            if (!_selection.Tiles.Any()) { return; }
            System.Action perform = default;
            System.Action undo = default;
            foreach (DungeonTile tile in _selection.Tiles)
            {
                Dungeon d = tile.Dungeon;
                Position p = tile.Position;
                TextureReference originalTexture = _dungeonCrawlerData.CurrentDungeon.GetTexture(p);
                perform += () => _dungeonCrawlerData.SetTexture(new TileReference(d, p), newTexture);
                undo += () => _dungeonCrawlerData.SetTexture(new TileReference(d, p), originalTexture);
            }
            _undoRedoStack.PerformEdit("Set Multiple Textures", perform, undo, _dungeonCrawlerData.CurrentDungeon);
        }

        private void SetSolidTextures(TextureReference newTexture) => SetWallTextures(newTexture, _wallSelectionData.Solid);
        private void SetDoorTextures(TextureReference newTexture) => SetWallTextures(newTexture, _wallSelectionData.Doors);
        private void SetSecretTextures(TextureReference newTexture) => SetWallTextures(newTexture, _wallSelectionData.SecretDoors);

        private void SetWallTextures(TextureReference newTexture, ISet<(Position, Facing)> walls)
        {
            Dungeon d = _dungeonCrawlerData.CurrentDungeon.Dungeon;
            void Perform()
            {
                foreach ((Position p, Facing f) in walls)
                {
                    TextureReference originalTexture = _dungeonCrawlerData.CurrentDungeon.GetTexture(p, f);
                    _dungeonCrawlerData.SetTexture(new WallReference(d, p, f), newTexture);
                }
            }
            _undoRedoStack.PerformEditSerializeState("Set Multiple Wall Textures", Perform, _dungeonCrawlerData);
        }

        public void OpenTileTextureSelector() => _tileTextureSelector.ShowTileSelection(SetTileTexture);
        public void OpenWallTextureSelector() => _tileTextureSelector.ShowWallSelection(SetSolidTextures, WallType.Solid, _wallSelectionData.Solid);
        public void OpenDoorsTextureSelector() => _tileTextureSelector.ShowWallSelection(SetDoorTextures, WallType.Door, _wallSelectionData.Doors);
        public void OpenSecretDoorsTextureSelector() => _tileTextureSelector.ShowWallSelection(SetSecretTextures, WallType.SecretDoor, _wallSelectionData.SecretDoors);
    }
}