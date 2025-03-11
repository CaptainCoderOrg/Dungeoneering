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
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    [RequireComponent(typeof(TextureSelectorPanel))]
    public class TileTextureSelectorPanel : MonoBehaviour
    {

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
            _onSelected?.Invoke(_defaultTextureReference);
        }

    }
}