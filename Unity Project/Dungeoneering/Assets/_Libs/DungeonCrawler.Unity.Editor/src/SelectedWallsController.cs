using System;
using System.Collections.Generic;
using System.Linq;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class SelectedWallsController : MonoBehaviour
    {
        [field: SerializeField]
        public DungeonEditorSelectionData Selected { get; private set; }
        [field: SerializeField]
        public SelectedDungeonWalls SelectionIndicatorPrefab { get; private set; }
        [field: SerializeField]
        public Transform IndicatorContainer { get; private set; }


        void OnEnable()
        {
            Selected.AddListener(HandleSelectionChanged);
        }

        void OnDisable()
        {
            Selected.AddListener(HandleSelectionChanged);
        }

        // TODO: Not Implemented
        private void HandleSelectionChanged(SelectionChangedData data) => throw new System.NotImplementedException();

        private void HandleSelectionChanged(IEnumerable<DungeonWallController> walls)
        {
            IndicatorContainer.DestroyAllChildren();
            foreach (DungeonWallController wall in walls)
            {
                SelectedDungeonWalls indicator = Instantiate(SelectionIndicatorPrefab, IndicatorContainer);
                indicator.transform.position = wall.Parent.transform.position;
                indicator.Facing = wall.Facing;
                indicator.gameObject.SetActive(true);
            }
            
        }
    }
}