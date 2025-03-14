using System.Linq;

using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using CaptainCoder.Dungeoneering.Unity.Data;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class WallTypeButtons : MonoBehaviour
    {
        [SerializeField]
        private DungeonEditorSelectionData _selectionData;
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
            DungeonCrawlerData data = controller.DungeonCrawlerData;
            DungeonWall[] walls = _selectionData.Walls.ToArray();

            void Perform()
            {
                foreach (DungeonWall wall in walls)
                {
                    WallReference wallRef = wall.WallReference;
                    WallType originalWallType = wallRef.Dungeon.Walls[wallRef.Position, wallRef.Facing];
                    if (originalWallType == newWallType) { continue; }
                    data.SetWallType(wallRef, newWallType);
                }
            }
            data.PerformEditSerializeState($"Set WallType: {newWallType}", Perform);
        }

    }
}