using System;
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
        public UnityEvent<IEnumerable<DungeonTile>> OnSelectionChanged { get; private set; } = new();
        // private List<DungeonTile> _originalElements;
        private HashSet<DungeonTile> SelectedTiles { get; set; } = new();

        public void AddListener(UnityAction<IEnumerable<DungeonTile>> onChange)
        {
            OnSelectionChanged.AddListener(onChange);
            onChange.Invoke(SelectedTiles);
        }

        public void RemoveListener(UnityAction<IEnumerable<DungeonTile>> onChange) => OnSelectionChanged.RemoveListener(onChange);

        public void ClearSelection()
        {
            SelectedTiles.Clear();
            OnSelectionChanged.Invoke(SelectedTiles);
        }

        public void AddTile(DungeonTile tile)
        {
            SelectedTiles.Add(tile);
            OnSelectionChanged.Invoke(SelectedTiles);
        }

        public void AddSelection(params DungeonTile[] tiles)
        {
            SelectedTiles.UnionWith(tiles);
            OnSelectionChanged.Invoke(SelectedTiles);
        }

        public void SetSelection(params DungeonTile[] tiles)
        {
            SelectedTiles.Clear();
            AddSelection(tiles);
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
            OnSelectionChanged.RemoveAllListeners();
        }

    }
}