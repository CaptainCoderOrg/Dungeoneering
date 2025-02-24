using System.Collections.Generic;

using CaptainCoder.Dungeoneering.DungeonMap.Unity;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class SelectedTilesController : MonoBehaviour
    {
        [field: SerializeField] public DungeonEditorSelectionData Selected { get; private set; }
        [field: SerializeField] public GameObject SelectionIndicatorPrefab { get; private set; }
        [field: SerializeField] public Transform IndicatorContainer { get; private set; }
        private HashSet<DungeonTile> _selectedTiles = new();
        private HashSet<DungeonTile> _altSelectedTiles = new();

        void OnEnable()
        {
            Selected.AddListener(HandleSelectionChanged);
        }

        void OnDisable()
        {
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

        public void HandleDungeonChanged(DungeonData dungeon)
        {
            // Assuming this event only happens when a new dungeon is loaded.
            // If that changes, we'll need to do some checking/updating
            _selectedTiles.Clear();
        }
    }
}