#if UNITY_WEBGL
#pragma warning disable IDE0005 // Using directive is unnecessary.
using System.Runtime.InteropServices;

using CaptainCoder.Unity.Assertions;
using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity;


#pragma warning restore IDE0005 // Using directive is unnecessary.
#endif

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using System.Linq;

using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using CaptainCoder.Dungeoneering.DungeonMap;

using System.Collections.Generic;

namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    [RequireComponent(typeof(TextureSelectorPanel))]
    public class TileTextureSelectorPanel : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private UndoRedoStackData _undoRedoStackData;
        [AssertIsSet][SerializeField] private DungeonCrawlerData _dungeonCrawlerData;
        [AssertIsSet][SerializeField] private DungeonEditorSelectionData _selectionData;
        [AssertIsSet][SerializeField] private RawImage _currentTexture;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _currentTextureName;
        [AssertIsSet][SerializeField] private RawImage _defaultTexture;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _selectedText;
        [SerializeField] private Texture2D _multipleTexturesImage;
        private TextureSelectorPanel _textureSelectorPanel;
        private TextureSelectorPanel TextureSelectorPanel => _textureSelectorPanel ??= GetComponent<TextureSelectorPanel>();
        private System.Action<TextureReference> _onSelected;
        private System.Action _useDefault;

        public void ShowTileSelection(System.Action<TextureReference> onSelected)
        {
            _onSelected = onSelected;
            _selectedText.text = $"Tiles Selected: {_selectionData.Tiles.Count}";
            _currentTextureName.text = _selectionData.Tiles.Count > 0 ? "Multiple Selected" : "None Selected";
            _currentTexture.texture = _multipleTexturesImage;
            if (_selectionData.Tiles.Count > 0)
            {
                string name = _selectionData.Tiles.First().FloorTextureName;
                if (_selectionData.Tiles.All(t => t.FloorTextureName == name))
                {
                    _currentTextureName.text = name;
                    _currentTexture.texture = _dungeonCrawlerData.MaterialCache.GetTexture(name).Texture;
                }
            }

            _defaultTexture.texture = _dungeonCrawlerData.MaterialCache.GetTexture(_dungeonCrawlerData.CurrentDungeon.Dungeon.TileTextures.Default).Texture;
            _useDefault = UseDefaultTileTexture;
            TextureSelectorPanel.ShowDialogue(onSelected);
        }

        private static string TypeLabel(WallType wallType) => wallType switch
        {
            WallType.Solid => "Walls",
            WallType.Door => "Doors",
            WallType.SecretDoor => "Secrets",
            _ => throw new System.Exception($"Unexpected {nameof(WallType)}: {wallType}"),
        };

        private static string DefaultTexture(Dungeon dungeon, WallType wallType) => wallType switch
        {
            WallType.Solid => dungeon.WallTextures.DefaultSolid,
            WallType.Door => dungeon.WallTextures.DefaultDoor,
            WallType.SecretDoor => dungeon.WallTextures.DefaultSecretDoor,
            _ => throw new System.Exception($" Unexpected {nameof(WallType)}: {wallType}"),
        };

        public void ShowWallSelection(System.Action<TextureReference> onSelected, WallType wallType, ISet<(Position position, Facing facing)> selection)
        {
            WallReference[] walls = selection.Select(s => new WallReference(_dungeonCrawlerData.CurrentDungeon.Dungeon, s.position, s.facing)).ToArray();
            _onSelected = onSelected;
            _selectedText.text = $"{TypeLabel(wallType)} Selected: {walls.Length}";
            _currentTextureName.text = walls.Length > 0 ? "Multiple Selected" : "None Selected";
            _currentTexture.texture = _multipleTexturesImage;
            if (walls.Length > 0)
            {
                TextureReference textureReference = _dungeonCrawlerData.MaterialCache.GetTexture(walls[0]);
                if (walls.All(t => _dungeonCrawlerData.MaterialCache.GetTexture(t) == textureReference))
                {
                    _currentTextureName.text = textureReference.TextureName;
                    _currentTexture.texture = textureReference.Texture;
                }
            }

            _defaultTexture.texture = _dungeonCrawlerData.MaterialCache.GetTexture(DefaultTexture(_dungeonCrawlerData.CurrentDungeon.Dungeon, wallType)).Texture;
            _useDefault = () => UseDefaultWallTexture(walls);
            TextureSelectorPanel.ShowDialogue(onSelected);
        }

        public void UseDefault() => _useDefault.Invoke();

        private void UseDefaultWallTexture(IEnumerable<WallReference> walls)
        {
            gameObject.SetActive(false);
            if (!walls.Any()) { return; }
            void Perform()
            {
                foreach (WallReference wall in walls)
                {
                    _dungeonCrawlerData.UseDefaultTexture(wall);
                }
            }
            _undoRedoStackData.PerformEditSerializeState("Set Multiple Textures", Perform, _dungeonCrawlerData);
        }

        private void UseDefaultTileTexture()
        {
            gameObject.SetActive(false);
            if (_selectionData.Tiles.Count == 0) { return; }
            var selected = _selectionData.Tiles.ToArray();
            void Perform()
            {
                foreach (DungeonTile tile in selected)
                {
                    _dungeonCrawlerData.UseDefaultTexture(new TileReference(_dungeonCrawlerData.CurrentDungeon.Dungeon, tile.Position));
                }
            }
            _undoRedoStackData.PerformEditSerializeState("Set Multiple Textures", Perform, _dungeonCrawlerData);
        }

    }
}