using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using CaptainCoder.Unity;
using CaptainCoder.Unity.UI;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class DungeonSelectorPanel : MonoBehaviour
    {
        [SerializeField]
        private DungeonController _dungeonController;
        [SerializeField]
        private Transform _buttonTransform;
        [SerializeField]
        private DungeonSelectorButton _dungeonButtonPrefab;
        [SerializeField]
        private TextEntryDialogPanel _textEntryDialogPanel;
        [SerializeField]
        private ConfirmPromptPanel _confirmPromptPanel;
        void Awake()
        {
            Assertion.NotNull(this, _buttonTransform, _dungeonButtonPrefab, _dungeonController);
        }

        void OnEnable()
        {
            _dungeonController.ManifestData.AddListener(HandleManifestLoaded);
        }

        void OnDisable()
        {
            _dungeonController.ManifestData.RemoveListener(HandleManifestLoaded);
        }

        private void HandleManifestLoaded(DungeonCrawlerManifest manifest)
        {
            _buttonTransform.DestroyAllChildren();
            foreach ((string name, Dungeon d) in manifest.Dungeons)
            {
                DungeonSelectorButton button = Instantiate(_dungeonButtonPrefab, _buttonTransform);
                button.Dungeon = d;
                button.OnSelected.AddListener(BuildDungeon);
                button.OnRemoved.AddListener(PromptDeleteDungeon);
            }
        }

        public void PromptDeleteDungeon(Dungeon dungeon)
        {
            if (_dungeonController.DungeonData.Dungeon == dungeon)
            {
                _confirmPromptPanel.Prompt($"Cannot delete <b>{dungeon.Name}</b> because it is currently open.", null, null);
                return;
            }
            _confirmPromptPanel.Prompt($"Are you sure you want to delete <b>{dungeon.Name}</b>?\n<b><color=red>This cannot be undone!</color></b>", DeleteDungeon);
            void DeleteDungeon()
            {
                _dungeonController.ManifestData.RemoveDungeon(dungeon);
            }
        }

        public void PromptCreateDungeon()
        {
            _textEntryDialogPanel.Prompt("Enter dungeon name", "Create Dungeon", "Cancel", CreateNewDungeon, ValidateDungeonName);
        }

        private string ValidateDungeonName(string name)
        {
            if (!_dungeonController.ManifestData.Manifest.Dungeons.ContainsKey(name)) { return null; }
            return $"A dungeon named {name} already exists";
        }
        public void CreateNewDungeon(string name)
        {
            if (_dungeonController.ManifestData.Manifest.Dungeons.ContainsKey(name))
            {
                _textEntryDialogPanel.ShowError($"A dungeon named {name} already exists.");
                return;
            }

            Dungeon newDungeon = new() { Name = name };
            _dungeonController.ManifestData.Manifest.AddDungeon(newDungeon.Name, newDungeon);
            _dungeonController.Build(newDungeon);
            Hide();
        }

        private void BuildDungeon(Dungeon d)
        {
            Hide();
            _dungeonController.Build(d);
        }


        public void Hide() => gameObject.SetActive(false);
        public void Toggle() => gameObject.SetActive(!gameObject.activeSelf);

    }
}