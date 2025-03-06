using System.Collections;

using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity;

using TMPro;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class DungeonInfoController : MonoBehaviour
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
            _dungeonCrawlerData.DungeonData.OnStateChanged.AddListener(HandleDungeonDataStateChanged);
            _dungeonCrawlerData.DungeonData.OnChange.AddListener(HandleDungeonChanged);
            _selectorToggleButton.onClick.AddListener(_dungeonSelectorPanel.Toggle);
            StartCoroutine(UpdateUIAtEndOfFrame());
        }

        private IEnumerator UpdateUIAtEndOfFrame()
        {
            yield return null;
            HandleDungeonChanged(new DungeonChangedData(null, _dungeonCrawlerData.DungeonData.Dungeon));
            HandleDungeonDataStateChanged(_dungeonCrawlerData.DungeonData.Dungeon, _dungeonCrawlerData.DungeonData.HasChanged);
        }

        void OnDisable()
        {
            _dungeonCrawlerData.DungeonData.OnStateChanged.RemoveListener(HandleDungeonDataStateChanged);
            _dungeonCrawlerData.DungeonData.OnChange.RemoveListener(HandleDungeonChanged);
            _selectorToggleButton.onClick.AddListener(_dungeonSelectorPanel.Toggle);
        }

        private void HandleDungeonChanged(DungeonChangedData dungeon) => _nameLabel.text = dungeon.New?.Name;
        public void ShowExportPanel() => _exportManifestPanel.Toggle();
        public void Save() => _dungeonCrawlerData.DungeonData.SaveToManifest(_dungeonCrawlerData.ManifestData);
        private void HandleDungeonDataStateChanged(Dungeon dungeon, bool hasChanged) => _nameLabel.text = hasChanged ? $"{dungeon?.Name}*" : dungeon?.Name;
    }
}