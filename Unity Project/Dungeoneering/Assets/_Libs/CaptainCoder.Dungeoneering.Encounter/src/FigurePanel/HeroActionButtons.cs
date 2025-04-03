using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class HeroActionButtons : MonoBehaviour
    {
        [AssertIsSet][field: SerializeField] public HeroActionButton ExpandButton { get; private set; }
        [AssertIsSet][field: SerializeField] public HeroActionButton MinimizeButton { get; private set; }
        [AssertIsSet][field: SerializeField] public HeroActionButton TakeTurnButton { get; private set; }
    }
}