#if UNITY_WEBGL
#pragma warning disable IDE0005 // Using directive is unnecessary.
using System.Runtime.InteropServices;
#pragma warning restore IDE0005 // Using directive is unnecessary.
#endif
using System.Collections;

using SFB;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

using System.Linq;

using CaptainCoder.Unity.UI;
using CaptainCoder.Unity;

using System.Collections.Generic;

using CaptainCoder.Dungeoneering.Unity.Data;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class DungeonTextureSelectorController : MonoBehaviour
    {
        [field: SerializeField]
        public Transform Grid { get; private set; }
        [field: SerializeField]
        public DungeonManifestData Manifest { get; private set; }
        [SerializeField]
        private DungeonData _dungeonData;
        [field: SerializeField]
        public DungeonTexturePreview PreviewPrefab { get; private set; }
        [field: SerializeField]
        public Button AddTextureButton { get; private set; }
        [SerializeField]
        private ConfirmTexturePromptPanel _confirmPanel;
        [SerializeField]
        private ScrollRect _scrollRect;
        private HashSet<TextureId> _textureIds = new();

        private System.Action<TextureId> _onSelectedCallback;
        private System.Action _onCanceledCallback;

        void Awake()
        {
            Assertion.NotNull(this, _confirmPanel, Grid, Manifest, PreviewPrefab, AddTextureButton, _scrollRect);
            Grid.DestroyAllChildren(AddTextureButton.transform);
        }

        void OnEnable()
        {
            Manifest.MaterialCache.AddListener(HandleCacheUpdate);
        }

        void OnDisable()
        {
            Manifest.MaterialCache.RemoveListener(HandleCacheUpdate);
        }

        private void HandleCacheUpdate(CacheUpdateData update)
        {
            bool gridChanged = update switch
            {
                CacheInitialized(IEnumerable<TextureReference> cache) => InitializeGrid(cache),
                CacheAddTexture(TextureReference texture) => AddTexture(texture),
                _ => false,
            };
            if (gridChanged)
            {
                AddTextureButton.transform.SetAsLastSibling();
            }
        }

        public bool AddTexture(TextureReference texture)
        {
            if (_textureIds.Contains(texture.TextureId)) { return false; }
            DungeonTexturePreview preview = DungeonTexturePreview.Instantiate(PreviewPrefab, Grid, texture.Material);
            _textureIds.Add(texture.TextureId);
            preview.SelectButton.OnClick.AddListener(SelectTexture);
            preview.OnDelete.AddListener(DeleteTexture);
            return true;
        }

        public bool InitializeGrid(IEnumerable<TextureReference> textures)
        {
            // TODO: Deleting all children is bad, this happens everytime the window is opened
            Grid.DestroyAllChildren(AddTextureButton.transform);
            _textureIds.Clear();
            foreach (TextureReference texture in textures)
            {
                AddTexture(texture);
            }
            return true;
        }

        private void DeleteTexture(DungeonTexturePreview preview)
        {
            Manifest.MaterialCache.RemoveTextureReference(preview.Material.Id);
            GameObject.Destroy(preview.gameObject);
        }

        private void AddTexture(string name, Texture2D texture)
        {
            Manifest.MaterialCache.AddTexture(name, texture);
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
            _onSelectedCallback?.Invoke(textureButton.TextureId);
        }

        public void ShowDialogue(System.Action<TextureId> onSelected, System.Action onCanceled)
        {
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
                _confirmPanel.Prompt(Manifest, texture, filename, AddTexture);
            }
        }

    }
}