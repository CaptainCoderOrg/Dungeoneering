using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class HeroActionButtons : MonoBehaviour
    {
        [AssertIsSet][field: SerializeField] public HeroActionButton InventoryButton { get; private set; }
        [AssertIsSet][field: SerializeField] public HeroActionButton SkillsButton { get; private set; }
        [AssertIsSet][field: SerializeField] public HeroActionButton TakeTurnButton { get; private set; }
        [AssertIsSet][field: SerializeField] public HeroActionButton EndTurnButton { get; private set; }
        [AssertIsSet][field: SerializeField] public HeroActionButton MovementButton { get; private set; }
        [AssertIsSet][field: SerializeField] public HeroActionButton AttackButton { get; private set; }

        internal void UpdateButtons(FigureData figureData)
        {
            if (figureData.Movement > 0)
            {
                MovementButton.Enabled = true;
                MovementButton.Tooltip.Tooltip = $"Move ({figureData.Movement})";
            }
            else
            {
                MovementButton.Enabled = false;
                MovementButton.Tooltip.Tooltip = "Move (0)";
            }

            if (figureData.Attacks > 0)
            {
                AttackButton.Enabled = true;
                AttackButton.Tooltip.Tooltip = $"Attack ({figureData.Attacks})";
            }
            else
            {
                AttackButton.Enabled = false;
                AttackButton.Tooltip.Tooltip = "Attack (0)";
            }
        }
    }
}