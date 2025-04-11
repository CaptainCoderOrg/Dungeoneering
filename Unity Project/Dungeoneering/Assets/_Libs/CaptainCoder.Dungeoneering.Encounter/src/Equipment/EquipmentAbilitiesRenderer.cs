
using System.Text;

using CaptainCoder.Unity.Assertions;

using TMPro;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EquipmentAbilitiesRenderer : MonoBehaviour, IEquipmentRenderer
    {
        private static readonly StringBuilder Builder = new();
        [AssertIsSet][SerializeField] private TextMeshProUGUI _label;
        [AssertIsSet][SerializeField] private ToggleablePanel _toggleablePanel;
        public void Render(EquipmentData data)
        {
            if (data.AttackAbilities.Length == 0) { _toggleablePanel.Hide(); return; }
            Builder.Clear();
            foreach (var ability in data.AttackAbilities)
            {
                Builder.Append(ability.Description);
                Builder.Append("\n");
            }
            _label.text = Builder.ToString();
            _toggleablePanel.Show();
        }
    }
}