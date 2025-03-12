using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity;

using TMPro;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class EditorTopBarController : MonoBehaviour
    {
        [SerializeField]
        private DungeonCrawlerData _dungeonCrawlerData;
        [SerializeField]
        private TextMeshProUGUI _nameLabel;
        [SerializeField]
        private DungeonSelectorPanel _dungeonSelectorPanel;
        [SerializeField]
        private ExportManifestPanel _exportManifestPanel;
        [SerializeField]
        private Button _selectorToggleButton;

        void Awake()
        {
            Assertion.NotNull(this, _nameLabel, _dungeonSelectorPanel, _exportManifestPanel, _selectorToggleButton, _dungeonCrawlerData);
        }

        void OnEnable()
        {
            _dungeonCrawlerData.AddObserver(HandleDungeonChanged);
            _selectorToggleButton.onClick.AddListener(_dungeonSelectorPanel.Toggle);
        }

        void OnDisable()
        {
            _dungeonCrawlerData.RemoveObserver(HandleDungeonChanged);
            _selectorToggleButton.onClick.AddListener(_dungeonSelectorPanel.Toggle);
        }

        private void HandleDungeonChanged(DungeonChanged changes)
        {
            switch (changes)
            {
                case DungeonLoaded(Dungeon loaded):
                    _nameLabel.text = loaded.Name;
                    break;
                case DungeonSyncedStateChanged(Dungeon dungeon, bool isSynced):
                    _nameLabel.text = isSynced ? dungeon.Name : $"{dungeon?.Name}*";
                    break;
            }

        }
        public void ShowExportPanel() => _exportManifestPanel.Toggle();
        public void Save() => _dungeonCrawlerData.SyncWithManifest();
    }
}