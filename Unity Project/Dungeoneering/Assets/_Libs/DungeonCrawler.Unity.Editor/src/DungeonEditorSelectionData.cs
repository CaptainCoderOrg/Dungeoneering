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
        // private List<DungeonTile> _originalElements;
        private HashSet<DungeonTile> SelectedTiles { get; set; } = new();
        [field: SerializeField]
        private HashSet<DungeonWallController> SelectedWalls { get; set; } = new();

        public void AddListener(UnityAction<IEnumerable<DungeonWallController>> onChange)
        {
            OnWallsChanged.AddListener(onChange);
            onChange.Invoke(SelectedWalls);
        }

        public void RemoveListener(UnityAction<IEnumerable<DungeonWallController>> onChange) => OnWallsChanged.RemoveListener(onChange);

        public void AddListener(UnityAction<IEnumerable<DungeonTile>> onChange)
        {
            OnTilesChanged.AddListener(onChange);
            onChange.Invoke(SelectedTiles);
        }

        public void RemoveListener(UnityAction<IEnumerable<DungeonTile>> onChange) => OnTilesChanged.RemoveListener(onChange);

        public void AddWallSelection(params DungeonWallController[] walls)
        {
            SelectedWalls.UnionWith(walls);
            OnWallsChanged.Invoke(SelectedWalls);
        }
        public void SetWallSelection(params DungeonWallController[] wall)
        {
            ClearSelection();
            SelectedWalls.UnionWith(wall);
            OnWallsChanged.Invoke(SelectedWalls);
        }

        public void ClearSelection()
        {
            SelectedWalls.Clear();
            SelectedTiles.Clear();
            OnTilesChanged.Invoke(SelectedTiles);
            OnWallsChanged.Invoke(SelectedWalls);
        }

        public void AddTile(DungeonTile tile)
        {
            SelectedTiles.Add(tile);
            OnTilesChanged.Invoke(SelectedTiles);
        }

        public void AddSelection(IEnumerable<DungeonTile> tiles)
        {
            SelectedTiles.UnionWith(tiles);
            OnTilesChanged.Invoke(SelectedTiles);
        }

        public void AddTileSelection(params DungeonTile[] tiles)
        {
            SelectedTiles.UnionWith(tiles);
            OnTilesChanged.Invoke(SelectedTiles);
        }

        public void SetSelection(IEnumerable<DungeonTile> tiles)
        {
            SelectedTiles.Clear();
            AddSelection(tiles);
        }

        public void SetTileSelection(params DungeonTile[] tiles)
        {
            SelectedTiles.Clear();
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
            SelectedTiles.Clear();
            OnTilesChanged.RemoveAllListeners();
        }

    }
}