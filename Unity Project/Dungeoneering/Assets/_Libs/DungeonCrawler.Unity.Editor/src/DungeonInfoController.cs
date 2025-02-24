using CaptainCoder.Dungeoneering.DungeonMap;
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
        private DungeonManifestData _manifest;
        [SerializeField]
        private DungeonData _dungeonData;
        [SerializeField]
        private TextMeshProUGUI _nameLabel;
        [SerializeField]
        private DungeonSelectorPanel _dungeonSelectorPanel;
        [SerializeField]
        private Button _selectorToggleButton;

        void Awake()
        {
            Assertion.NotNull(this, DungeonController, _nameLabel, _dungeonSelectorPanel, _selectorToggleButton, _dungeonData, _manifest);
        }

        void OnEnable()
        {
            _dungeonData.OnStateChanged.AddListener(HandleDungeonDataStateChanged);
            DungeonController.OnDungeonChanged.AddListener(HandleDungeonChanged);
            _selectorToggleButton.onClick.AddListener(_dungeonSelectorPanel.Toggle);
        }

        void OnDisable()
        {
            _dungeonData.OnStateChanged.RemoveListener(HandleDungeonDataStateChanged);
            DungeonController.OnDungeonChanged.RemoveListener(HandleDungeonChanged);
            _selectorToggleButton.onClick.AddListener(_dungeonSelectorPanel.Toggle);
        }

        private void HandleDungeonChanged(DungeonData dungeon)
        {
            _nameLabel.text = dungeon.Dungeon.Name;
        }

        public void Save() => _dungeonData.SaveToManifest(_manifest);

        private void HandleDungeonDataStateChanged(Dungeon dungeon, bool hasChanged)
        {
            _nameLabel.text = hasChanged ? $"{dungeon?.Name}*" : dungeon?.Name;
        }
    }
}