using UnityEngine;

namespace CaptainCoder.DungeonCrawler.Combat.Unity;

public class CrawlingViewPortController : MonoBehaviour
{
    public static CrawlingViewPortController Shared { get; private set; } = default!;
    public CrawlingViewPortController() { Shared = this; }
}