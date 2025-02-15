using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using CaptainCoder.Unity;

using TMPro;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class DungeonInfoController : MonoBehaviour
    {
        [field: SerializeField]
        public DungeonController DungeonController { get; private set; }

        [SerializeField]
        private TextMeshProUGUI _nameLabel;
        [SerializeField]
        private DungeonSelectorPanel _dungeonSelectorPanel;
        [SerializeField]
        private Button _selectorToggleButton;

        void Awake()
        {
            Assertion.NotNull(this, DungeonController, _nameLabel, _dungeonSelectorPanel, _selectorToggleButton);
        }

        void OnEnable()
        {
            DungeonController.OnDungeonChanged.AddListener(HandleDungeonChanged);
            _selectorToggleButton.onClick.AddListener(_dungeonSelectorPanel.Toggle);
        }

        void OnDisable()
        {
            DungeonController.OnDungeonChanged.RemoveListener(HandleDungeonChanged);
            _selectorToggleButton.onClick.AddListener(_dungeonSelectorPanel.Toggle);
        }

        private void HandleDungeonChanged(DungeonData dungeon)
        {
            _nameLabel.text = dungeon.Dungeon.Name;
        }


    }
}