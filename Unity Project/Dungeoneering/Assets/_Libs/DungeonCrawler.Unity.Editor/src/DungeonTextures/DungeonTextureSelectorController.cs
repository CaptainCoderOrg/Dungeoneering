#if UNITY_WEBGL
#pragma warning disable IDE0005 // Using directive is unnecessary.
using System.Runtime.InteropServices;
#pragma warning restore IDE0005 // Using directive is unnecessary.
#endif
using System.Collections;

using CaptainCoder.Dungeoneering.DungeonMap.Unity;

using SFB;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

using System.Linq;

using CaptainCoder.Unity.UI;
using CaptainCoder.Unity;

using System.Collections.Generic;
using System;
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
        private HashSet<string> _textureNames = new();

        private System.Action<string> _onSelectedCallback;
        private System.Action _onCanceledCallback;

        void Awake()
        {
            Assertion.NotNull(this, _confirmPanel, Grid, Manifest, PreviewPrefab, AddTextureButton, _scrollRect);
            Grid.DestroyAllChildren(AddTextureButton.transform);
        }

        void OnEnable()
        {
            Manifest.MaterialCache.AddListener(InitializeGrid);
        }

        void OnDisable()
        {
            Manifest.MaterialCache.RemoveListener(InitializeGrid);
        }

        public void InitializeGrid(CacheUpdateData update)
        {
            if (update.IsNewCache)
            {
                Grid.DestroyAllChildren(AddTextureButton.transform);
                _textureNames.Clear();
            }
            foreach ((string name, SelectableMaterial material) in update.Cache)
            {
                if (_textureNames.Contains(name)) { continue; }
                DungeonTexturePreview preview = DungeonTexturePreview.Instantiate(PreviewPrefab, name, Grid, material);
                _textureNames.Add(name);
                preview.SelectButton.OnClick.AddListener(SelectTexture);
                preview.OnDelete.AddListener(DeleteTexture);
            }
            AddTextureButton.transform.SetAsLastSibling();
        }

        private void DeleteTexture(DungeonTexturePreview preview)
        {
            Debug.Log($"Deleting: {preview}");
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
            _onSelectedCallback?.Invoke(textureButton.TextureName);
        }

        public void ShowDialogue(System.Action<string> onSelected, System.Action onCanceled)
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