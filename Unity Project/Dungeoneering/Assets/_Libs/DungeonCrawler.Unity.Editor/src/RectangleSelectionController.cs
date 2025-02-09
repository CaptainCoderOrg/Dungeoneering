using System.Collections.Generic;
using System.Linq;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using UnityEngine;
using UnityEngine.InputSystem;
namespace CaptainCoder.Dungeoneering.Unity
{
    public class RectangleSelectionController : MonoBehaviour
    {
        [field: SerializeField]
        private DungeonEditorSelectionData _selection;
        [field: SerializeField]
        private GameObject _cube;
        private bool _isDrag = false;
        private Vector3 _startPosition;

        void Awake()
        {
            Debug.Assert(_cube != null, this);
            _cube.SetActive(false);
        }

        private void ScaleCube(Vector3 corner1, Vector3 corner2)
        {
            corner1.y = .25f;
            corner2.y = -.25f;
            Vector3 center = (corner1 + corner2) * 0.5f;
            Vector3 scale = new (
                1 + Mathf.Abs(corner2.x - corner1.x),
                Mathf.Abs(corner2.y - corner1.y),
                1 + Mathf.Abs(corner2.z - corner1.z)
            );
            _cube.transform.position = center;
            _cube.transform.localScale = scale;
        }

        private Collider[] PerformBoxCast()
        {
            Vector3 center = _cube.transform.position;
            Vector3 halfExtents = _cube.transform.localScale * 0.45f;
            Vector3 direction = Vector3.down;
            float maxDistance = 0.1f;

            // Perform the BoxCast
            RaycastHit[] hits = Physics.BoxCastAll(center, halfExtents, direction, transform.rotation, maxDistance);
            return hits.Select(h => h.collider).Where(c => c != null).ToArray();
        }

        public void StartDrag(Vector3 position)
        {
            _isDrag = true;
            _startPosition = position;
            _cube.SetActive(true);
        }

        public void Drag(Vector3 position)
        {
            if (!_isDrag) { return; }
            ScaleCube(_startPosition, position);
        }

        public void EndDrag(Vector3 position)
        {
            if (!_isDrag) { return; }
            _isDrag = false;
            ScaleCube(_startPosition, position);
            Collider[] colliders = PerformBoxCast();
            List<DungeonTile> tiles = new();
            foreach (Collider c in colliders)
            {
                DungeonTile tile = c.gameObject.GetComponentInParent<DungeonTile>();
                if (tile == null) { continue; }
                tiles.Add(tile);
            }
            
            if (Keyboard.current.shiftKey.isPressed)
            {
                _selection.AddSelection(tiles);
            }
            else
            {
                _selection.SetSelection(tiles);
            }
            _cube.SetActive(false);
        }
    }
}