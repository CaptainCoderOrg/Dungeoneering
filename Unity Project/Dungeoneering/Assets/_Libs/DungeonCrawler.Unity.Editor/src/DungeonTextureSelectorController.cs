#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif
using System.Collections;

using CaptainCoder.Dungeoneering.DungeonMap.Unity;

using NaughtyAttributes;

using SFB;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
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

        private System.Action<string> _onSelectedCallback;
        private System.Action _onCanceledCallback;

        void OnEnable()
        {
            Manifest.AddListener(InitializeGrid);
        }

        void OnDisable()
        {
            Manifest.RemoveListener(InitializeGrid);
        }

        [Button]
        private void InitializeGrid() => InitializeGrid(Manifest.MaterialCache);

        public void InitializeGrid(Dictionary<string, Material> materialCache)
        {
            Grid.DestroyAllChildren(AddTextureButton.transform);
            foreach (var textureEntry in materialCache.Reverse())
            {
                DungeonTextureButton btn = Instantiate(ButtonPrefab, Grid);
                btn.TextureName = textureEntry.Key;
                btn.Image.texture = textureEntry.Value.mainTexture;
                btn.OnClick.AddListener(SelectTexture);
            }
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
                Debug.Log(filename);
                bool success = Manifest.AddTexture(filename, ((DownloadHandlerTexture)www.downloadHandler).texture);
                Debug.Log(success);
            }
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