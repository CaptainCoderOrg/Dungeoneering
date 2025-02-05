using System.Collections.Generic;
using System.Linq;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using CaptainCoder.Dungeoneering.Unity.Editor;
using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity
{
    public class SelectedTilesInfo : MonoBehaviour
    {
        [field: SerializeField]
        public DungeonEditorSelectionData SelectionData { get; private set; }

        private SingleTileSelectedInfo _singleTile;

        void Awake()
        {
            _singleTile = GetComponentInChildren<SingleTileSelectedInfo>();
            Debug.Assert(_singleTile != null);
        }

        void OnEnable()
        {
            SelectionData.AddListener(HandleSelectionChanged);
        }

        void OnDisable()
        {
            SelectionData.RemoveListener(HandleSelectionChanged);
        }

        private void HandleSelectionChanged(List<DungeonTile> tiles)
        {
            if (tiles.Count == 0) { Debug.Log("No selection"); }
            else if (tiles.Count == 1) 
            {
                _singleTile.Selected = tiles.First();
            }
            foreach (DungeonTile tile in tiles)
            {
                Debug.Log($"Tile selected: {tile.Position}");
            }
        }
    }
}