using System.Linq;

using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity;

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
            DungeonController controller = _selectionData.Walls.First().Parent.DungeonController;
            DungeonData dungeonData = controller.DungeonCrawlerData.CurrentDungeon;
            System.Action perform = default;
            System.Action undo = default;

            foreach (DungeonWallController wall in _selectionData.Walls)
            {
                Dungeon d = wall.Parent.Dungeon;
                Position p = wall.Parent.Position;
                Facing f = wall.Facing;
                WallType originalWallType = d.Walls[p, f];
                TextureId originalTexture = dungeonData.GetWallTexture(p, f);
                TextureId originalBackTexture = dungeonData.GetWallTexture(p.Step(f), f.Opposite());
                if (originalWallType == newWallType) { continue; }
                perform += () => dungeonData.SetWallType(p, f, newWallType);
                undo += () =>
                {
                    dungeonData.SetWallType(p, f, originalWallType);
                    dungeonData.SetWallTexture(p, f, originalTexture);
                    Position neighbor = p.Step(f);
                    if (controller.HasTile(neighbor))
                    {
                        dungeonData.SetWallTexture(neighbor, f.Opposite(), originalBackTexture);
                    }
                };
            }

            _undoRedoStackData.PerformEdit($"Set WallType: {newWallType}", perform, undo, dungeonData);
        }

    }
}