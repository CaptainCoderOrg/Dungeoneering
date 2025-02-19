#if UNITY_WEBGL
using System.Runtime.InteropServices;
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
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class DungeonTextureSelectorController : MonoBehaviour
    {
        [field: SerializeField]
        public Transform Grid { get; private set; }
        [field: SerializeField]
        public DungeonManifestData Manifest { get; private set; }
        [field: SerializeField]
        public DungeonTextureButton ButtonPrefab { get; private set; }
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
            Assertion.NotNull(this, _confirmPanel, Grid, Manifest, ButtonPrefab, AddTextureButton, _scrollRect);
            Grid.DestroyAllChildren(AddTextureButton.transform);
        }

        void OnEnable()
        {
            Manifest.AddListener(InitializeGrid);
        }

        void OnDisable()
        {
            Manifest.RemoveListener(InitializeGrid);
        }

        public void InitializeGrid(CacheUpdateData update)
        {
            foreach ((string name, Material material) in update.Cache)
            {
                if (_textureNames.Contains(name)) { continue; }
                _textureNames.Add(name);
                DungeonTextureButton btn = Instantiate(ButtonPrefab, Grid);
                btn.TextureName = name;
                btn.Image.texture = material.mainTexture;
                btn.OnClick.AddListener(SelectTexture);
            }
            AddTextureButton.transform.SetAsLastSibling();
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

        private void AddTexture(string name, Texture2D texture)
        {
            Manifest.AddTexture(name, texture);
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

    }
}