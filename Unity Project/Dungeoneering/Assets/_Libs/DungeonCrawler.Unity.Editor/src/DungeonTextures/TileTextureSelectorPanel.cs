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
            _useDefault = UseDefaultTile;
            TextureSelectorPanel.ShowDialogue(onSelected);
        }

        public void ShowWallSelection(System.Action<TextureReference> onSelected)
        {
            HashSet<DungeonWallController> selection = _selectionData.Walls.Where(w => w.WallType == WallType.Solid).ToHashSet();
            _onSelected = onSelected;
            _selectedText.text = $"Walls Selected: {selection.Count}";
            _currentTextureName.text = _selectionData.Tiles.Count > 0 ? "Multiple Selected" : "None Selected";
            _currentTexture.texture = _multipleTexturesImage;
            if (selection.Count > 0)
            {
                TextureReference textureReference = selection.First().Texture;
                if (selection.All(t => t.Texture == textureReference))
                {
                    _currentTextureName.text = textureReference.TextureName;
                    _currentTexture.texture = textureReference.Texture;
                }
            }

            _defaultTexture.texture = _dungeonCrawlerData.MaterialCache.GetTexture(_dungeonCrawlerData.CurrentDungeon.Dungeon.WallTextures.DefaultSolid).Texture;
            _useDefault = UseDefaultWall;
            TextureSelectorPanel.ShowDialogue(onSelected);
        }

        public void UseDefault() => _useDefault.Invoke();

        private void UseDefaultWall()
        {
            gameObject.SetActive(false);
            DungeonWallController[] selection = _selectionData.Walls.Where(w => w.WallType == WallType.Solid).ToArray();
            if (selection.Length == 0) { return; }
            void Perform()
            {
                foreach (DungeonWallController wall in selection)
                {
                    _dungeonCrawlerData.CurrentDungeon.RemoveWallTexture(wall.Parent.Position, wall.Facing);
                }
            }
            _undoRedoStackData.PerformEditSerializeState("Set Multiple Textures", Perform, _dungeonCrawlerData);
        }

        private void UseDefaultTile()
        {
            gameObject.SetActive(false);
            if (_selectionData.Tiles.Count == 0) { return; }
            var selected = _selectionData.Tiles.ToArray();
            void Perform()
            {
                foreach (DungeonTile tile in selected)
                {
                    _dungeonCrawlerData.CurrentDungeon.RemoveFloorTexture(tile.Position);
                }
            }
            _undoRedoStackData.PerformEditSerializeState("Set Multiple Textures", Perform, _dungeonCrawlerData);
        }

    }
}