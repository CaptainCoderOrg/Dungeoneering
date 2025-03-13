using System.Collections.Generic;

using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity.Assertions;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class SelectedWallsController : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private DungeonCrawlerData _dungeonCrawlerData;
        [field: SerializeField] public DungeonEditorSelectionData Selected { get; private set; }
        private HashSet<DungeonWallController> _selectedWalls = new();
        private HashSet<DungeonWallController> _altSelectedWalls = new();

        void OnEnable()
        {
            _dungeonCrawlerData.AddObserver(HandleDungeonChanged);
            Selected.AddObserver(HandleSelectionChanged);
        }

        void OnDisable()
        {
            _dungeonCrawlerData.RemoveObserver(HandleDungeonChanged);
            Selected.RemoveObserver(HandleSelectionChanged);
        }

        private void HandleSelectionChanged(SelectionChangedEvent data)
        {
            if (data is SelectionChanged selection)
            {
                HandleSelectionChanged(selection.Walls);
            }
        }

        private void HandleSelectionChanged(IEnumerable<DungeonWallController> newWalls)
        {
            foreach (var wall in newWalls)
            {
                wall.IsSelected = true;
                _altSelectedWalls.Add(wall);
            }

            foreach (var wall in _selectedWalls)
            {
                if (!_altSelectedWalls.Contains(wall))
                    wall.IsSelected = false;
            }

            (_altSelectedWalls, _selectedWalls) = (_selectedWalls, _altSelectedWalls);
            _altSelectedWalls.Clear();
        }

        public void HandleDungeonChanged(DungeonChangeEvent changes)
        {
            if (changes is DungeonLoaded or DefaultTileTextureChanged or DefaultWallTextureChanged)
            {
                // Assuming this event only happens when a new dungeon is loaded.
                // If that changes, we'll need to do some checking/updating 
                _selectedWalls.Clear();
                Selected.SetWallSelection(System.Array.Empty<DungeonWallController>());
            }
        }
    }
}