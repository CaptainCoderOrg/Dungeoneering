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

        private SingleTileSelectedInfo _singleTileInfo;
        private MultiTileSelectedInfo _multiTileInfo;

        void Awake()
        {
            _singleTileInfo = GetComponentInChildren<SingleTileSelectedInfo>();
            Debug.Assert(_singleTileInfo != null, this);
            _multiTileInfo = GetComponentInChildren<MultiTileSelectedInfo>();
            Debug.Assert(_multiTileInfo != null, this);
        }

        void OnEnable()
        {
            SelectionData.AddListener(HandleSelectionChanged);
        }

        void OnDisable()
        {
            SelectionData.RemoveListener(HandleSelectionChanged);
        }

        private void HandleSelectionChanged(IEnumerable<DungeonTile> tiles)
        {
            _singleTileInfo.gameObject.SetActive(false);
            _multiTileInfo.gameObject.SetActive(false);
            if (tiles.Count() == 0) { Debug.Log("No selection"); }
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
            Debug.Log($"Tiles Selected: {string.Join(", ", tiles.Select(t => t.Position))}");
        }
    }
}