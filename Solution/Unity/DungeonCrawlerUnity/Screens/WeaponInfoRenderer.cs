using CaptainCoder.DungeonCrawler.Combat;

using TMPro;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity;

public class WeaponInfoRenderer : MonoBehaviour
{
    public TextMeshProUGUI Name = default!;
    public TextMeshProUGUI DamageText = default!;
    public void Render(Weapon weapon)
    {
        Name.text = weapon.Name;
        DamageText.text = $"{weapon.AttackRoll.Min} - {weapon.AttackRoll.Max}";
    }
}