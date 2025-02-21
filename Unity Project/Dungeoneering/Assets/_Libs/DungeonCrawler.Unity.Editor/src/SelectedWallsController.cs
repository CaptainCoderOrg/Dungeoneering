using System.Collections.Generic;

using CaptainCoder.Dungeoneering.DungeonMap.Unity;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class SelectedWallsController : MonoBehaviour
    {
        [field: SerializeField]
        public DungeonEditorSelectionData Selected { get; private set; }
        [field: SerializeField]
        public Transform IndicatorContainer { get; private set; }
        private HashSet<DungeonWallController> _selectedWalls = new();
        private HashSet<DungeonWallController> _altSelectedWalls = new();

        void OnEnable()
        {
            Selected.AddListener(HandleSelectionChanged);
        }

        void OnDisable()
        {
            Selected.AddListener(HandleSelectionChanged);
        }

        private void HandleSelectionChanged(SelectionChangedData data) => HandleSelectionChanged(data.SelectedWalls);
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
    }
}