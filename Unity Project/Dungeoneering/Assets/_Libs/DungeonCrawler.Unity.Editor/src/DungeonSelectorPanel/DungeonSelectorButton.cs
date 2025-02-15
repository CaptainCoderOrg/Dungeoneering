using CaptainCoder.Unity;

using TMPro;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class DungeonSelectorButton : MonoBehaviour
    {
        public Button Button { get; private set; }
        public TextMeshProUGUI Label { get; private set; }
        void Awake()
        {
            Button = GetComponent<Button>();
            Label = GetComponentInChildren<TextMeshProUGUI>();
            Assertion.NotNull(this, Button, Label);
        }

    }
}