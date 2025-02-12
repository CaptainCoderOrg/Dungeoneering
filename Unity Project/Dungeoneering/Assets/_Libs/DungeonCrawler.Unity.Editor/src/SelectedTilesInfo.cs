using System.Collections.Generic;
using System.Linq;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using CaptainCoder.Dungeoneering.Unity.Editor;
using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity
{
    public class SelectedTilesInfo : MonoBehaviour
    {
        [field: SerializeField]
        public DungeonEditorSelectionData SelectionData { get; private set; }
        private SingleWallInfo _singleWallInfo;
        private MultiWallInfo _multiWallInfo;

        void Awake()
        {
            _singleWallInfo = GetComponentInChildren<SingleWallInfo>(true);
            Debug.Assert(_singleWallInfo != null, this);
            _multiWallInfo = GetComponentInChildren<MultiWallInfo>(true);
            Debug.Assert(_multiWallInfo != null, this);
        }

        void OnEnable()
        {
            SelectionData.AddListener(HandleWallsChanged);
        }

        void OnDisable()
        {
            SelectionData.RemoveListener(HandleWallsChanged);
        }

        private void HideAll()
        {
            _singleWallInfo.gameObject.SetActive(false);
            _multiWallInfo.gameObject.SetActive(false);
        }

        private void HandleWallsChanged(IEnumerable<DungeonWallController> walls)
        {
            HideAll();
            if (walls.Count() == 0) { }
            else if (walls.Count() == 1)
            {
                _singleWallInfo.Selected = walls.First();
                _singleWallInfo.gameObject.SetActive(true);
            }
            else
            {
                _multiWallInfo.Selected = walls;
                _multiWallInfo.gameObject.SetActive(true);
            }
        }

    }
}