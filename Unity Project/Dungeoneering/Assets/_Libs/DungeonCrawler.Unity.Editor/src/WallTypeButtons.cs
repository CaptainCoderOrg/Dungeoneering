using System.Linq;

using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class WallTypeButtons : MonoBehaviour
    {
        [SerializeField]
        private DungeonEditorSelectionData _selectionData;
        [SerializeField]
        private UndoRedoStackData _undoRedoStackData;

        [field: SerializeField]
        public Button NoWallButton { get; private set; }
        [field: SerializeField]
        public Button SolidButton { get; private set; }
        [field: SerializeField]
        public Button DoorButton { get; private set; }
        [field: SerializeField]
        public Button SecretDoorButton { get; private set; }

        void Awake()
        {
            Debug.Assert(_selectionData != null, this);
            Debug.Assert(_undoRedoStackData != null, this);
            Debug.Assert(NoWallButton != null, this);
            Debug.Assert(SolidButton != null, this);
            Debug.Assert(DoorButton != null, this);
            Debug.Assert(SecretDoorButton != null, this);
        }

        void OnEnable()
        {
            NoWallButton.onClick.AddListener(SetNoWall);
            SolidButton.onClick.AddListener(SetSolid);
            DoorButton.onClick.AddListener(SetDoor);
            SecretDoorButton.onClick.AddListener(SetSecretDoor);
        }

        void OnDisable()
        {
            NoWallButton.onClick.RemoveListener(SetNoWall);
            SolidButton.onClick.RemoveListener(SetSolid);
            DoorButton.onClick.RemoveListener(SetDoor);
            SecretDoorButton.onClick.RemoveListener(SetSecretDoor);
        }

        private void SetNoWall() => SetWallType(WallType.None);
        private void SetSolid() => SetWallType(WallType.Solid);
        private void SetDoor() => SetWallType(WallType.Door);
        private void SetSecretDoor() => SetWallType(WallType.SecretDoor);

        private void SetWallType(WallType newWallType)
        {
            if (!_selectionData.Walls.Any()) { return; }
            DungeonManifestData manifest = _selectionData.Walls.First().Parent.Manifest;
            System.Action perform = default;
            System.Action undo = default;

            foreach (DungeonWallController wall in _selectionData.Walls)
            {
                Dungeon d = wall.Parent.Dungeon;
                Position p = wall.Parent.Position;
                Facing f = wall.Facing;
                WallType originalWallType = d.Walls[p, f];
                string originalTexture = d.GetWallTexture(p, f);
                string originalBackTexture = d.GetWallTexture(p.Step(f), f.Opposite());
                if (originalWallType == newWallType) { continue; }
                perform += () => manifest.SetWallType(d, p, f, newWallType);
                undo += () =>
                {
                    manifest.SetWallType(d, p, f, originalWallType);
                    manifest.SetWallTexture(d, p, f, originalTexture);
                    manifest.SetWallTexture(d, p.Step(f), f.Opposite(), originalBackTexture);
                };
            }

            _undoRedoStackData.PerformEdit($"Set WallType: {newWallType}", perform, undo, manifest);
        }

    }
}