using System.IO;

using CaptainCoder.Dungeoneering.DungeonMap.IO;

using CaptainCoder.Dungeoneering.DungeonMap.Unity;

using SFB;

using UnityEngine;

using System.Collections;

using UnityEngine.Networking;


namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class ExportManifestPanel : MonoBehaviour
    {
        [SerializeField]
        private DungeonManifestData _manifestData;
        public void Hide() => gameObject.SetActive(false);
        public void Toggle() => gameObject.SetActive(!gameObject.activeSelf);


#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

    // Broser plugin should be called in OnPointerDown.
    public void PromptExport() {
        var bytes = Encoding.UTF8.GetBytes(_manifestData.Manifest.ToJson());
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
                File.WriteAllText(path, _manifestData.Manifest.ToJson());
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
            }
            else
            {
                _manifestData.TryLoadManifest(www.downloadHandler.text, out _);
            }
            Hide();
        }

    }
}