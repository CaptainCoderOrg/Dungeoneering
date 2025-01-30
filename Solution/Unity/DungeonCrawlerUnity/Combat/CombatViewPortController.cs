using UnityEngine;

namespace CaptainCoder.DungeonCrawler.Combat.Unity;

public class CombatViewPortController : MonoBehaviour
{
    public static CombatViewPortController Shared { get; private set; } = default!;
    public CombatViewPortController() { Shared = this; }
}