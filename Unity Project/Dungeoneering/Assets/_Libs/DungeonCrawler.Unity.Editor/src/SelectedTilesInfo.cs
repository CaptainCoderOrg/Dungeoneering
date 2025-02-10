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

        private SingleTileSelectedInfo _singleTileInfo;
        private MultiTileSelectedInfo _multiTileInfo;
        private SingleWallInfo _singleWallInfo;
        private MultiWallInfo _multiWallInfo;

        void Awake()
        {
            _singleTileInfo = GetComponentInChildren<SingleTileSelectedInfo>(true);
            Debug.Assert(_singleTileInfo != null, this);
            _multiTileInfo = GetComponentInChildren<MultiTileSelectedInfo>(true);
            Debug.Assert(_multiTileInfo != null, this);
            _singleWallInfo = GetComponentInChildren<SingleWallInfo>(true);
            Debug.Assert(_singleWallInfo != null, this);
            _multiWallInfo = GetComponentInChildren<MultiWallInfo>(true);
            Debug.Assert(_multiWallInfo != null, this);
        }

        void OnEnable()
        {
            SelectionData.AddListener(HandleTilesChanged);
            SelectionData.AddListener(HandleWallsChanged);
        }

        void OnDisable()
        {
            SelectionData.RemoveListener(HandleTilesChanged);
            SelectionData.RemoveListener(HandleWallsChanged);
        }

        private void HideAll()
        {
            _singleTileInfo.gameObject.SetActive(false);
            _multiTileInfo.gameObject.SetActive(false);
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

        private void HandleTilesChanged(IEnumerable<DungeonTile> tiles)
        {
            HideAll();
            if (tiles.Count() == 0) { }
            else if (tiles.Count() == 1)
            {
                _singleTileInfo.Selected = tiles.First();
                _singleTileInfo.gameObject.SetActive(true);
            }
            else
            {
                _multiTileInfo.Selected = tiles;
                _multiTileInfo.gameObject.SetActive(true);
            }
        }
    }
}