using System.Collections;

using TMPro;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Game.Unity;

public class QueuedMessageRenderer : MonoBehaviour
{
    [field: SerializeField]
    public TextMeshProUGUI Text { get; private set; } = default!;
    private readonly Queue<(string message, float duration)> _messages = new();

    public void OnEnable()
    {
        StartCoroutine(MessageHandler());
    }

    public void OnDisable()
    {
        StopAllCoroutines();
    }

    public void EnqueueMessage(Message message, float duration = 2f)
    {
        _messages.Enqueue((message.Text, duration));
    }

    private IEnumerator MessageHandler()
    {
        while (true)
        {
            while (_messages.TryDequeue(out var element))
            {
                (string message, float duration) = element;
                Text.text = message;
                yield return new WaitForSeconds(duration);
            }
            Text.text = string.Empty;
            yield return new WaitUntil(() => _messages.Count > 0);
        }
    }

}