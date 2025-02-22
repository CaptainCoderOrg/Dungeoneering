using System.Collections.Generic;

using CaptainCoder.Dungeoneering.DungeonMap.Unity;

using UnityEngine;
using UnityEngine.Events;
namespace CaptainCoder.Dungeoneering.Unity
{
    [CreateAssetMenu(menuName = "DC/SelectionData")]
    public class DungeonEditorSelectionData : ObservableSO
    {
        private readonly HashSet<DungeonTile> _selectedTiles = new();
        private ReadOnlySetView<DungeonTile> _cachedView;
        public ReadOnlySetView<DungeonTile> Tiles => _cachedView ??= new(_selectedTiles);
        private readonly HashSet<DungeonWallController> _walls = new();
        private ReadOnlySetView<DungeonWallController> _cachedWalls;
        public ReadOnlySetView<DungeonWallController> Walls => _cachedWalls ??= new(_walls);
        private readonly UnityEvent<SelectionChangedData> _onDataChanged = new();
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
            if (!_walls.Add(wall)) { _walls.Remove(wall); }
            Notify();
        }

        public void SetWallSelection(params DungeonWallController[] walls) => SetWallSelection((IEnumerable<DungeonWallController>)walls);

        public void SetWallSelection(IEnumerable<DungeonWallController> walls)
        {
            _walls.Clear();
            _selectedTiles.Clear();
            _walls.UnionWith(walls);
            Notify();
        }

        public void AddWallSelection(IEnumerable<DungeonWallController> walls)
        {
            _walls.UnionWith(walls);
            Notify();
        }
        public void ToggleTileSelected(DungeonTile tile)
        {
            if (!_selectedTiles.Add(tile)) { _selectedTiles.Remove(tile); }
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
            _walls.Clear();
            _selectedTiles.Clear();
            _selectedTiles.UnionWith(tiles);
            Notify();
        }

        protected override void OnExitEditMode()
        {
            base.OnExitEditMode();
        }

        protected override void OnExitPlayMode()
        {
            base.OnExitPlayMode();
            _onDataChanged.RemoveAllListeners();
            _walls.Clear();
            _selectedTiles.Clear();
        }

    }

    public class SelectionChangedData
    {
        public readonly ReadOnlySetView<DungeonTile> SelectedTiles;
        public readonly ReadOnlySetView<DungeonWallController> SelectedWalls;
        public SelectionChangedData(DungeonEditorSelectionData data) => (SelectedTiles, SelectedWalls) = (data.Tiles, data.Walls);
    }
}