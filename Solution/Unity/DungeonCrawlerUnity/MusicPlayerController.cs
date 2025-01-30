using System.Collections;

using UnityEngine;

namespace CaptainCoder.DungeonCrawler.Unity;

public class MusicPlayerController : MonoBehaviour
{
    public static MusicPlayerController Shared { get; private set; } = default!;
    public MusicPlayerController() { Shared = this; }
    public AudioSource AudioSource = default!;
    public MusicEntry[] Entries = [];
    private float _volume = 0.25f;
    public float FadeTime = 2f;

    public void OnVolumeChange(float value)
    {
        _volume = value;
        AudioSource.volume = value;
    }

    public void Play(Music toPlay, bool loop = true)
    {
        MusicEntry? entry = Entries.Where(e => e.Sound == toPlay).FirstOrDefault();
        if (entry is null) { Debug.Log($"No music file for {toPlay}."); return; }
        if (entry.Clip == AudioSource.clip) { return; }
        AudioSource.loop = loop;
        StartCoroutine(FadeIn(entry));
    }

    private IEnumerator FadeIn(MusicEntry entry)
    {
        float currentTime = 0;
        float totalTime = FadeTime * 0.5f;

        // Fade out
        while (currentTime < totalTime)
        {
            float percentage = (totalTime - currentTime) / totalTime;
            AudioSource.volume = percentage * _volume;
            yield return new WaitForEndOfFrame();
            currentTime += Time.deltaTime;
        }
        AudioSource.volume = 0;
        AudioSource.clip = entry.Clip;
        AudioSource.Play();
        currentTime = 0;
        while (currentTime < totalTime)
        {
            float percentage = currentTime / totalTime;
            AudioSource.volume = percentage * _volume;
            yield return new WaitForEndOfFrame();
            currentTime += Time.deltaTime;
        }
        AudioSource.volume = _volume;
    }
}


public enum Music
{
    MainMenu,
    Crawling,
    Combat,
    Victory,
    Credits,
    GameOver,
}

[Serializable]
public class MusicEntry
{
    public Music Sound;
    public AudioClip Clip = default!;
}