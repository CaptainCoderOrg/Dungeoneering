using System;
using System.Collections.Generic;
using System.Linq;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using UnityEngine;
using UnityEngine.Events;
namespace CaptainCoder.Dungeoneering.Unity
{
    [CreateAssetMenu(menuName = "DC/SelectionData")]
    public class DungeonEditorSelectionData : ObservableSO
    {
        [field: SerializeField]
        public UnityEvent<IEnumerable<DungeonTile>> OnTilesChanged { get; private set; } = new();
        [field: SerializeField]
        public UnityEvent<IEnumerable<DungeonWallController>> OnWallsChanged { get; private set; } = new();
        private HashSet<DungeonTile> _selectedTiles { get; set; } = new();
        public IEnumerable<DungeonTile> SelectedTiles => _selectedTiles;
        private HashSet<DungeonWallController> _selectedWalls { get; set; } = new();
        public IEnumerable<DungeonWallController> SelectedWalls => _selectedWalls;

        public void AddListener(UnityAction<IEnumerable<DungeonWallController>> onChange)
        {
            OnWallsChanged.AddListener(onChange);
            onChange.Invoke(_selectedWalls);
        }

        public void RemoveListener(UnityAction<IEnumerable<DungeonWallController>> onChange) => OnWallsChanged.RemoveListener(onChange);

        public void AddListener(UnityAction<IEnumerable<DungeonTile>> onChange)
        {
            OnTilesChanged.AddListener(onChange);
            onChange.Invoke(_selectedTiles);
        }

        public void RemoveListener(UnityAction<IEnumerable<DungeonTile>> onChange) => OnTilesChanged.RemoveListener(onChange);

        public void ToggleWallSelected(DungeonWallController wall)
        {
            if (_selectedWalls.Contains(wall)) { _selectedWalls.Remove(wall); }
            else { _selectedWalls.Add(wall); }
            OnWallsChanged.Invoke(_selectedWalls);
        }

        public void AddWallSelection(params DungeonWallController[] walls)
        {
            _selectedWalls.UnionWith(walls);
            OnWallsChanged.Invoke(_selectedWalls);
        }
        public void SetWallSelection(params DungeonWallController[] wall)
        {
            ClearSelection();
            _selectedWalls.UnionWith(wall);
            OnWallsChanged.Invoke(_selectedWalls);
        }

        public void ClearSelection()
        {
            _selectedWalls.Clear();
            _selectedTiles.Clear();
            OnTilesChanged.Invoke(_selectedTiles);
            OnWallsChanged.Invoke(_selectedWalls);
        }

        public void AddTile(DungeonTile tile)
        {
            _selectedTiles.Add(tile);
            OnTilesChanged.Invoke(_selectedTiles);
        }

        public void AddSelection(IEnumerable<DungeonTile> tiles)
        {
            _selectedTiles.UnionWith(tiles);
            OnTilesChanged.Invoke(_selectedTiles);
        }

        public void ToggleTileSelected(DungeonTile tile)
        {
            if (_selectedTiles.Contains(tile)) { _selectedTiles.Remove(tile); }
            else { _selectedTiles.Add(tile); }
            OnTilesChanged.Invoke(_selectedTiles);
        }

        public void AddTileSelection(params DungeonTile[] tiles)
        {
            _selectedTiles.UnionWith(tiles);
            OnTilesChanged.Invoke(_selectedTiles);
        }

        public void SetTileSelection(IEnumerable<DungeonTile> tiles)
        {
            _selectedTiles.Clear();
            AddSelection(tiles);
        }

        public void SetTileSelection(params DungeonTile[] tiles)
        {
            _selectedTiles.Clear();
            AddTileSelection(tiles);
        }

        protected override void OnEnterPlayMode()
        {
            base.OnEnterPlayMode();
            // _originalElements = SelectedTiles.ToList();
        }

        protected override void OnExitPlayMode()
        {
            base.OnEnterPlayMode();
            _selectedTiles.Clear();
            OnTilesChanged.RemoveAllListeners();
        }

    }
}