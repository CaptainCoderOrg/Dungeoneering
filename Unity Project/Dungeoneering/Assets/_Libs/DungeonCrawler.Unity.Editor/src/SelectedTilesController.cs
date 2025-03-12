using System.Collections.Generic;

using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity.Assertions;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class SelectedTilesController : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private DungeonCrawlerData _dungeonCrawlerData;
        [field: SerializeField] public DungeonEditorSelectionData Selected { get; private set; }
        private HashSet<DungeonTile> _selectedTiles = new();
        private HashSet<DungeonTile> _altSelectedTiles = new();

        void OnEnable()
        {
            _dungeonCrawlerData.CurrentDungeon.OnChange += HandleDungeonChanged;
            Selected.AddListener(HandleSelectionChanged);
        }

        void OnDisable()
        {
            _dungeonCrawlerData.CurrentDungeon.OnChange -= HandleDungeonChanged;
            Selected.RemoveListener(HandleSelectionChanged);
        }

        private void HandleSelectionChanged(SelectionChangedData changes) => HandleSelectionChanged(changes.SelectedTiles);

        private void HandleSelectionChanged(ReadOnlySetView<DungeonTile> tiles)
        {
            foreach (var tile in tiles)
            {
                tile.IsSelected = true;
                _altSelectedTiles.Add(tile);
            }

            foreach (var tile in _selectedTiles)
            {
                if (!_altSelectedTiles.Contains(tile))
                    tile.IsSelected = false;
            }

            (_altSelectedTiles, _selectedTiles) = (_selectedTiles, _altSelectedTiles);
            _altSelectedTiles.Clear();
        }

        public void HandleDungeonChanged(DungeonChangedData _)
        {
            // Assuming this event only happens when a new dungeon is loaded.
            // If that changes, we'll need to do some checking/updating
            _selectedTiles.Clear();
            Selected.SetTileSelection(System.Array.Empty<DungeonTile>());
        }
    }
}