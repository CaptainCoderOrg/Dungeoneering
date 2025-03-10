#if UNITY_WEBGL
#pragma warning disable IDE0005 // Using directive is unnecessary.
using System.Runtime.InteropServices;
#pragma warning restore IDE0005 // Using directive is unnecessary.
#endif

using System.Collections;

using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity;
using CaptainCoder.Unity.UI;

using SFB;

using TMPro;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

using System.IO;

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
        [SerializeField]
        private Button _deleteButton;
        private ConfirmPromptPanel _confirmPromptPanel;
        private ConfirmTextureReplacementPrompt _confirmTexturePrompt;
        private TextureReference _texture;
        public TextureReference Texture
        {
            get => _texture;
            set
            {
                if (_texture != null)
                {
                    _texture.OnTextureChange -= Render;
                }
                _texture = value;
                _texture.OnTextureChange += Render;
                Render(_texture);
            }
        }

        private void Render(TextureReference texture)
        {
            _textureImage.texture = texture.Material.Unselected.mainTexture;
            _textureNameLabel.text = texture.TextureName;
            _textureInfoLabel.text = $"{texture.Count} References";
            _deleteButton.interactable = !texture.IsDefaultTexture;
        }

        void Awake()
        {
            _confirmPromptPanel = GetComponentInChildren<ConfirmPromptPanel>(true);
            Debug.Assert(_confirmPromptPanel != null, "Confirm Prompt not set", gameObject);
            _confirmTexturePrompt = GetComponentInChildren<ConfirmTextureReplacementPrompt>(true);
            Debug.Assert(_confirmTexturePrompt != null, "Confirm Texture Prompt not set", gameObject);
            Assertion.NotNull(this, _textureImage, _textureNameLabel, _textureInfoLabel, _confirmPromptPanel, _dungeonCrawlerData, _undoRedoStack);
        }

        public void PromptDelete()
        {
            _confirmPromptPanel.Prompt($"Are you sure you want to delete this texture? It has {_texture.Count} references in the project.\n<color=red>This cannot be undone</color>", DeleteTexture);
        }

        private IEnumerator HandleLoadTexture(string absoluteUri)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(absoluteUri);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Texture2D newTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                newTexture.filterMode = FilterMode.Point;
                _confirmTexturePrompt.PromptReplacement(_texture, newTexture, () => HandleReplaceTexture(newTexture), null);
            }
        }

        private void HandleReplaceTexture(Texture2D newTexture)
        {
            System.Action perform = () =>
            {
                TextureReference textureRef = _dungeonCrawlerData.MaterialCache.GetTexture(_texture.TextureName);
                _dungeonCrawlerData.ManifestData.UpdateTexture(textureRef, newTexture);
            };
            _undoRedoStack.PerformEditSerializeState("Replace Texture", perform, _dungeonCrawlerData);
        }

        private void DeleteTexture()
        {
            System.Action perform = () =>
            {
                TextureReference textureRef = _dungeonCrawlerData.MaterialCache.GetTexture(_texture.TextureName);
                _dungeonCrawlerData.MaterialCache.RemoveTextureReference(textureRef);
            };
            _undoRedoStack.PerformEditSerializeState("Delete Texture", perform, _dungeonCrawlerData);
            Hide();
        }

#if UNITY_WEBGL && !UNITY_EDITOR
            //
            // WebGL
            //
        [DllImport("__Internal")]
        private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

        // Broser plugin should be called in OnPointerDown.
        public void PromptExport()
        {
            var bytes = _dungeonCrawlerData.ManifestData.Manifest.Textures[_texture.TextureName].Data;
            DownloadFile(gameObject.name, "OnFileDownload", $"{TextureFileNameWithoutExtension}.png", bytes, bytes.Length);
        }

        // Called from browser
        public void OnFileDownload() {
            Debug.Log($"File Exported: {_texture.TextureName}");
        }
#else
        public void PromptExport()
        {
            var path = StandaloneFileBrowser.SaveFilePanel("Title", "", TextureFileNameWithoutExtension, "png");
            if (!string.IsNullOrEmpty(path))
            {
                File.WriteAllBytes(path, _dungeonCrawlerData.ManifestData.Manifest.Textures[_texture.TextureName].Data);
            }
        }
#endif

        private string TextureFileNameWithoutExtension => _texture.TextureName.EndsWith(".png") ? _texture.TextureName[0..^4] : _texture.TextureName;

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void PromptUpdateTexture()
    {
        UploadFile(gameObject.name, "OnFileUpload", ".png", false);
    }

    // Called from browser
    public void OnFileUpload(string url) {
        StartCoroutine(HandleLoadTexture(url));
    }
#else

        public void PromptUpdateTexture()
        {
            ExtensionFilter[] filter = { new("Image", "png") };
            var paths = StandaloneFileBrowser.OpenFilePanel("Select a texture", "", filter, false);
            if (paths.Length > 0)
            {
                StartCoroutine(HandleLoadTexture(new System.Uri(paths[0]).AbsoluteUri));
            }

        }
#endif

        public void Toggle() => gameObject.SetActive(!gameObject.activeSelf);
        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
    }
}