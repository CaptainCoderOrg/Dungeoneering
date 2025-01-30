using UnityEngine;

namespace CaptainCoder.DungeonCrawler.Unity;

public class SFXController : MonoBehaviour
{
    public static SFXController Shared { get; private set; } = default!;
    public SFXController() { Shared = this; }
    public AudioSource SampleSound = default!;
    public SoundEffect SFXTemplate = default!;
    public Transform SFXParent = default!;
    public SoundEffectEntry[] Entries = [];

    private float _volume = 0.25f;

    public void OnVolumeChange(float value)
    {
        SampleSound.volume = value;
        if (!SampleSound.isPlaying) { SampleSound.Play(); }
        _volume = value;
    }

    public void PlaySound(Sound toPlay)
    {
        SoundEffectEntry? entry = Entries.Where(e => e.Sound == toPlay).FirstOrDefault();
        if (entry is not null)
        {
            int ix = UnityEngine.Random.Range(0, entry.Clips.Length);
            SoundEffect sfx = Instantiate(SFXTemplate);
            sfx.Play(entry.Clips[ix], _volume);
        }
    }
}

// Hack: If you change this you must also change LuaContext.Sound -- they must be the same order
// You must also update LuaApi.lua
public enum Sound
{
    Hit,
    Die,
    Footstep,
    Eat,
    Miss,
    Rest,
    Guard,
    Cheat,
    LevelUp,
}

[Serializable]
public class SoundEffectEntry
{
    public Sound Sound;
    public AudioClip[] Clips = [];
}

public class SoundEffect : MonoBehaviour
{
    public AudioSource Sound = default!;

    public void Play(AudioClip clip, float volume)
    {
        Sound.clip = clip;
        Sound.volume = volume;
        Sound.loop = false;
        Sound.Play();
        Destroy(gameObject, clip.length + 0.5f);
    }
}