using System.Collections;
using System.IO;

using CaptainCoder.Dungeoneering.DungeonMap.IO;
using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity;

using SFB;

using UnityEngine;
using UnityEngine.Networking;
using CaptainCoder.Unity.UI;




#if UNITY_WEBGL
#pragma warning disable IDE0005 // Using directive is unnecessary.
using System.Runtime.InteropServices;
using System.Text;
#pragma warning restore IDE0005 // Using directive is unnecessary.
#endif

namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class ExportManifestPanel : MonoBehaviour
    {
        [SerializeField]
        private DungeonCrawlerData _dungeonCrawlerData;
        [SerializeField]
        private InfoPromptPanel _infoPrompt;
        public void Hide() => gameObject.SetActive(false);
        public void Toggle() => gameObject.SetActive(!gameObject.activeSelf);

        void Awake()
        {
            Assertion.NotNull(this, (_dungeonCrawlerData, "Dungeon Crawler Data"));
        }


#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

    // Broser plugin should be called in OnPointerDown.
    public void PromptExport() {
        var bytes = Encoding.UTF8.GetBytes(_dungeonCrawlerData.ManifestData.Manifest.ToJson());
        DownloadFile(gameObject.name, "OnFileDownload", "dungeon-manifest.json", bytes, bytes.Length);
        Hide();
    }

    // Called from browser
    public void OnFileDownload() {
        
    }
#else

        public void PromptExport()
        {
            var path = StandaloneFileBrowser.SaveFilePanel("Title", "", "dungeon-manifest", "json");
            if (!string.IsNullOrEmpty(path))
            {
                File.WriteAllText(path, _dungeonCrawlerData.ManifestData.Manifest.ToJson());
            }
            Hide();
        }
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void PromptImport()
    {
        UploadFile(gameObject.name, "OnFileUpload", ".json", false);
    }

    // Called from browser
    public void OnFileUpload(string url) {
        StartCoroutine(ImportRoutine(url));
    }
#else

        public void PromptImport()
        {
            ExtensionFilter[] filter = { new("Dungeon Manifest", "json") };
            var paths = StandaloneFileBrowser.OpenFilePanel("Select a Manifest", "", filter, false);
            if (paths.Length > 0)
            {
                StartCoroutine(ImportRoutine(new System.Uri(paths[0]).AbsoluteUri));
            }

        }
#endif

        private IEnumerator ImportRoutine(string url)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                _infoPrompt.ShowInfo($"An error occurred while loading: {www.error}");
                yield break;
            }

            if (_dungeonCrawlerData.ManifestData.TryLoadManifest(www.downloadHandler.text, out string message))
            {
                Hide();
                yield break;
            }

            _infoPrompt.ShowInfo(message);
        }

    }
}