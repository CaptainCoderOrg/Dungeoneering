using System.Collections.Generic;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity
{
    public class SelectedTilesInfo : MonoBehaviour
    {
        [field: SerializeField]
        public DungeonEditorSelectionData SelectionData { get; private set; }

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
            foreach (DungeonTile tile in tiles)
            {
                Debug.Log($"Tile selected: {tile.Position}");
            }
        }
    }
}