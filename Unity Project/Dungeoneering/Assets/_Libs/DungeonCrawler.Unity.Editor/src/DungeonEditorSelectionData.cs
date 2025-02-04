using System.Collections.Generic;
using System.Linq;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using CaptainCoder.Dungeoneering.Player;
using UnityEngine;
using UnityEngine.Events;
namespace CaptainCoder.Dungeoneering.Unity
{
    [CreateAssetMenu(menuName = "DC/SelectionData")]
    public class DungeonEditorSelectionData : ObservableSO
    {
        [field: SerializeField]
        public UnityEvent<List<DungeonTile>> OnSelectionChanged { get; private set; } = new();
        private List<DungeonTile> _originalElements;
        [field: SerializeField]
        private List<DungeonTile> SelectedTiles { get; set; } = new();

        public void AddListener(UnityAction<List<DungeonTile>> onChange)
        {
            OnSelectionChanged.AddListener(onChange);
            onChange.Invoke(SelectedTiles);
        }

        public void RemoveListener(UnityAction<List<DungeonTile>> onChange) => OnSelectionChanged.RemoveListener(onChange);

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

        public void SetSelection(params DungeonTile[] tiles)
        {
            SelectedTiles.Clear();
            SelectedTiles.AddRange(tiles);
            OnSelectionChanged.Invoke(SelectedTiles);
        }

        protected override void OnEnterPlayMode()
        {
            base.OnEnterPlayMode();
            _originalElements = SelectedTiles.ToList();
        }

        protected override void OnExitPlayMode()
        {
            base.OnEnterPlayMode();
            SelectedTiles.Clear();
            SelectedTiles.AddRange(_originalElements);
            OnSelectionChanged.RemoveAllListeners();
        }
    }
}