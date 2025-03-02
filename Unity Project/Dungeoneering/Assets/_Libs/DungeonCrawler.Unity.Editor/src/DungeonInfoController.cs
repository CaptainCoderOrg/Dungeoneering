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
            Assertion.NotNull(this, _nameLabel, _dungeonSelectorPanel, _selectorToggleButton, _dungeonData, _manifest);
        }

        void OnEnable()
        {
            _dungeonData.OnStateChanged.AddListener(HandleDungeonDataStateChanged);
            _dungeonData.OnChange.AddListener(HandleDungeonChanged);
            _selectorToggleButton.onClick.AddListener(_dungeonSelectorPanel.Toggle);
            StartCoroutine(UpdateUIAtEndOfFrame());
        }

        private IEnumerator UpdateUIAtEndOfFrame()
        {
            yield return null;
            HandleDungeonChanged(new DungeonChangedData(null, _dungeonData.Dungeon));
            HandleDungeonDataStateChanged(_dungeonData.Dungeon, _dungeonData.HasChanged);
        }

        void OnDisable()
        {
            _dungeonData.OnStateChanged.RemoveListener(HandleDungeonDataStateChanged);
            _dungeonData.OnChange.RemoveListener(HandleDungeonChanged);
            _selectorToggleButton.onClick.AddListener(_dungeonSelectorPanel.Toggle);
        }

        private void HandleDungeonChanged(DungeonChangedData dungeon)
        {
            _nameLabel.text = dungeon.New?.Name;
        }

        public void Save() => _dungeonData.SaveToManifest(_manifest);

        private void HandleDungeonDataStateChanged(Dungeon dungeon, bool hasChanged)
        {
            _nameLabel.text = hasChanged ? $"{dungeon?.Name}*" : dungeon?.Name;
        }
    }
}