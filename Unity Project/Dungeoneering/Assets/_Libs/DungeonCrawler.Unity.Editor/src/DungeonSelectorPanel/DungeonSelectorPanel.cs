using System.Collections.Generic;

using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity;
using CaptainCoder.Unity.UI;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class DungeonSelectorPanel : MonoBehaviour
    {
        [SerializeField]
        private DungeonCrawlerData _dungeonCrawlerData;
        public DungeonCrawlerData DungeonCrawlerData => _dungeonCrawlerData;
        [SerializeField]
        private Transform _buttonTransform;
        [SerializeField]
        private DungeonSelectorButton _dungeonButtonPrefab;
        [SerializeField]
        private TextEntryDialogPanel _textEntryDialogPanel;
        [SerializeField]
        private ConfirmPromptPanel _confirmPromptPanel;
        [SerializeField]
        private DungeonInfoPanel _dungeonInfoPanel;
        private readonly Dictionary<Dungeon, DungeonSelectorButton> _buttons = new();
        void Awake()
        {
            Assertion.NotNull(this, _buttonTransform, _dungeonButtonPrefab, _dungeonCrawlerData);
            Assertion.NotNull(this, (_dungeonInfoPanel, nameof(_dungeonInfoPanel)));
        }

        void OnEnable()
        {
            _dungeonCrawlerData.ManifestData.AddObserver(HandleManifestChanges);
        }

        void OnDisable()
        {
            _dungeonCrawlerData.ManifestData.RemoveObserver(HandleManifestChanges);
        }

        private void HandleManifestChanges(DungeonManifestChanged change)
        {
            switch (change)
            {
                case ManifestLoadedEvent(DungeonCrawlerManifest manifest):
                    HandleManifestLoaded(manifest);
                    break;
                case DungeonRemovedEvent(Dungeon removed):
                    HandleDungeonRemoved(removed);
                    break;
            }
        }

        private void HandleDungeonRemoved(Dungeon dungeon)
        {
            if (_buttons.Remove(dungeon, out var removed))
            {
                Destroy(removed.gameObject);
            }
        }

        private void HandleManifestLoaded(DungeonCrawlerManifest manifest)
        {
            _buttons.Clear();
            _buttonTransform.DestroyAllChildren();
            foreach ((string name, Dungeon d) in manifest.Dungeons)
            {
                DungeonSelectorButton button = Instantiate(_dungeonButtonPrefab, _buttonTransform);
                button.Initialize(d, _dungeonCrawlerData.CurrentDungeon.Name == d.Name);
                button.OnSelected.AddListener(TryOpenDungeon);
                button.OnRemoved.AddListener(PromptDeleteDungeon);
                button.OnInfo.AddListener(_dungeonInfoPanel.Show);
                _buttons[d] = button;
            }
        }

        public void PromptDeleteDungeon(Dungeon dungeon)
        {
            if (_dungeonCrawlerData.CurrentDungeon == dungeon)
            {
                _confirmPromptPanel.Prompt($"Cannot delete <b>{dungeon.Name}</b> because it is currently open.", null, null);
                return;
            }
            _confirmPromptPanel.Prompt($"Are you sure you want to delete <b>{dungeon.Name}</b>?\n<b><color=red>This cannot be undone!</color></b>", DeleteDungeon);
            void DeleteDungeon()
            {
                _dungeonCrawlerData.ManifestData.RemoveDungeon(dungeon);
            }
        }

        private void WarnIfChanges(System.Action action)
        {
            if (_dungeonCrawlerData.HasChanged)
            {
                _confirmPromptPanel.Prompt($"You have unsaved changes in <b>{_dungeonCrawlerData.CurrentDungeon.Name}</b>.\nAre you sure you want to continue?\n<b><color=red>This cannot be undone!</color></b>", action);
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
            if (!_dungeonCrawlerData.ManifestData.Manifest.Dungeons.ContainsKey(name)) { return null; }
            return $"A dungeon named {name} already exists";
        }

        public void CreateNewDungeon(string name)
        {
            Dungeon newDungeon = new() { Name = name };
            newDungeon.SetBorderWalls(DungeonGlobals.Positions, WallType.Solid, DungeonGlobals.AllFacings);
            if (!_dungeonCrawlerData.ManifestData.TryAddDungeon(newDungeon, out string error))
            {
                _textEntryDialogPanel.ShowError(error);
            }
            Hide();
        }

        private void OpenDungeon(Dungeon d)
        {
            Hide();
            _dungeonCrawlerData.LoadDungeon(d.Copy());
        }

        public void Hide() => gameObject.SetActive(false);
        public void Toggle() => gameObject.SetActive(!gameObject.activeSelf);

    }
}