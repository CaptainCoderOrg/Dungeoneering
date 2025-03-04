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
        private MaterialCacheData _materialCacheData;
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
            Assertion.NotNull(this, _textureImage, _textureNameLabel, _textureInfoLabel, _confirmPromptPanel, _materialCacheData);
        }

        public void PromptDelete()
        {
            _confirmPromptPanel.Prompt($"Are you sure you want to delete this texture? It has {_texture.Count} references in the project.\n<color=red>This cannot be undone</color>", DeleteTexture);
        }

        private void DeleteTexture()
        {
            _materialCacheData.Cache.RemoveTextureReference(_texture);
            Hide();
        }

        public void Toggle() => gameObject.SetActive(!gameObject.activeSelf);
        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
    }
}