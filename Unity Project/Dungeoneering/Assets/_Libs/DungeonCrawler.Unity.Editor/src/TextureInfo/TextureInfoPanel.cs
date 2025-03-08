using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.IO;
using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity;
using CaptainCoder.Unity.UI;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class TextureInfoPanel : MonoBehaviour
    {
        [SerializeField]
        private UndoRedoStackData _undoRedoStack;
        [SerializeField]
        private DungeonCrawlerData _dungeonCrawlerData;
        [SerializeField]
        private RawImage _textureImage;
        [SerializeField]
        private TextMeshProUGUI _textureNameLabel;
        [SerializeField]
        private TextMeshProUGUI _textureInfoLabel;
        private ConfirmPromptPanel _confirmPromptPanel;
        private TextureReference _texture;
        public TextureReference Texture
        {
            get => _texture;
            set
            {
                if (_texture == value) { return; }
                _texture = value;
                _textureImage.texture = _texture.Material.Unselected.mainTexture;
                _textureNameLabel.text = _texture.TextureName;
                _textureInfoLabel.text = $"{_texture.Count} References";
            }
        }

        void Awake()
        {
            _confirmPromptPanel = GetComponentInChildren<ConfirmPromptPanel>(true);
            Debug.Assert(_confirmPromptPanel != null, "Confirm Prompt not set", gameObject);
            Assertion.NotNull(this, _textureImage, _textureNameLabel, _textureInfoLabel, _confirmPromptPanel, _dungeonCrawlerData, _undoRedoStack);
        }

        public void PromptDelete()
        {
            _confirmPromptPanel.Prompt($"Are you sure you want to delete this texture? It has {_texture.Count} references in the project.\n<color=red>This cannot be undone</color>", DeleteTexture);
        }

        private void DeleteTexture()
        {
            // TODO: Consider tracking changes rather than a full serialize:
            // Reloading the manifest causes all of the texture to be rebuilt which would probably be awful
            // for any non-trivial project size
            string originalDungeonJson = JsonExtensions.ToJson(_dungeonCrawlerData.DungeonData.Dungeon);
            string originalManifestJson = JsonExtensions.ToJson(_dungeonCrawlerData.ManifestData.Manifest);
            System.Action perform = () =>
            {
                TextureReference textureRef = _dungeonCrawlerData.MaterialCache.GetTexture(_texture.TextureName);
                _dungeonCrawlerData.MaterialCache.RemoveTextureReference(textureRef);
            };
            System.Action undo = () =>
            {
                _dungeonCrawlerData.ManifestData.TryLoadManifest(originalManifestJson, out _);
                _dungeonCrawlerData.DungeonData.Dungeon = JsonExtensions.LoadModel<Dungeon>(originalDungeonJson);
            };
            _undoRedoStack.PerformEdit("Delete Texture", perform, undo, _dungeonCrawlerData.DungeonData);
            Hide();
        }

        public void Toggle() => gameObject.SetActive(!gameObject.activeSelf);
        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
    }
}