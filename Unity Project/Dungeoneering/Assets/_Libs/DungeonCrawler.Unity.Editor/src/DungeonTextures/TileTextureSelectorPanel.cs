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
        private TextureReference _defaultTextureReference;
        private System.Action<TextureReference> _onSelected;

        public void ShowTileSelect(System.Action<TextureReference> onSelected)
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

            _defaultTextureReference = _dungeonCrawlerData.MaterialCache.GetTexture(_dungeonCrawlerData.CurrentDungeon.Dungeon.TileTextures.Default);
            _defaultTexture.texture = _defaultTextureReference.Texture;

            TextureSelectorPanel.ShowDialogue(onSelected);
        }

        public void UseDefault()
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

        // private void SetTileTexture(TextureReference newTexture)
        // {
        //     if (!_selection.Tiles.Any()) { return; }
        //     System.Action perform = default;
        //     System.Action undo = default;
        //     foreach (DungeonTile tile in _selection.Tiles)
        //     {
        //         Dungeon d = tile.Dungeon;
        //         Position p = tile.Position;
        //         TextureReference originalTexture = _dungeonCrawlerData.CurrentDungeon.GetTexture(p);
        //         perform += () => _dungeonCrawlerData.CurrentDungeon.SetTexture(p, newTexture);
        //         undo += () => _dungeonCrawlerData.CurrentDungeon.SetTexture(p, originalTexture);
        //     }
        //     _undoRedoStack.PerformEdit("Set Multiple Textures", perform, undo, _dungeonCrawlerData.CurrentDungeon);
        // }

    }
}