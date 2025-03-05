using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity;
using CaptainCoder.Unity.UI;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class DungeonSelectorPanel : MonoBehaviour
    {
        [SerializeField]
        private DungeonData _dungeonData;
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
            Assertion.NotNull(this, _buttonTransform, _dungeonButtonPrefab, _dungeonController, _dungeonData);
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
                button.Initialize(d, _dungeonController.DungeonData.Dungeon.Name == d.Name);
                button.OnSelected.AddListener(TryOpenDungeon);
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

        private void WarnIfChanges(System.Action action)
        {
            if (_dungeonData.HasChanged)
            {
                _confirmPromptPanel.Prompt($"You have unsaved changes in <b>{_dungeonData.Dungeon.Name}</b>.\nAre you sure you want to continue?\n<b><color=red>This cannot be undone!</color></b>", action);
            }
            else
            {
                action.Invoke();
            }
        }

        public void PromptCreateDungeon() => WarnIfChanges(ShowNewDungeonPrompt);
        private void TryOpenDungeon(Dungeon d) => WarnIfChanges(() => OpenDungeon(d));

        private void ShowNewDungeonPrompt() => _textEntryDialogPanel.Prompt("Enter dungeon name", "Create Dungeon", "Cancel", CreateNewDungeon, ValidateDungeonName);

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
            newDungeon.SetBorderWalls(DungeonGlobals.Positions, WallType.Solid, DungeonGlobals.AllFacings);
            _dungeonController.ManifestData.Manifest.AddDungeon(newDungeon.Name, newDungeon);
            _dungeonController.DungeonData.Dungeon = newDungeon.Copy();
            Hide();
        }

        private void OpenDungeon(Dungeon d)
        {
            Hide();
            _dungeonData.Dungeon = d.Copy();
        }

        public void Hide() => gameObject.SetActive(false);
        public void Toggle() => gameObject.SetActive(!gameObject.activeSelf);

    }
}