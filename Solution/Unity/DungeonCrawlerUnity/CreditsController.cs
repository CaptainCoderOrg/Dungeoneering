using System.Collections;

using CaptainCoder.DungeonCrawler.Unity;
using CaptainCoder.Dungeoneering.Player.Unity;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity;

public class CreditsController : MonoBehaviour
{
    public static CreditsController Shared { get; private set; } = default!;
    public CreditsController() { Shared = this; }
    public GameObject ScrollingObject = default!;
    public float StartPosition = 900;
    public float EndPosition = 1975;
    public AudioClip Clip = default!;
    public float Duration;
    public float CurrentTime;
    public float Percent;
    public float TotalDistance;
    public Vector3 Position;
    private Action? _onClose;

    public void OnDisable()
    {
        StopAllCoroutines();
        PlayerInputHandler.Shared.gameObject.SetActive(true);
        _onClose?.Invoke();
    }

    public void OnEnable() => Initialize();

    public void Initialize(Action? onClose = null)
    {
        _onClose = onClose;
        PlayerInputHandler.Shared.gameObject.SetActive(false);
        MusicPlayerController.Shared.Play(Music.Credits, false);
        gameObject.SetActive(true);
        StartCoroutine(ScrollFor(Clip.length - 20));
    }

    private IEnumerator ScrollFor(float duration)
    {
        Duration = duration;
        CurrentTime = 0;
        RectTransform rect = ((RectTransform)ScrollingObject.transform);
        Position = rect.anchoredPosition;
        Position.y = StartPosition;
        rect.anchoredPosition = Position;
        TotalDistance = EndPosition - StartPosition;
        while (CurrentTime < duration)
        {
            yield return new WaitForEndOfFrame();
            Percent = Math.Clamp(CurrentTime / duration, 0, 1);
            Position.y = StartPosition + (TotalDistance * Percent);
            rect.anchoredPosition = Position;
            CurrentTime += Time.deltaTime;
        }
        Position.y = EndPosition;
        rect.anchoredPosition = Position;
    }
}