using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class HeroActionButtons : MonoBehaviour
    {
        [AssertIsSet][field: SerializeField] public HeroActionButton InventoryButton { get; private set; }
        [AssertIsSet][field: SerializeField] public HeroActionButton SkillsButton { get; private set; }
        [AssertIsSet][field: SerializeField] public HeroActionButton TakeTurnButton { get; private set; }
        [AssertIsSet][field: SerializeField] public HeroActionButton MovementButton { get; private set; }
        [AssertIsSet][field: SerializeField] public HeroActionButton AttackButton { get; private set; }
    }
}