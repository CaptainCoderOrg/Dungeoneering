using System.Collections;

using UnityEngine;
using UnityEngine.Networking;

namespace CaptainCoder.Dungeoneering.Unity;

public static class WebLoader
{
    public static IEnumerator GetTextFromURL(string url, Action<string> onSuccess, Action<string> onFail)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            onFail.Invoke(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            onSuccess.Invoke(www.downloadHandler.text);
        }
    }
}