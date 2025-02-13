using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using UnityEngine;
using UnityEngine.Events;
namespace CaptainCoder.Dungeoneering.Unity
{
    [CreateAssetMenu(menuName = "DC/SelectionData")]
    public class DungeonEditorSelectionData : ObservableSO
    {
        private HashSet<DungeonTile> _selectedTiles = new();
        private ReadOnlySetView<DungeonTile> _cachedView;
        public ReadOnlySetView<DungeonTile> SelectedTiles => _cachedView ??=  new (_selectedTiles);
        private HashSet<DungeonWallController> _selectedWalls = new();
        private ReadOnlySetView<DungeonWallController> _cachedWallsView;
        public ReadOnlySetView<DungeonWallController> SelectedWalls => _cachedWallsView ??= new(_selectedWalls);
        private UnityEvent<SelectionChangedData> _onDataChanged = new ();
        private SelectionChangedData _changes;

        public void AddListener(UnityAction<SelectionChangedData> onChange) => _onDataChanged.AddListener(onChange);
        public void RemoveListener(UnityAction<SelectionChangedData> onChange) => _onDataChanged.RemoveListener(onChange);
        private void Notify()
        {
            _changes ??= new SelectionChangedData(this);
            _onDataChanged.Invoke(_changes);
        }

        public void ToggleWallSelected(DungeonWallController wall)
        {
            if (_selectedWalls.Contains(wall)) { _selectedWalls.Remove(wall); }
            else { _selectedWalls.Add(wall); }
            Notify();
        }

        public void SetWallSelection(params DungeonWallController[] walls) => SetWallSelection((IEnumerable<DungeonWallController>)walls);
        
        public void SetWallSelection(IEnumerable<DungeonWallController> walls)
        {
            _selectedWalls.Clear();
            _selectedTiles.Clear();
            _selectedWalls.UnionWith(walls);
            Notify();
        }

        public void AddWallSelection(IEnumerable<DungeonWallController> walls) {
            _selectedWalls.UnionWith(walls);
            Notify();
        }
        public void ToggleTileSelected(DungeonTile tile)
        {
            if (_selectedTiles.Contains(tile)) { _selectedTiles.Remove(tile); }
            else { _selectedTiles.Add(tile); }
            Notify();
        }
        public void AddTileSelection(IEnumerable<DungeonTile> tiles)
        {
            _selectedTiles.UnionWith(tiles);
            Notify();
        }

        public void SetTileSelection(params DungeonTile[] tiles) => SetTileSelection((IEnumerable<DungeonTile>)tiles);
        public void SetTileSelection(IEnumerable<DungeonTile> tiles)
        {
            _selectedWalls.Clear();
            _selectedTiles.Clear();
            _selectedTiles.UnionWith(tiles);
            Notify();
        }

        protected override void OnEnterPlayMode()
        {
            base.OnEnterPlayMode();
        }

        protected override void OnExitPlayMode()
        {
            base.OnExitPlayMode();
            _onDataChanged.RemoveAllListeners();
        }

    }

    public class SelectionChangedData
    {
        public readonly ReadOnlySetView<DungeonTile> SelectedTiles;
        public readonly ReadOnlySetView<DungeonWallController> SelectedWalls;
        public SelectionChangedData(DungeonEditorSelectionData data) => (SelectedTiles, SelectedWalls) = (data.SelectedTiles, data.SelectedWalls);
    }
}