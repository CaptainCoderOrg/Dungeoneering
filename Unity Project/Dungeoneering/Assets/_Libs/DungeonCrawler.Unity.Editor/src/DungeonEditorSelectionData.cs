using System.Collections.Generic;

using CaptainCoder.Dungeoneering.DungeonMap.Unity;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity
{
    [CreateAssetMenu(menuName = "DC/SelectionData")]
    public class DungeonEditorSelectionData : ObservableSO
    {
        private readonly HashSet<DungeonTile> _selectedTiles = new();
        private ReadOnlySetView<DungeonTile> _cachedView;
        public ReadOnlySetView<DungeonTile> Tiles => _cachedView ??= new(_selectedTiles);
        private readonly HashSet<DungeonWall> _walls = new();
        private ReadOnlySetView<DungeonWall> _cachedWalls;
        public ReadOnlySetView<DungeonWall> Walls => _cachedWalls ??= new(_walls);
        private System.Action<SelectionChangedEvent> _onDataChanged;
        private SelectionChangedEvent _changes;

        public void AddObserver(System.Action<SelectionChangedEvent> onChange)
        {
            _onDataChanged += onChange;
            onChange.Invoke(_changes ??= new SelectionChanged(Tiles, Walls));
        }
        public void RemoveObserver(System.Action<SelectionChangedEvent> onChange) => _onDataChanged -= onChange;
        private void Notify()
        {
            _changes ??= new SelectionChanged(Tiles, Walls);
            _onDataChanged?.Invoke(_changes);
        }

        public void ToggleWallSelected(DungeonWall wall)
        {
            if (!_walls.Add(wall)) { _walls.Remove(wall); }
            Notify();
        }

        public void SetWallSelection(params DungeonWall[] walls) => SetWallSelection((IEnumerable<DungeonWall>)walls);

        public void SetWallSelection(IEnumerable<DungeonWall> walls)
        {
            _walls.Clear();
            _selectedTiles.Clear();
            _walls.UnionWith(walls);
            Notify();
        }

        public void AddWallSelection(IEnumerable<DungeonWall> walls)
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

        protected override void OnExitPlayMode()
        {
            base.OnExitPlayMode();
            _onDataChanged = null;
            _walls.Clear();
            _selectedTiles.Clear();
        }

    }
}