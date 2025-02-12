using System.Collections.Generic;
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
            ClearTiles();
            OnWallsChanged.Invoke(_selectedWalls);
        }

        public void SetWallSelection(params DungeonWallController[] wall)
        {
            ClearTiles();
            _selectedWalls.Clear();
            _selectedWalls.UnionWith(wall);
            OnWallsChanged.Invoke(_selectedWalls);
        }

        public void ClearTiles()
        {
            if (_selectedTiles.Count == 0) { return; }
            _selectedTiles.Clear();
            OnTilesChanged.Invoke(_selectedTiles);
        }

        public void ClearWalls()
        {
            if (_selectedWalls.Count == 0) { return; }
            _selectedWalls.Clear();
            OnWallsChanged.Invoke(_selectedWalls);
        }

        public void ToggleTileSelected(DungeonTile tile)
        {
            if (_selectedTiles.Contains(tile)) { _selectedTiles.Remove(tile); }
            else { _selectedTiles.Add(tile); }
            ClearWalls();
            OnTilesChanged.Invoke(_selectedTiles);
        }

        public void AddTileSelection(params DungeonTile[] tiles) => AddTileSelection((IEnumerable<DungeonTile>)tiles);
        public void AddTileSelection(IEnumerable<DungeonTile> tiles)
        {
            ClearWalls();
            _selectedTiles.UnionWith(tiles);
            OnTilesChanged.Invoke(_selectedTiles);
        }

        public void SetTileSelection(params DungeonTile[] tiles) => SetTileSelection((IEnumerable<DungeonTile>)tiles);
        public void SetTileSelection(IEnumerable<DungeonTile> tiles)
        {
            _selectedTiles.Clear();
            AddTileSelection(tiles);
        }

        protected override void OnEnterPlayMode()
        {
            base.OnEnterPlayMode();
        }

        protected override void OnExitPlayMode()
        {
            base.OnExitPlayMode();
            _selectedTiles.Clear();
            _selectedWalls.Clear();
            OnTilesChanged.RemoveAllListeners();
        }

    }
}