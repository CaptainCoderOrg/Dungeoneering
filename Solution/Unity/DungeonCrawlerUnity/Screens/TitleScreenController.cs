using System.Collections;

using CaptainCoder.DungeonCrawler.Combat.Unity;
using CaptainCoder.DungeonCrawler.Unity;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity;

public class TitleScreenController : MonoBehaviour
{
    public static TitleScreenController Shared { get; private set; } = default!;
    public TitleScreenController() { Shared = this; }
    public RectTransform IkeaFloat = default!;
    private Vector3 _startPosition = default!;
    public float BobbleRange = 5f;
    public float BobbleSpeed = 2f;
    public GameObject[] Doors = [];
    public GameObject Compass = default!;
    public GameObject Loading = default!;

    public void OnDisable()
    {
        StopAllCoroutines();
    }
    public void OnEnable()
    {
        Loading.SetActive(false);
        MusicPlayerController.Shared.Play(Music.MainMenu);
        CrawlingModeController.Shared.gameObject.SetActive(false);
        CombatModeController.Shared.gameObject.SetActive(false);
        StartCoroutine(OnNextFrame(CloseDoors));
        StartCoroutine(Bobble());
        void CloseDoors() { foreach (GameObject door in Doors) { door.SetActive(true); } }
    }

    public void Awake()
    {
        _startPosition = IkeaFloat.anchoredPosition;
    }

    public IEnumerator Bobble()
    {
        IkeaFloat.anchoredPosition = _startPosition;
        Vector3 position = _startPosition;
        float current = 0;
        while (true)
        {
            yield return new WaitForEndOfFrame();
            current += Time.deltaTime;
            float offset = Mathf.Sin(current * BobbleSpeed) * BobbleRange;
            position.y = _startPosition.y + offset;
            IkeaFloat.anchoredPosition = position;
        }
    }

    public void StartGame()
    {
        Loading.SetActive(true);
        StartCoroutine(OnNextFrame(DoStartGame));
    }

    public void DoStartGame()
    {
        CrawlingModeController.Shared.StartFromManifest(OnFinished);
        void OnFinished()
        {
            gameObject.SetActive(false);
            Loading.SetActive(false);
            Compass.SetActive(true);
        }
    }

    private IEnumerator OnNextFrame(Action action)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        action.Invoke();
    }
}