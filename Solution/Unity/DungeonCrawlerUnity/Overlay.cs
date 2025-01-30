using CaptainCoder.DungeonCrawler.Combat.Unity;

using UnityEngine;

namespace CaptainCoder.DungeonCrawler.Unity;

public class Overlay : MonoBehaviour
{
    public static Overlay Shared { get; private set; } = default!;
    [field: SerializeField]
    public CharacterCardRenderer[] Cards { get; private set; } = [];
    public Overlay() { Shared = this; }
}