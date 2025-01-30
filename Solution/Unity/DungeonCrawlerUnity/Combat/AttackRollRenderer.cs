using CaptainCoder.Dungeoneering.Player.Unity;

using TMPro;

using UnityEngine;
namespace CaptainCoder.DungeonCrawler.Combat.Unity;
public class AttackRollRenderer : MonoBehaviour
{
    public TextMeshProUGUI AttackLabel = default!;
    public TextMeshProUGUI HealthArmorLabel = default!;
    public TextMeshProUGUI DamageLabel = default!;
    public void OnDisable()
    {
        CombatHelpPanel.Shared.gameObject.SetActive(false);
    }
    public void Render(Enemy target, int exert, int energy, IAttackRoll roll)
    {
        gameObject.SetActive(true);
        string keys = $"{PlayerInputHandler.Shared.GetKeys(MenuControl.Up)} and {PlayerInputHandler.Shared.GetKeys(MenuControl.Down)}";
        string backspace = $"{PlayerInputHandler.Shared.GetKeys(MenuControl.Cancel)}";
        CombatHelpPanel.Shared.Text = $"Armor absorbs damage. Press {keys} to spend energy to increase the damage roll. Press {backspace} to cancel.";
        AttackLabel.text = $"Attack: {target.Card.Name}";
        HealthArmorLabel.text = $"Health: {target.Health} | Armor: {target.Card.Armor}";
        DamageLabel.text = $"DMG: {roll.Min} - {roll.Max} | ENG: {exert}/{energy}";
        if (energy == 0) { DamageLabel.text = $"DMG: {roll.Min} - {roll.Max} | No Energy"; }
    }
}