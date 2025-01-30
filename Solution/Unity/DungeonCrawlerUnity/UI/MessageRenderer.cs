using System.Collections;

using CaptainCoder.Dungeoneering.Unity;
using CaptainCoder.Unity;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace CaptainCoder.Dungeoneering.Game.Unity;

public class MessageRenderer : MonoBehaviour
{
    public static MessageRenderer Shared { get; private set; } = default!;
    private WaitUntil WaitUntilQueueHasItem => new(() => _renderQueue.Count > 0);
    [field: SerializeField]
    public Transform TextOutput { get; private set; } = default!;
    [field: SerializeField]
    public TextMeshProUGUI TextTemplate { get; private set; } = default!;
    [field: SerializeField]
    public ScrollRect Scroll { get; private set; } = default!;

    private readonly Queue<Message> _renderQueue = new();

    public MessageRenderer() { Shared = this; }

    public void Awake()
    {
        TextOutput.DestroyAllChildren(Destroy);
    }

    public void Initialize()
    {
        gameObject.SetActive(true);
        CrawlingModeController.CrawlerMode.OnMessageAdded += AddMessage;
        CrawlingModeController.OnCrawlerModeChange += crawler => crawler.OnMessageAdded += AddMessage;
        StopAllCoroutines();
        StartCoroutine(RenderText());
    }

    public void AddMessage(string message) => AddMessage(new Message(message));

    public void AddMessage(Message message) => _renderQueue.Enqueue(message);
    private IEnumerator RenderText()
    {
        while (true)
        {
            yield return WaitUntilQueueHasItem;
            Message message = _renderQueue.Dequeue();
            TextMeshProUGUI newText = Instantiate(TextTemplate, TextOutput);
            Debug.Log($"Info: {message}");
            if (TextOutput.childCount > 10)
            {
                Destroy(TextOutput.GetChild(0).gameObject);
            }
            newText.text = message.Text;
            newText.maxVisibleCharacters = 0;
            while (newText.maxVisibleCharacters < message.Text.Length)
            {
                newText.maxVisibleCharacters += 5;
                yield return new WaitForEndOfFrame();
                Scroll.normalizedPosition = new Vector2(0, 0); // Scrolls to bottom
            }
        }
    }
}