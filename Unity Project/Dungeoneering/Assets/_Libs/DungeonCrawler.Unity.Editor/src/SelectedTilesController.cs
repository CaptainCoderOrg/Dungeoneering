using System;
using System.Collections.Generic;
using System.Linq;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class SelectedTilesController : MonoBehaviour
    {
        [field: SerializeField]
        public DungeonEditorSelectionData Selected { get; private set; }
        [field: SerializeField]
        public GameObject SelectionIndicatorPrefab { get; private set; }
        [field: SerializeField]
        public Transform IndicatorContainer { get; private set; }


        void OnEnable()
        {
            Selected.AddListener(HandleSelectionChanged);
        }

        void OnDisable()
        {
            Selected.RemoveListener(HandleSelectionChanged);
        }

        private void HandleSelectionChanged(SelectionChangedData changes) => HandleSelectionChanged(changes.SelectedTiles);

        private void HandleSelectionChanged(IEnumerable<DungeonTile> tiles)
        {
            IndicatorContainer.DestroyAllChildren();
            foreach (DungeonTile tile in tiles)
            {
                GameObject indicator = Instantiate(SelectionIndicatorPrefab, IndicatorContainer);
                indicator.transform.position = tile.Position.ToVector3();
                indicator.SetActive(true);
            }
            
        }
    }
}