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
            Selected.OnWallsChanged.AddListener(HandleSelectionChanged);
        }

        void OnDisable()
        {
            Selected.OnWallsChanged.RemoveListener(HandleSelectionChanged);
        }

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