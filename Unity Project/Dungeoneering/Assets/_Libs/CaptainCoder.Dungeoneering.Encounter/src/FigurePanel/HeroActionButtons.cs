using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class HeroActionButtons : MonoBehaviour
    {
        [AssertIsSet][field: SerializeField] public HeroActionButton ExpandButton { get; private set; }
        [AssertIsSet][field: SerializeField] public HeroActionButton MinimizeButton { get; private set; }
        [AssertIsSet][field: SerializeField] public HeroActionButton TakeTurnButton { get; private set; }
        [AssertIsSet][field: SerializeField] public HeroActionButton MovementButton { get; private set; }
        [AssertIsSet][field: SerializeField] public HeroActionButton AttackButton { get; private set; }

        [SerializeField] private HeroActionButton[] _hiddenButtons;

        public void Show()
        {
            ExpandButton.Hide();
            MinimizeButton.Show();
            foreach (var button in _hiddenButtons)
            {
                button.Show();
            }
        }

        public void Hide()
        {
            ExpandButton.Show();
            MinimizeButton.Hide();
            foreach (var button in _hiddenButtons)
            {
                button.Hide();
            }
        }
    }
}