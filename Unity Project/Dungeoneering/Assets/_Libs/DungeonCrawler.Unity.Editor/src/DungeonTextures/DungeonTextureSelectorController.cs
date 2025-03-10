#if UNITY_WEBGL
#pragma warning disable IDE0005 // Using directive is unnecessary.
using System.Runtime.InteropServices;
#pragma warning restore IDE0005 // Using directive is unnecessary.
#endif
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity;
using CaptainCoder.Unity.UI;

using SFB;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

using TMPro;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class DungeonTextureSelectorController : MonoBehaviour
    {
        [SerializeField]
        private DungeonCrawlerData _dungeonCrawlerData;
        [SerializeField]
        private TextureInfoPanel _textureInfoPanel;
        [field: SerializeField]
        public Transform Grid { get; private set; }
        [field: SerializeField]
        public DungeonTexturePreview PreviewPrefab { get; private set; }
        [field: SerializeField]
        public Button AddTextureButton { get; private set; }
        [SerializeField]
        private ConfirmTexturePromptPanel _confirmPanel;
        [SerializeField]
        private ScrollRect _scrollRect;
        [SerializeField]
        private TextMeshProUGUI _headerText;
        private Dictionary<TextureReference, DungeonTexturePreview> _textureButtons = new();
        private System.Action<TextureReference> _onSelectedCallback;
        private System.Action _onCanceledCallback;

        void Awake()
        {
            Assertion.NotNull(this, _confirmPanel, Grid, _dungeonCrawlerData, PreviewPrefab, AddTextureButton, _scrollRect, _textureInfoPanel, _headerText);
            Grid.DestroyAllChildren(AddTextureButton.transform);
        }

        void OnEnable()
        {
            _dungeonCrawlerData.MaterialCache.AddObserver(HandleCacheUpdate);
        }

        void OnDisable()
        {
            _dungeonCrawlerData.MaterialCache.RemoveObserver(HandleCacheUpdate);
        }

        private void HandleCacheUpdate(CacheUpdateData update)
        {
            bool gridChanged = update switch
            {
                CacheInitialized(IEnumerable<TextureReference> cache) => InitializeGrid(cache),
                CacheAddTexture(TextureReference texture) => AddTexture(texture),
                CacheRemoveTexture(TextureReference texture) => RemoveTexture(texture),
                _ => false,
            };
            if (gridChanged)
            {
                AddTextureButton.transform.SetAsLastSibling();
            }
        }

        public bool RemoveTexture(TextureReference texture)
        {
            if (!_textureButtons.TryGetValue(texture, out DungeonTexturePreview button)) { return false; }
            GameObject.Destroy(button.gameObject);
            return true;
        }

        public bool AddTexture(TextureReference texture)
        {
            if (_textureButtons.ContainsKey(texture)) { return false; }
            DungeonTexturePreview preview = DungeonTexturePreview.Instantiate(PreviewPrefab, Grid, texture);
            _textureButtons[texture] = preview;
            preview.SelectButton.OnClick.AddListener(SelectTexture);
            preview.OnDelete.AddListener(OpenTextureInfoPanel);
            return true;
        }

        public bool InitializeGrid(IEnumerable<TextureReference> textures)
        {
            // TODO: Deleting all children is bad, this happens everytime the window is opened
            Grid.DestroyAllChildren(AddTextureButton.transform);
            _textureButtons.Clear();
            foreach (TextureReference texture in textures)
            {
                AddTexture(texture);
            }
            return true;
        }

        private void OpenTextureInfoPanel(DungeonTexturePreview preview)
        {
            _textureInfoPanel.Texture = preview.Texture;
            _textureInfoPanel.Show();
        }

        private void AddTexture(string name, Texture2D texture)
        {
            _dungeonCrawlerData.MaterialCache.AddTexture(name, texture);
            StartCoroutine(ScrollToBottom());
        }

        private IEnumerator ScrollToBottom()
        {
            yield return null;
            _scrollRect.verticalNormalizedPosition = 0;
        }

        public void Cancel()
        {
            gameObject.SetActive(false);
            _onCanceledCallback?.Invoke();
        }

        private void SelectTexture(DungeonTextureButton textureButton)
        {
            gameObject.SetActive(false);
            _onSelectedCallback?.Invoke(textureButton.Texture);
        }

        public void ShowDialogue(string headerText, System.Action<TextureReference> onSelected, System.Action onCanceled)
        {
            _headerText.text = headerText;
            _headerText.gameObject.SetActive(true);
            _onSelectedCallback = onSelected;
            _onCanceledCallback = onCanceled;
            gameObject.SetActive(true);
        }

        public void ShowDialogue(System.Action<TextureReference> onSelected, System.Action onCanceled)
        {
            _headerText.gameObject.SetActive(false);
            _onSelectedCallback = onSelected;
            _onCanceledCallback = onCanceled;
            gameObject.SetActive(true);
        }

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void PromptAddNewTexture()
    {
        UploadFile(gameObject.name, "OnFileUpload", ".png", false);
    }

    // Called from browser
    public void OnFileUpload(string url) {
        StartCoroutine(OutputRoutine(url));
    }
#else

        public void PromptAddNewTexture()
        {
            ExtensionFilter[] filter = { new("Image", "png") };
            var paths = StandaloneFileBrowser.OpenFilePanel("Select a texture", "", filter, false);
            if (paths.Length > 0)
            {
                StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
            }

        }
#endif

        private IEnumerator OutputRoutine(string url)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string filename = www.uri.Segments.Last();
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                _confirmPanel.Prompt(_dungeonCrawlerData.ManifestData, texture, filename, AddTexture);
            }
        }

    }
}